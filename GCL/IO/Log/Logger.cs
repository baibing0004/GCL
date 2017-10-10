using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.IO.Log {
    public interface ILogListener {
        void Init(object sender, GCL.Event.EventArg e);
        void Log(object sender, GCL.Event.EventArg e);
        void Close(object sender, GCL.Event.EventArg e);
    }
    public class Logger : IDisposable {
        private LogRecord record;

        private bool isEnable = true;
        public void SetEnable(bool value) {
            this.isEnable = value;
        }
        public bool GetEnable() {
            return isEnable;
        }

        public LogRecord DefaultLogRecord {
            get {
                return record;
            }
        }

        public ILogListener Listener {
            set {
                AddListener(value);
            }
        }

        /// <summary>
        /// 设置订阅者
        /// </summary>
        /// <param name="listener"></param>
        public virtual void AddListener(ILogListener listener) {
            listener.Init(this, null);
            this.LogEvent += new GCL.Event.EventHandle(listener.Log);
            this.CloseEvent += new GCL.Event.EventHandle(listener.Close);
        }

        /// <summary>
        /// 使用桥接类实现灵活配置的默认参数/格式化/多资源日志记录！
        /// </summary>
        /// <param name="record">默认参数</param>
        public Logger(LogRecord record) {
            this.record = record;
            this.LogEvent += new GCL.Event.EventHandle(Event.EventArg._EventHandleDefault);
            this.CloseEvent += new GCL.Event.EventHandle(Event.EventArg._EventHandleDefault);
        }

        /// <summary>
        /// 记录日志时触发 参数为GetPara(0) LogType GetPara(1) string
        /// </summary>
        public event Event.EventHandle LogEvent;
        /// <summary>
        ///根据Logger状态判断是否可以合适的记录数据 
        /// </summary>
        /// <param name="record"></param>
        public virtual void Log(LogRecord record) {
            //if (this.isEnable && record.GetLogType() <= this.record.GetLogType()) {
            //    this.resource.Write(record.GetLogType(), this.formatter.ToString(record));
            if (this.isEnable)
                Event.EventArg.CallEventSafely(LogEvent, this, new GCL.Event.EventArg(record));
            //}
        }

        /// <summary>
        /// 便于记录其日志类型，内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void Log(LogType type, string data) {
            LogRecord record = this.record.Clone() as LogRecord;
            record.SetLogType(type);
            record.SetContent(data);
            this.Log(record);
        }

        /// <summary>
        /// 便于记录其日志类型，格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Log(LogType type, string data, params object[] param) {
            LogRecord record = this.record.Clone() as LogRecord;
            record.SetLogType(type);
            record.SetContent(data);
            record.SetParams(param);
            this.Log(record);
        }

        /// <summary>
        /// 便于记录其日志类型，日志号,内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="type"></param>
        /// <param name="num"></param>
        /// <param name="data"></param>
        public void Log(LogType type, int num, string data) {
            LogRecord record = this.record.Clone() as LogRecord;
            record.SetLogType(type);
            record.SetNum(num);
            record.SetContent(data);
            this.Log(record);
        }

        /// <summary>
        /// 便于记录其日志类型，日志号,格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="type"></param>
        /// <param name="num"></param>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Log(LogType type, int num, string data, params object[] param) {
            LogRecord record = this.record.Clone() as LogRecord;
            record.SetLogType(type);
            record.SetNum(num);
            record.SetContent(data);
            record.SetParams(param);
            this.Log(record);
        }

        /// <summary>
        /// 便于记录Release类型，内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void Release(string data) {
            this.Log(LogType.RELEASE, data);
        }

        /// <summary>
        /// 便于记录Release类型，日志号,内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="num"></param>
        /// <param name="data"></param>
        public void Release(int num, string data) {
            this.Log(LogType.RELEASE, num, data);
        }

        /// <summary>
        /// 便于记录Release类型，格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Release(string data, params object[] param) {
            this.Log(LogType.RELEASE, data, param);
        }

        /// <summary>
        /// 便于记录Release类型，日志号,格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="num"></param>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Release(int num, string data, params object[] param) {
            this.Log(LogType.RELEASE, num, data, param);
        }

        /// <summary>
        /// 便于记录Error类型，内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void Error(string data) {
            this.Log(LogType.ERROR, data);
        }

        /// <summary>
        /// 便于记录Error类型，日志号,内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="num"></param>
        /// <param name="data"></param>
        public void Error(int num, string data) {
            this.Log(LogType.ERROR, num, data);
        }

        /// <summary>
        /// 便于记录Error类型，格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Error(string data, params object[] param) {
            this.Log(LogType.ERROR, data, param);
        }

        /// <summary>
        /// 便于记录Error类型，日志号,格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="num"></param>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Error(int num, string data, params object[] param) {
            this.Log(LogType.ERROR, num, data, param);
        }

        /// <summary>
        /// 便于记录Warn类型，内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void Warn(string data) {
            this.Log(LogType.WARN, data);
        }

        /// <summary>
        /// 便于记录Warn类型，日志号,内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="num"></param>
        /// <param name="data"></param>
        public void Warn(int num, string data) {
            this.Log(LogType.WARN, num, data);
        }

        /// <summary>
        /// 便于记录Warn类型，格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Warn(string data, params object[] param) {
            this.Log(LogType.WARN, data, param);
        }

        /// <summary>
        /// 便于记录Warn类型，日志号,格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="num"></param>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Warn(int num, string data, params object[] param) {
            this.Log(LogType.WARN, num, data, param);
        }

        /// <summary>
        /// 便于记录Info类型，内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void Info(string data) {
            this.Log(LogType.INFO, data);
        }

        /// <summary>
        /// 便于记录Info类型，日志号,内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="num"></param>
        /// <param name="data"></param>
        public void Info(int num, string data) {
            this.Log(LogType.INFO, num, data);
        }

        /// <summary>
        /// 便于记录Info类型，格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Info(string data, params object[] param) {
            this.Log(LogType.INFO, data, param);
        }

        /// <summary>
        /// 便于记录Info类型，日志号,格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="num"></param>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Info(int num, string data, params object[] param) {
            this.Log(LogType.INFO, num, data, param);
        }

        /// <summary>
        /// 便于记录Debug类型，内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void Debug(string data) {
            this.Log(LogType.DEBUG, data);
        }

        /// <summary>
        /// 便于记录Debug类型，日志号,内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="num"></param>
        /// <param name="data"></param>
        public void Debug(int num, string data) {
            this.Log(LogType.DEBUG, num, data);
        }

        /// <summary>
        /// 便于记录Debug类型，格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Debug(string data, params object[] param) {
            this.Log(LogType.DEBUG, data, param);
        }

        /// <summary>
        /// 便于记录Debug类型，日志号,格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="num"></param>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Debug(int num, string data, params object[] param) {
            this.Log(LogType.DEBUG, num, data, param);
        }

        /// <summary>
        /// 便于记录Test类型，内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void Test(string data) {
            this.Log(LogType.TEST, data);
        }

        /// <summary>
        /// 便于记录Test类型，日志号,内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="num"></param>
        /// <param name="data"></param>
        public void Test(int num, string data) {
            this.Log(LogType.TEST, num, data);
        }

        /// <summary>
        /// 便于记录Test类型，格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Test(string data, params object[] param) {
            this.Log(LogType.TEST, data, param);
        }

        /// <summary>
        /// 便于记录Test类型，日志号,格式化内容 其它采用默认值，默认值修改通过访问DefaultLogRecord
        /// </summary>
        /// <param name="num"></param>
        /// <param name="data">格式化字符串</param>
        /// <param name="param">内容</param>
        public void Test(int num, string data, params object[] param) {
            this.Log(LogType.TEST, num, data, param);
        }

        /// <summary>
        /// Logger关闭事件
        /// </summary>
        public event GCL.Event.EventHandle CloseEvent;

        /// <summary>
        /// Logger关闭事件
        /// </summary>
        public void Close() {
            Event.EventArg.CallEventSafely(CloseEvent, this, null);
        }

        /// <summary>
        /// 生成一个SourceLoggerDecorator
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Logger CreateSourceLoggerDecorator(string source) {
            return new LoggerSourceDecorator(this, source);
        }

        public Logger CreateSourceLoggerDecorator(object source) {
            return new LoggerSourceDecorator(this, source);
        }

        #region IDisposable Members

        public void Dispose() {
            this.Close();
        }

        #endregion
    }
}
