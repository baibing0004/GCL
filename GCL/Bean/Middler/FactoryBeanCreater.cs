using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GCL.Bean.Middler {
    public class FactoryBeanCreater : AObjectCreater {

        private int constructorParaLength = 0;
        private ObjectStaticFactory factory;
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="dll"></param>
        /// <param name="type"></param>
        /// <param name="constructorParaLength">构造函数参数长度</param>
        /// <param name="paras"></param>
        public FactoryBeanCreater(string dll, string type, int constructorParaLength, CreaterParameters paras)
            : base(dll, type.Substring(0, type.LastIndexOf(".")), paras) {
            factory = new ObjectStaticFactory(dll, type);

            this.constructorParaLength = constructorParaLength;
            if (this.constructorParaLength == 0)
                throw new InvalidOperationException("FactoryBean方式不支持0构造参数，请使用Bean方式！ method=\"Bean\"");
        }

        public override object GetObject() {
            string[] names = this.GetCreaterParameters().GetNames();
            object[] values = this.GetCreaterParameters().GetParameters();
            object[] constructorValues = new object[this.constructorParaLength];
            for (int w = 0; values != null && w < constructorParaLength; w++)
                constructorValues[w] = values[w];

            object v = factory.GetObject(constructorValues);

            for (int w = constructorParaLength; w < names.Length; w++)
                try {
                    BeanTool.SetPropertyValueSP(v, names[w], values[w]);
                } catch (Exception ex) {
                    throw new MiddlerException(this.GetDll(), this.GetTypeName(), CreateMethod.FactoryBean, values, string.Format("设置属性{0}时出错", names[w]), ex);
                }
            return v;
        }
    }
}
