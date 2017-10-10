using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Bean.Middler {
    /// <summary>
    /// 创建方法
    /// </summary>
    public enum CreateMethod {
        /// <summary>
        /// 构造函数
        /// </summary>
        Constructor,
        /// <summary>
        /// 工厂方法（无参构造函数，其余为参数方法）
        /// </summary>
        Factory,
        /// <summary>
        /// Bean方式（无参构造函数，Set方法设置属性）
        /// </summary>
        Bean,
        /// <summary>
        /// 先构造后Bean属性方式混用
        /// </summary>
        ConstructorBean,
        /// <summary>
        /// 先工厂后Bean属性方式混用
        /// </summary>
        FactoryBean
    }
}
