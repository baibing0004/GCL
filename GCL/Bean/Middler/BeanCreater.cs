using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Bean.Middler {
    public class BeanCreater : AObjectCreater {

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="dll"></param>
        /// <param name="type"></param>
        /// <param name="paras"></param>
        public BeanCreater(string dll, string type, CreaterParameters paras) : base(dll, type, paras) { }


        public override object GetObject() {
            object v = BeanTool.CreateInstance(this.GetDll(), this.GetTypeName());
            //string name = v.GetType().Name;
            string[] names = this.GetCreaterParameters().GetNames();
            object[] values = this.GetCreaterParameters().GetParameters();
            if (v == null) { throw new MiddlerException(this.GetDll(), this.GetTypeName(), CreateMethod.Bean, values, "创建对象为空", null); }
            for (int w = 0; w < names.Length; w++)
                try {
                    //这里使用后门
                    if (values[w] is object[] && ParamsCreaterParameter.ParamsCreaterParameterKey.Equals(((object[])values[w])[0])) {
                        BeanTool.SetPropertyValue(v, names[w], ((object[])((object[])values[w])[1]));
                    } else
                        BeanTool.SetPropertyValueSP(v, names[w], values[w]);
                } catch (Exception ex) {
                    throw new MiddlerException(this.GetDll(), this.GetTypeName(), CreateMethod.Bean, values, string.Format("设置属性{0}时出错", names[w]), ex);
                }
            return v;
        }
    }
}
