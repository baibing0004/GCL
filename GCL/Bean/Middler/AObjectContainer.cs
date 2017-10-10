using System;
using System.Collections.Generic;
using System.Text;
using GCL.IO.Config;
namespace GCL.Bean.Middler {
    /// <summary>
    /// 主要用于代理方式处理对象的保持（单例/新对象/池）
    /// 请外部人员在生成对象后最好返回这个对象
    /// </summary>
    public abstract class AObjectContainer : IClosable {
        private AObjectCreater creater;
        public AObjectContainer(AObjectCreater creater) {
            this.creater = creater;
        }
        /// <summary>
        /// 在不得已的情况下生成新对象
        /// </summary>
        /// <returns></returns>
        protected object CreateObject() {
            return this.creater.GetObject();
        }
        /// <summary>
        /// 用于提供对象
        /// </summary>
        /// <returns></returns>
        public abstract object GetValue();

        /// <summary>
        /// 用于缓存或者关闭对象
        /// </summary>
        /// <param name="v"></param>
        public abstract void SetValue(object v);
        public abstract void Dispose();
        public abstract void Close();
    }
}
