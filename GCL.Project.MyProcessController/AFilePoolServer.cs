using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using GCL.IO.Log;

namespace GCL.Project.MyProcessController {
    /// <summary>
    /// 需要设置
    /// </summary>
    public abstract class AFilePoolServer : APoolServer {


        private string path;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">要遍历的文件夹路径</param>
        /// <param name="minCapacity">文件队列最小容量，当文件队列小于此容量时，生产者自动添加新目录下文件</param>
        /// <param name="createrNum">生产者最大值</param>
        /// <param name="createrWaittime">生产数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="customerNum">消费数据者线程数</param>
        /// <param name="customerWaittime">消费数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        public AFilePoolServer(string path, int minCapacity, int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime)
            : base(createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime) {
            this.path = path;
            this.minCapacity = minCapacity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">要遍历的文件夹路径</param>
        /// <param name="minCapacity">文件队列最小容量，当文件队列小于此容量时，生产者自动添加新目录下文件</param>
        /// <param name="createrNum">生产者最大值</param>
        /// <param name="createrWaittime">生产数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="customerNum">消费数据者线程数</param>
        /// <param name="customerWaittime">消费数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        /// <param name="createWaitTime">生产数据者线程生产数据时等待时间（单位 毫秒）</param>
        public AFilePoolServer(string path, int minCapacity, int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime, int createWaitTime)
            : base(createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime, createWaitTime) {
            this.path = path;
            this.minCapacity = minCapacity;
        }

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
                        throw new InvalidOperationException("值已经设定");
                    isSelect = value;
                }
            }
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
        private bool isFinishClose = false;

        public bool IsFinishClose {
            get { return isFinishClose; }
            set { isFinishClose = value; }
        }
        private object key2 = DateTime.Now;
        protected override object Create(object sender, GCL.Event.EventArg e) {

            #region 取数据深度遍历
            if (!IsSelect && fileQueue.Count <= this.minCapacity)
                try {
                    IsSelect = true;

                    if (dicStack.Count == 0 && dicQueue.Count > 0) {
                        //加入文件夹路径
                        dicStack.Push(dicQueue.Dequeue());
                        curDic = null;
                    }

                    if (dicStack.Count == 0 && dicQueue.Count == 0) {
                        this.CallProcessEventSimple(LogType.RELEASE, 302, "数据已经全部读取，系统请求停止!");
                        if (isFinishClose)
                            this.CallDispose();
                    } else {
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
                } catch (Exception ex) {
                    string e2 = ex.ToString();
                    this.CallProcessEventSimple(LogType.RELEASE, 305, ex.ToString());
                } finally {
                    try {
                        IsSelect = false;
                    } catch (InvalidOperationException) { }
                }
            #endregion

            try {
                lock (key2) {
                    return fileQueue.Dequeue();
                }
            } catch (InvalidOperationException) {
                GCL.Common.Tool.ObjectSleep(10000);
                throw new Exception("等数据");
            }
        }
    }
}
