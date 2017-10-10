using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.IO.Log {
    /// <summary>
    /// 用于桌面输出!
    /// </summary>
    public class ConsoleLogResource : ILogResource {

        #region ILogResource Members

        public void Init() {
        }

        public void Close() {
        }

        public void Write(LogRecord record, string value) {
            Console.Write(value);
        }

        #endregion
    }
}
