using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using GCL.IO.Config;
namespace GCL.Bean.Middler {
    /// <summary>
    /// 中介者解析器
    /// </summary>
    public abstract class AMiddlerConfigConvert : AConfigConvert, INeedConfigManager {

        protected ConfigManager ma;
        /// <summary>
        /// 通过将XML解析为中介者Config
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override AConfig ToConfig(XmlNode data) {
            IDictionary<string, IDictionary<string, AObjectContainer>> idic = CreateTopDictionary();

            foreach (XmlNode node in data.ChildNodes) {
                //处理App
                //XmlNode node = ienum.Current as XmlNode;
                if (node.NodeType != XmlNodeType.Element || !node.LocalName.ToLower().Equals("app"))
                    continue;
                IDictionary<string, AObjectContainer> appdic = CreateAppDictionary();
                idic[node.Attributes["name"].InnerText.ToLower().Trim()] = appdic;
                DefaultValues df = CreateDefaultValues(node);
                foreach (XmlNode objectNode in node.ChildNodes) {
                    //处理具体的Container
                    //XmlNode objectNode = ienum2.Current as XmlNode;
                    if (objectNode.NodeType == XmlNodeType.Element)
                        appdic[objectNode.Attributes["name"].InnerText.ToLower().Trim()] = CreateContainer(appdic, objectNode, df);
                }
            }
            return new MiddlerConfig(idic);
        }

        #region 相关方法

        /// <summary>
        /// 用于产生默认值对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected abstract DefaultValues CreateDefaultValues(XmlNode node);
        /// <summary>
        /// 返回整体使用的字典对象
        /// </summary>
        /// <returns></returns>
        protected virtual IDictionary<string, IDictionary<string, AObjectContainer>> CreateTopDictionary() {
            return new Dictionary<string, IDictionary<string, AObjectContainer>>();
        }

        /// <summary>
        /// 返回App使用的字典对象
        /// </summary>
        /// <returns></returns>
        protected virtual IDictionary<string, AObjectContainer> CreateAppDictionary() {
            return new Dictionary<string, AObjectContainer>();
        }

        protected string GetAttributePackage(string text, DefaultValues df) {
            return (!string.IsNullOrEmpty(df.Package) && text.StartsWith(".")) ? df.Package + text : text;
        }

        /// <summary>
        /// 处理嵌套配置信息以返回容器
        /// </summary>
        /// <param name="idic"></param>
        /// <param name="node"></param>
        /// <param name="df"></param>
        /// <returns></returns>
        protected virtual AObjectContainer CreateContainer(IDictionary<string, AObjectContainer> idic, XmlNode node, DefaultValues df) {
            //处理object与objects
            if (node.LocalName.ToLower().Trim().Equals("object")) {
                CreaterParameters paras = new CreaterParameters(this.ma);
                string method = GetAttribute(node, "method", df.Method);
                string mode = GetAttribute(node, "mode", df.Mode);
                for (IEnumerator ie = node.ChildNodes.GetEnumerator(); ie.MoveNext(); ) {
                    XmlNode node2 = ie.Current as XmlNode;
                    if (node2.NodeType == XmlNodeType.Element) {
                        DefaultValues df2 = df.Clone() as DefaultValues;
                        //df2.SetValue(node);
                        //20100814 白冰 修改只有package dll严格传递
                        if (BeanTool.IsEnable(node.Attributes["dll"]))
                            df2.Dll = node.Attributes["dll"].InnerText;
                        if (BeanTool.IsEnable(node.Attributes["package"]))
                            df2.Package = node.Attributes["package"].InnerText.TrimEnd('.');


                        paras.AddParameter(CreateParameter(idic, node2, df2, GetIsNeedName(method)));
                    }
                }
                return CreateContainer(node, df, mode, CreateCreater(node, df, method, paras));
            } else if (node.LocalName.ToLower().Trim().Equals("objects")) {
                #region 递归调用进行但是必须都是object类型
                //这是一个Objects类型的对象
                DefaultValues df2 = df.Clone() as DefaultValues;
                df2.SetValue(node);
                ArrayObjectContainer container = new ArrayObjectContainer(this.ma, GetAttribute(node, "dll", df2.Dll), GetAttributePackage(GetAttribute(node, "convert", false), df2));
                for (IEnumerator ie = node.ChildNodes.GetEnumerator(); ie.MoveNext(); ) {
                    XmlNode node2 = ie.Current as XmlNode;
                    if (node2.NodeType == XmlNodeType.Element)
                        if (node2.LocalName.Trim().ToLower().Equals("object"))
                            container.AddParameter(CreateParameter(idic, node2, df2, false));
                        else
                            throw new Exception("objects解析节点错误" + node2.InnerXml);
                }
                return container;
                #endregion
            } else throw new Exception("App解析节点错误" + node.InnerXml);
        }

