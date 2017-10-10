using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using GCL.IO.Config;
namespace GCL.Bean.Middler {
    /// <summary>
    /// 主要用于给生成者类提供各项参数
    /// </summary>
    public class CreaterParameters : IClosable {
        private LinkedList<ACreaterParameter> paralist = new LinkedList<ACreaterParameter>();
        private ConfigManager config = null;
        public CreaterParameters(ConfigManager ma) {
            this.config = ma;
        }

        /// <summary>
        /// 获取参数的名字
        /// </summary>
        /// <returns></returns>
        public string[] GetNames() {
            string[] v = new string[paralist.Count];
            int w = 0;
            for (IEnumerator<ACreaterParameter> ienum = paralist.GetEnumerator(); ienum.MoveNext(); w++)
                v[w] = ienum.Current.GetName();
            return v;
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <returns></returns>
        public object[] GetParameters() {
            object[] v = new object[paralist.Count];
            int w = 0;
            for (IEnumerator<ACreaterParameter> ienum = paralist.GetEnumerator(); ienum.MoveNext(); w++)
                v[w] = ienum.Current.GetValue(config);
            return v;
        }

        public void AddParameter(ACreaterParameter para) {
            paralist.AddLast(para);
        }

        #region IClosable Members

        public void Close() {
            paralist.Clear();
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            try {
                this.Close();
            } catch {
            }
        }

        #endregion
    }
    /// <summary>
    /// 产生具体参数
    /// </summary>
    public abstract class ACreaterParameter {
        private string name = null;
        public ACreaterParameter(string name) {
            this.name = name;
        }
        public string GetName() {
            return name;
        }
        public abstract object GetValue(ConfigManager config);
    }

    /// <summary>
    /// 枚举类型对象 需要声明其全路径类型与值
    /// </summary>
    public class EnumCreaterParameter : ACreaterParameter {
        private string dll, type, value;
        private ObjectStaticFactory factory;
        public EnumCreaterParameter(string name, string dll, string value)
            : base(name) {
            this.dll = dll;
            this.type = value.Substring(0, value.LastIndexOf("."));
            this.value = value.Substring(value.LastIndexOf(".") + 1);
            factory = new ObjectStaticFactory(dll, value);
        }

        public override object GetValue(ConfigManager config) {
            return factory.GetStaticObject().GetField(this.value).GetValue(null);
        }
    }

    /// <summary>
    /// 使用Convert将常用的进行系统类型转换 比如注意一定要填写类型的包装类比如int要写成Int32,long必须写成Int64，大小写可以忽略 比如byte与Byte认为是相同的，但是必须是包装类
    /// 但是建议使用Convert方法中应有的类型
    /// </summary>
    public class ConvertCreaterParameter : ACreaterParameter {
        static readonly Type convertType = typeof(Convert);
        private string type, value;
        static Type stringType = typeof(string);
        public ConvertCreaterParameter(string name, string type, string value)
            : base(name) {
            this.type = type.Trim();
            this.value = value.Trim();
            if (type.ToLower().Equals("int"))
                this.type = "Int32";
            else if (type.ToLower().Equals("long"))
                this.type = "Int64";
            else if (type.ToLower().Equals("short"))
                this.type = "Int16";
            else if (type.ToLower().Equals("bool"))
                this.type = "Boolean";
        }

        public override object GetValue(ConfigManager config) {
            MethodInfo me = convertType.GetMethod("To" + type, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public, null, new Type[] { stringType }, null);
            if (me == null)
                throw new InvalidOperationException(string.Format("Conver.To{0} 方法没有找到！", type));
            else
                return me.Invoke(null, new object[] { value });
        }
    }

    /// <summary>
    /// 记录ConfigKey,MiddlerParameter以产生对应的对象
    /// </summary>
    public class ObjectCreaterParameter : ACreaterParameter {

        private MiddlerGetParameter para = null;
        public ObjectCreaterParameter(string name, MiddlerGetParameter para)
            : base(name) {
            this.para = para;
        }

