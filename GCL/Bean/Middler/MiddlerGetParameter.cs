using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Bean.Middler {
    /// <summary>
    /// 获取参数类
    /// </summary>
    public abstract class MiddlerGetParameter {
        private string app = null;
        /// <summary>
        /// 应用域名
        /// </summary>
        public string Application {
            get { return app; }
        }

        /// <summary>
        /// 中介者获取参数
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        public MiddlerGetParameter(string app) {
            this.app = app;
        }

        /// <summary>
        /// 访问者模式设定其访问的具体方法
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public abstract object GetValue(MiddlerConfig config);
    }

    /// <summary>
    /// 通过名字获取相关值
    /// </summary>
    public class NameMiddlerGetParameter : MiddlerGetParameter {
        private string name = "";

        public string Name {
            get { return name; }
        }

        /// <summary>
        /// 通过名字获取相关值
        /// </summary>
        /// <param name="app"></param>
        /// <param name="name"></param>
        public NameMiddlerGetParameter(string app, string name) : base(app) {
            this.name = name;
        }


        public override object GetValue(MiddlerConfig config) {
            return config.GetValueByName(Application, name);
        }
    }

    /// <summary>
    /// 返回此类型的所有相关值
    /// </summary>
    public class TypeMiddlerGetParameter : MiddlerGetParameter {
        private Type type=null;

        /// <summary>
        /// 返回此类型的所有相关值
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public TypeMiddlerGetParameter(string app, Type type) : base(app) {
            this.type = type;
        }

        public override object GetValue(MiddlerConfig config) {
            return config.GetValueByType(Application, type);
        }
    }
}
