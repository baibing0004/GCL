using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.IO.Log {
    /// <summary>
    /// Logger源装饰器，用于
    /// </summary>
    public class LoggerSourceDecorator : Logger {
        private Logger logger;
        private string source;
        /// <summary>
        /// Logger源装饰器
        /// </summary>
        /// <param name="logger">内部Logger</param>
        /// <param name="source">源参数</param>
        public LoggerSourceDecorator(Logger logger, string source)
            : base(logger.DefaultLogRecord) {
            this.logger = logger;
            this.source = source;
        }

        /// <summary>
        /// Logger源装饰器
        /// </summary>
        /// <param name="logger">内部Logger</param>
        /// <param name="source">源参数</param>
        public LoggerSourceDecorator(Logger logger, object source)
            : this(logger, source.GetType().FullName) {
        }
        /// <summary>
        ///根据Logger状态判断是否可以合适的记录数据 
        /// </summary>
        /// <param name="record"></param>
        public override void Log(LogRecord record) {
            record.SetSource(this.source);
            logger.Log(record);
        }

        /// <summary>
        /// 设置订阅者
        /// </summary>
        /// <param name="listener"></param>
        public override void AddListener(ILogListener listener) {
            logger.AddListener(listener);
        }
    }
}
