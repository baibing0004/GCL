using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace GCL.IO.Log {
    /// <summary>
    /// Windows日志
    /// </summary>
    public class WindowEventLogResource : ILogResource {

        #region ILogResource Members

        public void Init() {
        }

        public void Close() {
        }

        private string eventName = "";


        public WindowEventLogResource(string name) {
            this.eventName = name;
        }
        public WindowEventLogResource() : this("") { }
        /// <summary>
        /// 获取事件名
        /// </summary>
        public string EventName {
            get { return eventName; }
            set { eventName = value; }
        }

        public void Write(LogRecord record, string value) {
            if (!string.IsNullOrEmpty(this.eventName) && !EventLog.SourceExists(EventName))
                EventLog.CreateEventSource(EventName, EventName);
            EventLog.WriteEntry(EventName, record.GetContent(), this.ToEntityType(record.GetLogType()));
        }

        protected EventLogEntryType ToEntityType(LogType type) {
            switch (type) {
                case LogType.WARN:
                    return EventLogEntryType.Warning;
                case LogType.ERROR:
                    return EventLogEntryType.Error;
                default:
                    return EventLogEntryType.Information;
            }
        }
        #endregion
    }
}
