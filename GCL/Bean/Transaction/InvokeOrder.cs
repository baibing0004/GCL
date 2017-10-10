using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Bean.Transaction {
    /// <summary>
    /// 声明Command在事务中执行的顺序
    /// </summary>
    public enum InvokeOrder {
        并行,
        串行
    }
}
