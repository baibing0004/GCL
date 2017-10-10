using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.IO.Log {
    public interface ILogResource {
        /// <summary>
        /// 初始化
        /// </summary>
        void Init();

        /// <summary>
        /// 关闭输出
        /// </summary>
        void Close();

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="record"></param>
        /// <param name="value"></param>
        void Write(LogRecord record, string value);
    }
}
