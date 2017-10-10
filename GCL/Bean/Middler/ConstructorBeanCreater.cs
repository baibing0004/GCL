using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Bean.Middler {
    public class ConstructorBeanCreater : AObjectCreater {

        private int constructorParaLength = 0;
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="dll"></param>
        /// <param name="type"></param>
        /// <param name="constructorParaLength">构造函数参数长度</param>
        /// <param name="paras"></param>
        public ConstructorBeanCreater(string dll, string type, int constructorParaLength, CreaterParameters paras)
            : base(dll, type, paras) {
            this.constructorParaLength = constructorParaLength;
            if (this.constructorParaLength == 0)
                throw new InvalidOperationException("ConstructorBean方式不支持0构造参数，请使用Bean方式！ method=\"Bean\"");
        }

        public override object GetObject() {
            string[] names = this.GetCreaterParameters().GetNames();
            object[] values = this.GetCreaterParameters().GetParameters();
            object[] constructorValues = new object[this.constructorParaLength];
            for (int w = 0; values != null && w < constructorParaLength; w++)
                constructorValues[w] = values[w];

            object v = null;
            try {
                if (BeanTool.IsEnable(this.GetDll()))
                    v = BeanTool.CreateInstance(this.GetDll(), this.GetTypeName(), constructorValues);
                //v = Activator.CreateInstanceFrom(this.GetDll(), this.GetTypeName(), true, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance, null, constructorValues, null, null, null).Unwrap();
                else
                    v = Activator.CreateInstance(Type.GetType(this.GetTypeName()), constructorValues);
                if (v == null) throw new Exception("创建对象为空!");
            } catch (Exception ex) {
                throw new MiddlerException(this.GetDll(), this.GetTypeName(), CreateMethod.ConstructorBean, constructorValues, ex);
            }

            for (int w = constructorParaLength; w < names.Length; w++)
                try {
                    BeanTool.SetPropertyValueSP(v, names[w], values[w]);
                } catch (Exception ex) {
                    throw new MiddlerException(this.GetDll(), this.GetTypeName(), CreateMethod.ConstructorBean, values, string.Format("设置属性{0}时出错", names[w]), ex);
                }
            return v;
        }
    }
}
