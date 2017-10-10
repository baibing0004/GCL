using System;
using System.Collections.Generic;
using System.Text;
using GCL.Event;
namespace GCL.IO.Log {

    /// <summary>
    /// ��¼���� �Ƿ񷢻�������ILogRecordFormatter����
    /// </summary>
    public class LogRecord : ILogRecordFormatter, ICloneable {
        public LogRecord() {
            this.SetLogType(LogType.INFO);
            this.SetNeedDateTime(true);
            this.SetNeedDiscript(false);
        }

        /// <summary>
        /// ���ü���
        /// </summary>
        /// <param name="type"></param>
        public LogRecord(LogType type) {
            this.SetLogType(type);
        }

        /// <summary>
        /// ���ü�����Ƿ���Ҫʱ�䣬�Ƿ���Ҫ��־�����������Ƿ���Ҫ�ָ���!
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
        ///  ��ȡ��־����
        /// </summary>
        public LogType GetLogType() {
            return this.logType;
        }

        /// <summary>
        ///  type
        ///            ������־����
        /// </summary>
        public void SetLogType(LogType type) {
            this.logType = type;
        }

        private bool needDiscript = false;

        /// <summary>
        ///  �Ƿ���Ҫ��־��������
        /// </summary>
        public bool IsNeedDiscript() {
            return needDiscript;
        }

        /// <summary>
        ///  needDiscript
        ///            �����Ƿ���Ҫ��־��������
        /// </summary>
        public void SetNeedDiscript(bool Discript) {
            this.needDiscript = Discript;
        }

        private string data = "";

        /// <summary>
        ///  ��ȡ��־��Ϣ
        /// </summary>
        public string GetContent() {
            return this.data;
        }

        /// <summary>
        /// ������־��Ϣ
        /// </summary>
        public void SetContent(string data) {
            this.data = data;
        }

        private string source = "";

        /// <summary>
        ///  ��ȡ��־Դ
        /// </summary>
        public string GetSource() {
            return this.source;
        }

        /// <summary>
        /// ������־Դ
        /// </summary>
        public void SetSource(string source) {
            this.source = source;
        }

        /// <summary>
        /// ������־Դ
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
        /// ������־��
        /// </summary>
        /// <param name="num"></param>
        public void SetNum(int num) {
            lognum = num;
        }

        /// <summary>
        /// ������־��
        /// </summary>
        /// <returns></returns>
        public int GetNum() {
            return lognum;
        }

        private bool needDateTime = false;

        /// <summary>
        ///  �Ƿ���Ҫ����ʱ��
        /// </summary>
        public bool IsNeedDateTime() {
            return this.needDateTime;
        }

        /// <summary>
        ///  needDateTime
        ///            ��������ʱ��
        /// </summary>
        public void SetNeedDateTime(bool needDateTime) {
            this.needDateTime = needDateTime;
        }

        private DateTime recordTime = DateTime.Now;
        /// <summary>
        /// ��¼ʱ��
        /// </summary>
        /// <returns></returns>
        public DateTime GetRecordTime() {
            return recordTime;
        }

        private object[] paras;

        /// <summary>
        ///  ��ȡ���� ���ǿ�ʱ����
        /// </summary>
        public object[] GetParams() {
            return paras;
        }

        /// <summary>
        ///  params
        ///            ���ò���
        /// </summary>
        public void SetParams(params object[] param) {
            this.paras = param;
        }
        private bool isNeedSeparator = false;
        /// <summary>
        /// �Ƿ���Ҫ�ָ���
        /// </summary>
        /// <returns></returns>
        public bool IsNeedSeparator() {
            return isNeedSeparator;
        }
        /// <summary>
        /// �����Ƿ���Ҫ�ָ���
        /// </summary>
        /// <param name="value"></param>
        public void SetNeedSeparator(bool value) {
            this.isNeedSeparator = value;
        }

        private string separator = "==================================================================" + IOTool.LineSeparator;

        /// <summary>
        /// ��÷ָ���
        /// </summary>
        /// <returns></returns>
        public string GetSeparator() {
            return separator;
        }

        /// <summary>
        /// ���÷ָ���
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
