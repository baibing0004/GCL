using System;
using System.Collections.Generic;
using System.Text;
using GCL.Event;
namespace GCL.IO.Log {

    /// <summary>
    /// 记录参数 是否发挥作用试ILogRecordFormatter而定
    /// </summary>
    public class LogRecord : ILogRecordFormatter, ICloneable {
        public LogRecord() {
            this.SetLogType(LogType.INFO);
            this.SetNeedDateTime(true);
            this.SetNeedDiscript(false);
        }

        /// <summary>
        /// 设置级别
        /// </summary>
        /// <param name="type"></param>
        public LogRecord(LogType type) {
            this.SetLogType(type);
        }

        /// <summary>
        /// 设置级别和是否需要时间，是否需要日志级别描述，是否需要分隔符!
        /// </summary>
        /// <param name="type"></param>
        /// <param name="needDateTime"></param>
        /// <param name="needDiscript"></param>
        /// <param name="needSeparator"></param>
        public LogRecord(LogType type, bool needDateTime, bool needDiscript, bool needSeparator) {
            this.SetLogType(type);
            this.SetNeedDateTime(needDateTime);
            this.SetNeedDiscript(needDiscript);
            this.SetNeedSeparator(needSeparator);
        }

        private LogType logType;

        /// <summary>
        ///  获取日志级别
        /// </summary>
        public LogType GetLogType() {
            return this.logType;
        }

        /// <summary>
        ///  type
        ///            设置日志级别
        /// </summary>
        public void SetLogType(LogType type) {
            this.logType = type;
        }

        private bool needDiscript = false;

        /// <summary>
        ///  是否需要日志级别描述
        /// </summary>
        public bool IsNeedDiscript() {
            return needDiscript;
        }

        /// <summary>
        ///  needDiscript
        ///            设置是否需要日志级别描述
        /// </summary>
        public void SetNeedDiscript(bool Discript) {
            this.needDiscript = Discript;
        }

        private string data = "";

        /// <summary>
        ///  获取日志信息
        /// </summary>
        public string GetContent() {
            return this.data;
        }

        /// <summary>
        /// 设置日志信息
        /// </summary>
        public void SetContent(string data) {
            this.data = data;
        }

        private string source = "";

        /// <summary>
        ///  获取日志源
        /// </summary>
        public string GetSource() {
            return this.source;
        }

        /// <summary>
        /// 设置日志源
        /// </summary>
        public void SetSource(string source) {
            this.source = source;
        }

        /// <summary>
        /// 设置日志源
        /// </summary>
        /// <param name="source"></param>
        public void SetSource(object source) {
            if (source != null)
                this.source = source.GetType().FullName;
            else
                this.source = "";
        }

        private int lognum = 0;

        /// <summary>
        /// 设置日志号
        /// </summary>
        /// <param name="num"></param>
        public void SetNum(int num) {
            lognum = num;
        }

        /// <summary>
        /// 饭或日志号
        /// </summary>
        /// <returns></returns>
        public int GetNum() {
            return lognum;
        }

        private bool needDateTime = false;

        /// <summary>
        ///  是否需要日期时间
        /// </summary>
        public bool IsNeedDateTime() {
            return this.needDateTime;
        }

        /// <summary>
        ///  needDateTime
        ///            设置日期时间
        /// </summary>
        public void SetNeedDateTime(bool needDateTime) {
            this.needDateTime = needDateTime;
        }

        private DateTime recordTime = DateTime.Now;
        /// <summary>
        /// 记录时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetRecordTime() {
            return recordTime;
        }

        private object[] paras;

        /// <summary>
        ///  获取参数 当非空时视作
        /// </summary>
        public object[] GetParams() {
            return paras;
        }

        /// <summary>
        ///  params
        ///            设置参数
        /// </summary>
        public void SetParams(params object[] param) {
            this.paras = param;
        }
        private bool isNeedSeparator = false;
        /// <summary>
        /// 是否需要分割线
        /// </summary>
        /// <returns></returns>
        public bool IsNeedSeparator() {
            return isNeedSeparator;
        }
        /// <summary>
        /// 设置是否需要分割线
        /// </summary>
        /// <param name="value"></param>
        public void SetNeedSeparator(bool value) {
            this.isNeedSeparator = value;
        }

        private string separator = "==================================================================" + IOTool.LineSeparator;

        /// <summary>
        /// 获得分割线
        /// </summary>
        /// <returns></returns>
        public string GetSeparator() {
            return separator;
        }

        /// <summary>
        /// 设置分割线
        /// </summary>
        /// <param name="value"></param>
        public void SetSeparator(string value) {
            this.separator = value;
        }


        #region ICloneable Members

        public object Clone() {
            LogRecord record = this.MemberwiseClone() as LogRecord;
            record.recordTime = DateTime.Now;
            return record;
            //LogRecord record = new LogRecord();
            //Bean.BeanTool.Transport(this, record);
            //return record;
        }


        #endregion


        #region ILogRecordFormatter Members

        private string formatString = "{0}{1}{2}{3}{4}{5}";
        public string ToString(LogRecord record) {
            return string.Format(formatString, new object[] { (record.IsNeedSeparator() ? record.GetSeparator() : ""),
                (record.IsNeedDateTime()?IOTool.FormatDate(record.GetRecordTime())+" ":""),
                (record.IsNeedDiscript()?record.GetLogType().ToString()+" ":""),
                (this.lognum>0?lognum+" ":""),
                (IOTool.IsEnable(record.GetParams())?string.Format(record.GetContent(),record.GetParams()):record.GetContent()),
                (record.IsNeedSeparator()?IOTool.LineSeparator+record.GetSeparator():IOTool.LineSeparator)
            });
        }

        #endregion
    }
}
