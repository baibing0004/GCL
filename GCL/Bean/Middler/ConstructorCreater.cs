using System;
using System.Collections.Generic;
using System.Text;
using GCL.IO.Config;
namespace GCL.Bean.Middler {
    /// <summary>
    /// 构造函数方式生成对象
    /// </summary>
    public class ConstructorCreater : AObjectCreater {

        public ConstructorCreater(string dll, string type, CreaterParameters paras)
            : base(dll, type, paras) {
        }

        public override object GetObject() {
            object[] values = GetCreaterParameters().GetParameters();
            try {
                object v = null;
                if (BeanTool.IsEnable(this.GetDll()))
                    v = BeanTool.CreateInstance(this.GetDll(), this.GetTypeName(), values);
                //return Activator.CreateInstanceFrom(this.GetDll(), this.GetTypeName(), true, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance, null, values, null, null, null).Unwrap();
                else
                    v = Activator.CreateInstance(Type.GetType(this.GetTypeName()), values);
                if (v == null) throw new Exception("创建对象为空!");
                return v;
            } catch (Exception ex) {
                throw new MiddlerException(this.GetDll(), this.GetTypeName(), CreateMethod.Constructor, values, ex);
            }
        }
    }
}
