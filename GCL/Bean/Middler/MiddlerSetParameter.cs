using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Bean.Middler {
    /// <summary>
    /// 设置参数类 继承Name参数类但是其调用的方法参数不同
    /// </summary>
    public class MiddlerSetParameter : NameMiddlerGetParameter {

        public MiddlerSetParameter(string app, string name) : base(app, name) { }
        public virtual void SetValue(MiddlerConfig config, object v) {
            config.SetValue(Application, Name, v);
        }
    }
}
