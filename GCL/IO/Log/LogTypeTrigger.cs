using System;
using System.Collections.Generic;
using System.Text;
using GCL.Module.Trigger;
namespace GCL.IO.Log {
    /// <summary>
    /// 日志级别触发器
    /// 根据最低、最高日志级别判断此次日志是否应该记录
    /// 请注意这里是&gt;=与&lt;=的关系。
    /// </summary>
    public class LogTypeTrigger : ATrigger {
        private LogType logType = LogType.RELEASE, maxLogType = LogType.RELEASE;

        /// <summary>
        /// 日志级别触发器
        /// 根据最低、最高日志级别判断此次日志是否应该记录
        /// </summary>
        /// <param name="logType">&gt;=最低日志级别</param>
        /// <param name="maxLogType">&lt;=最高日志级别</param>
        public LogTypeTrigger(LogType logType, LogType maxLogType) {
            this.maxLogType = logType;
            this.logType = maxLogType;
        }

        /// <summary>
        /// 日志级别触发器
        /// 根据最低、最高日志级别判断此次日志是否应该记录
        /// </summary>
        /// <param name="logType">&gt;=最低日志级别</param>
        public LogTypeTrigger(LogType logType) {
            this.logType = logType;
        }

        protected override GCL.Event.EventArg Attempt(GCL.Event.EventArg e) {
            LogType type = (e.GetPara(0) as LogRecord).GetLogType();
            if (type >= this.maxLogType && type <= this.logType)
                return e;
            else
                return null;
        }

        public override void ReSet() {
        }
    }
}
