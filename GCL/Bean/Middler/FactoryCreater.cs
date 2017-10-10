using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using GCL.IO.Config;

namespace GCL.Bean.Middler {
    public class FactoryCreater : AObjectCreater {
        private ObjectStaticFactory factory;
        /// <summary>
        /// 工厂方法新建对象
        /// </summary>
        /// <param name="dll"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        public FactoryCreater(string dll, string type, CreaterParameters paras)
            : base(dll, type.Substring(0, type.LastIndexOf(".")), paras) {
            this.factory = new ObjectStaticFactory(dll, type);
        }

        /// <summary>
        /// 这里很有可能产生无法实例化的对象
        /// </summary>
        /// <returns></returns>
        public override object GetObject() {
            return factory.GetObject(this.GetCreaterParameters().GetParameters());
        }
    }
}
