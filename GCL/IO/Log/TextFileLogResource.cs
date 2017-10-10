using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GCL.Module.Trigger;
using System.Threading.Tasks;
using GCL.Event;
namespace GCL.IO.Log {
    /// <summary>
    /// 文件日志
    /// </summary>
    public class TextFileLogResource : ILogResource {


        private Encoding encoding;

        /// <summary>
        /// 字符集
        /// </summary>
        public Encoding Encoding {
            get { return encoding; }
        }

        private string nameFormat, filename = "";

        /// <summary>
        /// TextLog文件名
        /// </summary>
        public string Filename {
            get { return filename; }
        }

        /// <summary>
        /// TextLog文件名格式字符串
        /// </summary>
        public string NameFormat {
            get { return nameFormat; }
        }

        private long bufferSize;
        /// <summary>
        /// 缓存大小（Byte）
        /// </summary>
        public long BufferSize {
            get { return bufferSize; }
        }
        private INameTrigger trigger;
        private StringBuilder sb = new StringBuilder();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameFormat">TextLog文件名格式字符串</param>
        /// <param name="encoding">字符集</param>
        /// <param name="buffer">缓存大小（Byte）</param>
        /// <param name="trigger">文件名触发器（输入为空或者文件满足某些条件时重新产生文件名）</param>
        public TextFileLogResource(string nameFormat, Encoding encoding, long buffer, INameTrigger trigger) {
            this.nameFormat = ((nameFormat.StartsWith("/") || nameFormat.StartsWith("\\")) ? AppDomain.CurrentDomain.BaseDirectory : "") + nameFormat;
            this.encoding = encoding;
            this.bufferSize = buffer;
            this.trigger = trigger;
            this.filename = string.Format(this.nameFormat, trigger.Taste(null));
        }

        /// <summary>
        /// 默认为系统字符集，默认缓冲区4K
        /// </summary>
        /// <param name="nameFormat"></param>
        /// <param name="trigger"></param>
        public TextFileLogResource(string nameFormat, INameTrigger trigger) : this(nameFormat, IOTool.DefaultEncoding, trigger) { }

        /// <summary>
        /// 默认缓冲区4K
        /// </summary>
        /// <param name="nameFormat"></param>
        /// <param name="trigger"></param>
        public TextFileLogResource(string nameFormat, Encoding end, INameTrigger trigger) : this(nameFormat, end, (long)IOTool.KToBytes(1), trigger) { }


        private Threading.ProtectThread thread;
        private void _Write(object sender, EventArg e) {
            //简介nodejs下调优经验 将append操作改为定时执行,降低IO操作。
            EventTool.ObjectSleep(10000);
            if (!Directory.Exists(filename.Substring(0, filename.LastIndexOf('\\'))))
                Directory.CreateDirectory(filename.Substring(0, filename.LastIndexOf('\\')));

            var content = "";
            lock (sb) {
                if (sb.Length > 0) {
                    content = sb.ToString();
                    sb.Remove(0, sb.Length);}
            }
            File.AppendAllText(filename, content, encoding);
        }
        protected void WriteText() {
            string file = trigger.Taste(filename);
            if (!string.IsNullOrEmpty(file)) this.filename = string.Format(nameFormat, file);

            //有效降低锁SB的时间，但是可能导致日志写入顺序发生颠倒,丢失日志
            //如果缓冲区足够大就不会发生这种情况，鉴于文件写入效率问题很少可能发生，所以此写法废弃
            //(甚至发生写入文件访问冲突错误)
            //string v = "";
            //lock (sb) {
            //    v = sb.ToString();
            //    sb.Remove(0, sb.Length);
            //}
            //try {
            //    lock (writeKey) {
            //        File.AppendAllText(filename, v, encoding);
            //    }
            //} catch (Exception ex) {
            //    sb.Insert(0, v);
            //    throw ex;
            //}

            if (thread == null) {thread = new Threading.ProtectThread(new Threading.Run(this._Write)); thread.Start();}
        }

        #region ILogResource Members

        public void Init() {
        }

        public void Close() {
            if (sb != null)
                lock (sb) {
                    if (sb.Length > 0)
                        WriteText();
                    sb = null;
                }
        }

        public void Write(LogRecord logRecord, string value) {
            if (sb != null) {
                sb.Append(value);
                if (sb.Length >= bufferSize)
                    WriteText();
            }
        }

        #endregion
    }
}