        #endregion

        #region 虚方法

        /// <summary>
        /// 创建具体构造器
        /// </summary>
        /// <param name="node"></param>
        /// <param name="df"></param>
        /// <param name="method"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        protected abstract AObjectCreater CreateCreater(XmlNode node, DefaultValues df, string method, CreaterParameters paras);

        /// <summary>
        /// 创建具体构造容器
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mode"></param>
        /// <param name="creater"></param>
        /// <returns></returns>
        protected abstract AObjectContainer CreateContainer(XmlNode node, DefaultValues df, string mode, AObjectCreater creater);

        /// <summary>
        /// 用于判断此方法参数是否必须有name属性一般认为Bean方法下参数必须有name属性
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        protected abstract bool GetIsNeedName(string method);

        /// <summary>
        /// 处理嵌套参数
        /// </summary>
        /// <param name="idic"></param>
        /// <param name="node"></param>
        /// <param name="df"></param>
        /// <param name="isNeedName"></param>
        /// <returns></returns>
        protected abstract ACreaterParameter CreateParameter(IDictionary<string, AObjectContainer> idic, XmlNode node, DefaultValues df, bool isNeedName);

        #endregion

        /// <summary>
        /// 将Config返转为XML文件
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public override XmlNode ToXML(AConfig config) {
            throw new NotImplementedException("Middler不可写!");
        }

        #region INeedConfigManager Members

        public void SetConfigManager(ConfigManager config) {
            this.ma = config;
        }

        #endregion
    }

    /// <summary>
    /// 用于记录默认值
    /// </summary>
    public class DefaultValues : ICloneable {
        private string dll = "";

        public string Dll {
            get { return dll; }
            set { dll = value; }
        }
        private string method = CreateMethod.Bean.ToString();

        public string Method {
            get { return method; }
            set { method = value; }
        }
        private string mode = CreateMode.Static.ToString();

        public string Mode {
            get { return mode; }
            set { mode = value; }
        }

        private int size = 50, wait = 60000, timeout = 30000;

        public int PoolWaitTime {
            get { return wait; }
            set { wait = value; }
        }

        public int PoolSize {
            get { return size; }
            set { size = value; }
        }

        public int PoolTimeOut {
            get { return timeout; }
            set { timeout = value; }
        }


        private string app = "";

        public string App {
            get { return app; }
            set { app = value; }
        }

        private string package = "";
        public string Package {
            get { return package; }
            set { package = value; }
        }


        public DefaultValues(string app, string dll, string method, string mode, string package) {
            this.app = app;
            this.dll = dll;
            this.method = method;
            this.mode = mode;
            this.package = package;
        }

        public DefaultValues(XmlNode node) {
            SetValue(node);
        }

        public virtual void SetValue(XmlNode node) {
            //只有当为App节点时设置此值
            if (node.LocalName.Trim().ToLower().Equals("app") && BeanTool.IsEnable(node.Attributes["name"]))
                this.app = node.Attributes["name"].InnerText;
            if (BeanTool.IsEnable(node.Attributes["dll"]))
                this.dll = node.Attributes["dll"].InnerText;
            if (BeanTool.IsEnable(node.Attributes["method"]))
                this.method = node.Attributes["method"].InnerText;
            if (BeanTool.IsEnable(node.Attributes["package"]))
                this.package = node.Attributes["package"].InnerText.TrimEnd('.');
            if (BeanTool.IsEnable(node.Attributes["mode"])) {
                this.mode = node.Attributes["mode"].InnerText;
                this.size = Convert.ToInt32((node.Attributes["size"] != null ? node.Attributes["size"].InnerText : this.size.ToString()));
                this.wait = Convert.ToInt32((node.Attributes["wait"] != null ? node.Attributes["wait"].InnerText : this.wait.ToString()));
                this.timeout = Convert.ToInt32((node.Attributes["timeout"] != null ? node.Attributes["timeout"].InnerText : this.timeout.ToString()));
            }
        }

