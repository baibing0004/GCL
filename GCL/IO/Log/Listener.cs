using System;
using System.Collections.Generic;
using System.Text;
using GCL.Module.Trigger;
namespace GCL.IO.Log {
    /// <summary>
    /// 默认Listener
    /// </summary>
    public class Listener : ILogListener {

        /// <summary>
        /// 是否全部Trigger都为真才记录，否则只要有一个Trigger为真那么就记录
        /// </summary>
        private bool isAnd;


        /// <summary>
        /// 根据isAnd,判断是否在Trigger全部通过的情况下
        /// </summary>
        /// <param name="triggers"></param>
        /// <param name="formatter"></param>
        /// <param name="resources"></param>
        /// <param name="isAnd">是否全部Trigger都为真才记录，否则只要有一个Trigger为真那么就记录,默认为真</param>
        public Listener(ATrigger[] triggers, ILogRecordFormatter formatter, ILogResource[] resources, bool isAnd) {
            if (!IOTool.IsEnable(triggers))
                throw new InvalidOperationException("条件触发器不能为空!");
            this.triggers = triggers;
            if (!IOTool.IsEnable(formatter))
                throw new InvalidOperationException("格式化不能为空!");
            this.formatter = formatter;
            if (!IOTool.IsEnable(resources))
                throw new InvalidOperationException("数据源不能为空!");
            this.resources = resources;
            this.isAnd = isAnd;
        }

        /// <summary>
        /// 根据Trigger如果全部通过的情况下
        /// </summary>
        /// <param name="triggers"></param>
        /// <param name="formatter"></param>
        /// <param name="resources"></param>
        public Listener(ATrigger[] triggers, ILogRecordFormatter formatter, ILogResource resources) : this(triggers, formatter, new ILogResource[] { resources }, true) { }
        /// <summary>
        /// 根据Trigger如果全部通过的情况下
        /// </summary>
        /// <param name="triggers"></param>
        /// <param name="formatter"></param>
        /// <param name="resources"></param>
        public Listener(ATrigger[] triggers, ILogRecordFormatter formatter, ILogResource[] resources) : this(triggers, formatter, resources, true) { }

        /// <summary>
        /// 根据Trigger如果全部通过的情况下
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="formatter"></param>
        /// <param name="res"></param>
        public Listener(ATrigger trigger, ILogRecordFormatter formatter, ILogResource res) : this(new ATrigger[] { trigger }, formatter, new ILogResource[] { res }) { }

        public static ATrigger[] ToATrigger(object[] source) {
            return Array.ConvertAll<object, ATrigger>(source, _ToATrigger);
        }
        protected static ATrigger _ToATrigger(object source) {
            return (ATrigger)source;
        }

        public static ILogResource[] ToILogResource(object[] source) {
            return Array.ConvertAll<object, ILogResource>(source, _ToILogResource);
        }
        protected static ILogResource _ToILogResource(object source) {
            return (ILogResource)source;
        }

        private ATrigger[] triggers;

        protected ATrigger[] Triggers {
            get { return triggers; }
        }
        private ILogRecordFormatter formatter;

        protected ILogRecordFormatter Formatter {
            get { return formatter; }
        }
        private ILogResource[] resources;

        protected ILogResource[] Resources {
            get { return resources; }
        }
        #region ILogListener Members

        public void Init(object sender, GCL.Event.EventArg e) {
            try {
                foreach (ILogResource res in resources)
                    res.Init();
            } catch {
                this.Close(sender, e);
                throw;
            }
        }

        public void Log(object sender, GCL.Event.EventArg e) {
            bool isLog = isAnd;
            for (int w = 0; (isAnd ? isLog : !isLog) && w < triggers.Length; w++)
                if (isAnd)
                    isLog &= triggers[w].Taste(e);
                else
                    isLog |= triggers[w].Taste(e);
            if (isLog) {
                LogRecord record = e.GetPara(0) as LogRecord;
                string data = formatter.ToString(record);
                foreach (ILogResource res in resources)
                    res.Write(record, data);
            }
        }

        public void Close(object sender, GCL.Event.EventArg e) {
            foreach (ILogResource res in resources)
                try {
                    res.Close();
                } catch {
                }
        }

        #endregion
    }
}