        public override object GetValue(ConfigManager config) {
            return config.GetConfigValue(MiddlerConfig.NodeName, para);
        }
    }

    /// <summary>
    /// 用于返回该文件者本身
    /// </summary>
    public class ThisCreaterParameter : ACreaterParameter {
        private ConfigManager ma;
        public ThisCreaterParameter(ConfigManager cma) : base("This") { this.ma = cma; }
        public override object GetValue(ConfigManager config) {
            return this.ma;
        }
    }

    /// <summary>
    /// Config文件返回其他类型的值
    /// </summary>
    public class ConfigCreaterParameter : ACreaterParameter {

        private string config, key;
        public ConfigCreaterParameter(string name, string config, string key)
            : base(name) {
            this.config = config;
            this.key = key;
        }

        public override object GetValue(ConfigManager config) {
            return config.GetConfigValue(this.config, this.key);
        }
    }

    /// <summary>
    /// 微软默认App.config文件返回其他类型的值
    /// 暂时只能实现AppSettings和ConnectionStrings
    /// </summary>
    public class MSConfigCreaterParameter : ACreaterParameter {

        private string config, key;
        public MSConfigCreaterParameter(string name, string config, string key)
            : base(name) {
            this.config = config;
            this.key = key;
        }

        public override object GetValue(ConfigManager config) {
            if (this.config.ToLower().Equals("appsettings"))
                return Convert.ToString(System.Configuration.ConfigurationManager.AppSettings[this.key]);
            else if (this.config.ToLower().Equals("connectionstrings"))
                return Convert.ToString(System.Configuration.ConfigurationManager.ConnectionStrings[this.key]);
            else
                throw new InvalidOperationException(string.Format("此属性{0}暂时不支持！", this.config));
        }
    }

    /// <summary>
    /// 处理空对象参数
    /// <null name="" />
    /// </summary>
    public class NullCreaterParameter : ACreaterParameter {

        public NullCreaterParameter(string name) : base(name) { }

        public override object GetValue(ConfigManager config) {
            return null;
        }
    }
    /// <summary>
    /// 管理多参数设置比如SetP(a,b)
    /// </summary>
    public class ParamsCreaterParameter : ACreaterParameter {
        public static readonly string ParamsCreaterParameterKey = "GCL.Bean.Middler.ParamsCreaterParameterKey 20091022";
        private CreaterParameters paras;
        public ParamsCreaterParameter(GCL.IO.Config.ConfigManager ma, string name)
            : base(name) {
            paras = new CreaterParameters(ma);
        }
        public void AddParameter(ACreaterParameter para) {
            paras.AddParameter(para);
        }
        public override object GetValue(ConfigManager config) {
            //这里添加标记，用引用标示是否ParamsCreaterParameter的结果
            return new object[] { ParamsCreaterParameterKey, paras.GetParameters() };
        }
    }

    /// <summary>
    /// 处理数组参数
    /// </summary>
    public class ArrayCreaterParameter : ParamsCreaterParameter {

        private readonly Type BEANTOOLTYPE = typeof(BeanTool);
        private readonly Type OBJECTTYPE = typeof(object[]);
        private string type;
        public ArrayCreaterParameter(GCL.IO.Config.ConfigManager ma, string name, string type)
            : base(ma, name) {
            this.type = type;
            if (type.ToLower().Equals("int"))
                this.type = "Int32";
            else if (type.ToLower().Equals("long"))
                this.type = "Int64";
            else if (type.ToLower().Equals("short"))
                this.type = "Int16";
            else if (type.ToLower().Equals("bool"))
                this.type = "Boolean";
        }

        public override object GetValue(ConfigManager config) {
            MethodInfo me = BEANTOOLTYPE.GetMethod("To" + type, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public, null, new Type[] { OBJECTTYPE }, null);
            if (me == null)
                throw new InvalidOperationException(string.Format("BeanTool.To{0} 方法没有找到！", type));
            else
                return me.Invoke(null, new object[] { ((object[])base.GetValue(config))[1] });
        }
    }
}