        #region 自动解析String类型
        private static IDictionary CMethod = new Hashtable();
        private static IDictionary CMode = new Hashtable();
        static DefaultValues() {
            CMethod[CreateMethod.Bean.ToString().ToLower()] = CreateMethod.Bean;
            CMethod[CreateMethod.Constructor.ToString().ToLower()] = CreateMethod.Constructor;
            CMethod[CreateMethod.Factory.ToString().ToLower()] = CreateMethod.Factory;
            CMethod[CreateMethod.ConstructorBean.ToString().ToLower()] = CreateMethod.ConstructorBean;
            CMethod[CreateMethod.FactoryBean.ToString().ToLower()] = CreateMethod.FactoryBean;
            CMode[CreateMode.Instance.ToString().ToLower()] = CreateMode.Instance;
            CMode[CreateMode.Pool.ToString().ToLower()] = CreateMode.Pool;
            CMode[CreateMode.Static.ToString().ToLower()] = CreateMode.Static;
        }
        public static CreateMethod ParseCreateMethod(string me) {
            me = me.Trim().ToLower();
            if (!CMethod.Contains(me))
                throw new Exception(me + "不是CreateMethod的类型之一");
            else
                return (CreateMethod)CMethod[me];
        }

        public static CreateMode ParseCreateMode(string mo) {
            mo = mo.Trim().ToLower();
            if (!CMode.Contains(mo))
                throw new Exception(mo + "不是CreateMode的类型之一");
            else
                return (CreateMode)CMode[mo];
        }
        #endregion

        #region ICloneable Members

        public object Clone() {
            return MemberwiseClone();
        }

        #endregion
    }

    public enum ParameterEnum {
        Object,
        Objects,
        Array,
        Params,
        Config,
        Enum,
        Null,
        This
    }

    /// <summary>
    /// 标准的中介者解析器 允许用户扩展其自定义属性
    /// </summary>
    public class MiddlerConfigConvert : AMiddlerConfigConvert {

        /// <summary>
        /// 使用默认的对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override DefaultValues CreateDefaultValues(XmlNode node) {
            return new DefaultValues(node);
        }

        /// <summary>
        /// 创建具体构造容器
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mode"></param>
        /// <param name="creater"></param>
        /// <returns></returns>
        protected override AObjectContainer CreateContainer(XmlNode node, DefaultValues df, string mode, AObjectCreater creater) {
            CreateMode mode1 = DefaultValues.ParseCreateMode(mode);
            switch (mode1) {
                case CreateMode.Static:
                    return new StaticObjectContainer(creater);
                case CreateMode.Instance:
                    return new InstanceObjectContainer(creater);
                case CreateMode.Pool:
                    return new PoolObjectContainer(Convert.ToInt32(GetAttribute(node, "size", df.PoolSize.ToString())), Convert.ToInt32(GetAttribute(node, "wait", df.PoolWaitTime.ToString())), Convert.ToInt32(GetAttribute(node, "timeout", df.PoolTimeOut.ToString())), creater);
                default:
                    throw new Exception("异常构造模式" + mode);
            }
        }

        private string GetAttributeType(XmlNode node, DefaultValues df) {
            return GetAttributePackage(GetAttribute(node, "type"), df);
        }

