using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Bean.Middler {
    /// <summary>
    /// 创建模式
    /// </summary>
    public enum CreateMode {
        /// <summary>
        /// 静态的
        /// </summary>
        Static,
        /// <summary>
        /// 每次都新建
        /// </summary>
        Instance,
        /// <summary>
        /// 池方式
        /// </summary>
        Pool
    }
}
