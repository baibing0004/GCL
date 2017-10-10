using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using GCL.IO.Log;
namespace GCL.Project.MyProcessController {

    /// <summary>
    /// 主要用于逐行处理Text文件内容
    /// </summary>
    public abstract class ATextFilePoolServer : APoolServer {


        private string path, delpath;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">要遍历的文件夹路径</param>
        /// <param name="minCapacity">文件队列最小容量，当文件队列小于此容量时，生产者自动添加新目录下文件</param>
        /// <param name="delpath">处理文件夹，同时也是备份文件夹 默认将原文件名前加上日期时分秒</param>
        /// <param name="createrNum">生产者最大值</param>
        /// <param name="createrWaittime">生产数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="customerNum">消费数据者线程数</param>
        /// <param name="customerWaittime">消费数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        public ATextFilePoolServer(string path, int minCapacity, string delpath, int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime)
            : base(createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime) {
            this.path = path;
            this.delpath = delpath;
            this.minCapacity = minCapacity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">要遍历的文件夹路径</param>
        /// <param name="minCapacity">文件队列最小容量，当文件队列小于此容量时，生产者自动添加新目录下文件</param>
        /// <param name="delpath">处理文件夹，同时也是备份文件夹 默认将原文件名前加上日期时分秒</param>
        /// <param name="createrNum">生产者最大值</param>
        /// <param name="createrWaittime">生产数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="customerNum">消费数据者线程数</param>
        /// <param name="customerWaittime">消费数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        /// <param name="createWaitTime">生产数据者线程生产数据时等待时间（单位 毫秒）</param>
        public ATextFilePoolServer(string path, int minCapacity, string delpath, int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime, int createWaitTime)
            : base(createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime, createWaitTime) {
            this.path = path;
            this.delpath = delpath;
            this.minCapacity = minCapacity;
        }


        private Queue textQueue = new Queue();
        private TextReader reader;
        private FileInfo lastFile;
        private object key3 = DateTime.Now;
        private string errFile;
        protected override object Create(object sender, GCL.Event.EventArg e) {
            if (!IsSelect && textQueue.Count <= minCapacity) {
                try {
                    IsSelect = true;
                    for (int w = 0; w < minCapacity; w++) {
                        string v = (reader != null ? reader.ReadLine() : null);
                        if (v != null)
                            textQueue.Enqueue(v);
                        else {
                            if (lastFile != null) {
                                try {
                                    reader.Close();
                                } catch {
                                }
                                reader = null;
                                RemoveFile(lastFile);
                                lastFile = null;
                            }
                            lastFile = GetFile();
                            string filename = delpath + "/" + lastFile.Name + ".temp";
                            if (File.Exists(filename))
                                File.Delete(filename);
                            lastFile.MoveTo(filename);
                            lastFile = new FileInfo(filename);
                            errFile = lastFile.FullName.Replace(".temp", ".err");
                            reader = GetReader(lastFile);
                        }
                    }
                } catch (HasSelectedException) { } catch (InvalidOperationException) {//文件队列为空！
                }
                    //} catch (Exception ex) {
                    //    this.CallProcessEventSimple(403, ex.ToString());
                    //    throw ex;
                 finally {
                    IsSelect = false;
                }
            }
            lock (key3) {
                return textQueue.Dequeue();
            }
        }

        #region 处理文件
        private Stack dicStack = new Stack();
        private Queue fileQueue = new Queue();
        private Queue dicQueue = new Queue();
        private DirectoryInfo curDic = null;
        private int minCapacity = 50;
        public override void Init() {
            string[] paths = path.Split(';');
            foreach (string _path in paths) { this.CallProcessEventSimple(LogType.RELEASE, 101, "获取需要处理的文件路径" + _path); dicQueue.Enqueue(new DirectoryInfo(_path)); }
            base.Init();
        }
        /// <summary>
        /// isOver是否已经没有数据等待结束，是否正在有人读取信息
        /// </summary>
        private bool isSelect = false;
        private object key = DateTime.Now;
        /// <summary>
        /// 是否正在有人读取信息
        /// </summary>
        public bool IsSelect {
            get { return isSelect; }
            set {
                lock (key) {
                    if (isSelect && value)
                        throw new HasSelectedException("值已经设定");
                    isSelect = value;
                }
            }
        }

        class HasSelectedException : Exception {
            public HasSelectedException(string text) : base(text) { }
            public HasSelectedException(string text, Exception inner) : base(text, inner) { }
        }

        protected int GetSmallNum(string[] names, string name) {
            if (name.CompareTo(names[0]) <= 0)
                return 0;
            if (name.CompareTo(names[1]) <= 0)
                return 1;
            if (name.CompareTo(names[names.Length - 1]) > 0)
                return names.Length - 1;

            int l = 0, r, m;
            r = names.Length;
            while (r - l > 1) {
                m = (l + r) / 2;
                if (name.CompareTo(names[m]) >= 0)
                    l = m;
                else
                    r = m;
            }
            return l;
        }

        protected FileInfo GetFile() {

            #region 取数据深度遍历
            if (dicStack.Count == 0 && dicQueue.Count > 0) {
                //加入文件夹路径
                dicStack.Push(dicQueue.Dequeue());
                curDic = null;
            }

            if (dicStack.Count == 0 && dicQueue.Count == 0) {
                //当全部处理完成后，重新导入文件夹进行处理
                string[] paths = path.Split(';');
                foreach (string _path in paths) { dicQueue.Enqueue(new DirectoryInfo(_path)); }
                this.CallProcessEventSimple(LogType.DEBUG, 302, "数据已经全部读取，重新浏览所有的文件夹!" + path);
            }
            //this.CallProcessEventSimple(LogType.RELEASE, 302, "数据已经全部读取，系统请求停止!"); else
            {
                if (dicStack.Count > 0) {
                    DirectoryInfo dic = dicStack.Peek() as DirectoryInfo;
                    if (curDic != null) {
                        #region 获取当前文件夹所在位置
                        DirectoryInfo[] dics = dic.GetDirectories();
                        string[] dicnames = new string[dics.Length];
                        for (int w = 0; w < dics.Length; w++) dicnames[w] = dics[w].Name;
                        Array.Sort(dicnames);
                        int num = GetSmallNum(dicnames, curDic.Name);
                        #endregion
                        if (dicnames.Length > num + 1) {
                            //上级文件夹还没有处理完 遍历下一个文件夹
                            curDic = new DirectoryInfo(dic.FullName + "\\" + dicnames[num + 1]);
                            DirectoryInfo[] dicss = curDic.GetDirectories();
                            while (dicss != null && dicss.Length > 0) {
                                dicStack.Push(curDic);
                                curDic = dicss[0];
                                dicss = curDic.GetDirectories();
                            }
                        } else {
                            //上级文件夹已经处理完了
                            curDic = dicStack.Pop() as DirectoryInfo;
                        }
                    } else {
                        //处理第一个文件夹
                        curDic = dicStack.Pop() as DirectoryInfo;
                        DirectoryInfo[] dicss = curDic.GetDirectories();
                        while (dicss != null && dicss.Length > 0) {
                            dicStack.Push(curDic);
                            curDic = dicss[0];
                            dicss = curDic.GetDirectories();
                        }
                    }
                }

                foreach (FileInfo info in curDic.GetFiles())
                    fileQueue.Enqueue(info);
            }

            #endregion
            return fileQueue.Dequeue() as FileInfo;
        }
        #endregion

        /// <summary>
        /// 获取Reader
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected abstract TextReader GetReader(FileInfo info);

        /// <summary>
        /// 处理读完的文件
        /// </summary>
        /// <param name="info"></param>
        protected abstract void RemoveFile(FileInfo info);

        protected virtual void AddErrFile(string content) {
            if (!string.IsNullOrEmpty(errFile))
                File.AppendAllText(errFile, content);
        }
        protected override void OnClose(object sender, GCL.Event.EventArg e) {
            if (lastFile != null && lastFile.Exists) {
                lock (this) {
                    FileInfo last = lastFile;
                    lastFile = null;
                    if (reader != null) {
                        string filename = path.Split(';')[0] + "\\" + last.Name.Replace(".temp", "");
                        StringBuilder sb = new StringBuilder();
                        while (textQueue.Count > 0) {
                            sb.AppendLine((string)textQueue.Dequeue());
                        }
                        File.WriteAllText(filename, sb.ToString());
                        sb.Remove(0, sb.Length);
                        File.WriteAllText(filename, reader.ReadToEnd());
                    }
                    try {
                        reader.Close();
                    } catch {
                    }
                    last.Delete();
                }
            }
            base.OnClose(sender, e);
        }
    }
}