        /// <summary>
        /// 创建具体构造器
        /// </summary>
        /// <param name="node"></param>
        /// <param name="df"></param>
        /// <param name="method"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        protected override AObjectCreater CreateCreater(XmlNode node, DefaultValues df, string method, CreaterParameters paras) {
            CreateMethod method1 = DefaultValues.ParseCreateMethod(method);
            switch (method1) {
                case CreateMethod.Bean:
                    return new BeanCreater(GetAttribute(node, "dll", df.Dll), GetAttributeType(node, df), paras);
                case CreateMethod.Constructor:
                    return new ConstructorCreater(GetAttribute(node, "dll", df.Dll), GetAttributeType(node, df), paras);
                case CreateMethod.Factory:
                    return new FactoryCreater(GetAttribute(node, "dll", df.Dll), GetAttributeType(node, df), paras);
                case CreateMethod.ConstructorBean:
                    return new ConstructorBeanCreater(GetAttribute(node, "dll", df.Dll), GetAttributeType(node, df), Convert.ToInt32(GetAttribute(node, "constructorparalength")), paras);
                case CreateMethod.FactoryBean:
                    return new FactoryBeanCreater(GetAttribute(node, "dll", df.Dll), GetAttributeType(node, df), Convert.ToInt32(GetAttribute(node, "constructorparalength")), paras);
                default:
                    throw new Exception("异常构造类型" + method);
            }
        }


        #region 自动解析参数类型类型
        private static IDictionary CArray = new Hashtable();
        static MiddlerConfigConvert() {
            CArray[ParameterEnum.Object.ToString().ToLower()] = ParameterEnum.Object;
            CArray[ParameterEnum.Objects.ToString().ToLower()] = ParameterEnum.Objects;
            CArray[ParameterEnum.Array.ToString().ToLower()] = ParameterEnum.Array;
            CArray[ParameterEnum.Params.ToString().ToLower()] = ParameterEnum.Params;
            CArray[ParameterEnum.Config.ToString().ToLower()] = ParameterEnum.Config;
            CArray[ParameterEnum.Enum.ToString().ToLower()] = ParameterEnum.Enum;
            CArray[ParameterEnum.Null.ToString().ToLower()] = ParameterEnum.Null;
            CArray[ParameterEnum.This.ToString().ToLower()] = ParameterEnum.This;
        }
        #endregion

        static int num = 0;
        ///返回临时文件
        protected virtual string GetTempObjectName() {
            return "_" + (num++).GetHashCode() + DateTime.Now.Ticks;
        }

        /// <summary>
        /// 处理嵌套参数
        /// </summary>
        /// <param name="idic"></param>
        /// <param name="node"></param>
        /// <param name="df"></param>
        /// <param name="isNeedName"></param>
        /// <returns></returns>
        protected override ACreaterParameter CreateParameter(IDictionary<string, AObjectContainer> idic, XmlNode node, DefaultValues df, bool isNeedName) {

            //参数有5种类型 object,objects,config,null,convert 其中默认为convert类型
            if (CArray.Contains(node.LocalName.Trim().ToLower())) {
                switch ((ParameterEnum)CArray[node.LocalName.Trim().ToLower()]) {
                    case ParameterEnum.This:
                        return new ThisCreaterParameter(this.ma);
                    case ParameterEnum.Object:
                    case ParameterEnum.Objects:
                        if (BeanTool.IsEnable(node.Attributes["ref"])) {
                            string[] name = GetAttribute(node, "ref").TrimStart('/').Split('/');
                            if (name.Length < 2)
                                return new ObjectCreaterParameter(GetAttribute(node, "name", isNeedName), new NameMiddlerGetParameter(df.App, name[0]));
                            else
                                return new ObjectCreaterParameter(GetAttribute(node, "name", isNeedName), new NameMiddlerGetParameter(name[0], name[1]));
                        } else {
                            string name = GetTempObjectName().Trim().ToLower();
                            idic[name] = CreateContainer(idic, node, df);
                            return new ObjectCreaterParameter(GetAttribute(node, "name", isNeedName), new NameMiddlerGetParameter(df.App, name));
                        }
                    case ParameterEnum.Config:
                        string[] name2 = GetAttribute(node, "ref").Split('/');
                        if (name2.Length < 2)
                            throw new Exception("Config解析节点错误,ref属性中必须有config/key" + BeanTool.LineSeparator + node.InnerXml);
                        else
                            return new ConfigCreaterParameter(GetAttribute(node, "name", isNeedName), name2[0], name2[1]);
                    case ParameterEnum.Null:
                        return new NullCreaterParameter(GetAttribute(node, "name", isNeedName));
                    case ParameterEnum.Enum:
                        return new EnumCreaterParameter(GetAttribute(node, "name", isNeedName), GetAttribute(node, "dll", df.Dll), GetAttributePackage(GetAttribute(node, "value", node.InnerText), df));
                    case ParameterEnum.Params:
                    case ParameterEnum.Array:
                        ParamsCreaterParameter paras;
                        if ((ParameterEnum)CArray[node.LocalName.Trim().ToLower()] == ParameterEnum.Params)
                            paras = new ParamsCreaterParameter(this.ma, GetAttribute(node, "name", isNeedName));
                        else
                            paras = new ArrayCreaterParameter(this.ma, GetAttribute(node, "name", isNeedName), GetAttribute(node, "type"));
                        for (IEnumerator ie = node.ChildNodes.GetEnumerator(); ie.MoveNext(); ) {
                            XmlNode node2 = ie.Current as XmlNode;
                            if (node2.NodeType == XmlNodeType.Element)
                                paras.AddParameter(CreateParameter(idic, node2, df, false));
                        }
                        return paras;
                    default:
                        throw new Exception("不可能错误!" + node.InnerXml);
                }
            } else {
                //默认是Convert参数
                return new ConvertCreaterParameter(GetAttribute(node, "name", isNeedName), node.LocalName, GetAttribute(node, "value", node.InnerText));
            }
        }

        /// <summary>
        /// 用于判断此方法参数是否必须有name属性一般认为Bean方法下参数必须有name属性
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        protected override bool GetIsNeedName(string method) {
            CreateMethod method1 = DefaultValues.ParseCreateMethod(method);
            switch (method1) {
                case CreateMethod.Bean:
                case CreateMethod.ConstructorBean:
                case CreateMethod.FactoryBean:
                    return true;
                default:
                    return false;
            }
        }
    }
}
