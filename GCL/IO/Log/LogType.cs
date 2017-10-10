using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.IO.Log {
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogType {
        /// <summary>
        /// 最终的需要显示的正确结果
        ///</summary>
        RELEASE = 0,

        /// <summary>
        /// 程序错误
        ///</summary>
        ERROR = 1,

        /// <summary>
        /// 可能引发错误的警告
        ///</summary>
        WARN = 2,

        /// <summary>
        /// 程序日志信息（默认信息）
        ///</summary>
        INFO = 3,

        /// <summary>
        /// 程序调试信息
        ///</summary>
        DEBUG = 4,

        /// <summary>
        /// 测试信息说明
        ///</summary>
        TEST = 5,

        /// <summary>
        /// 全部的，一般作为默认参数使用
        /// </summary>
        All=255
    }
}
