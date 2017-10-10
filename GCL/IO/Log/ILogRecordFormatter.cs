using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.IO.Log {
    /// <summary>
    /// LogRecord转换为String
    /// </summary>
    public interface ILogRecordFormatter {
        string ToString(LogRecord record);
    }
}
