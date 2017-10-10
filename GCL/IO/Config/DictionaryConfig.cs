using System;
using System.Collections;
using System.Text;
using System.Xml;
namespace GCL.IO.Config {
    /// <summary>
    /// 提供默认格式的字典Config对象
    /// </summary>
    public class DictionaryConfig : AConfig, IDisposable {
        protected IDictionary idic;
        /// <summary>
        /// 是否自动忽略大小写
        /// </summary>
        private bool ignorecase = true;

        public DictionaryConfig(IDictionary idic, bool allowCascade, bool allowChangeValue, bool ignorecase)
            : base(allowCascade, allowChangeValue) {
            this.idic = idic;
            this.idic.Clear();
            this.ignorecase = ignorecase;
        }

        public DictionaryConfig(bool allowCascade, bool allowChangeValue, bool ignorecase) : this(new Hashtable(), allowCascade, allowChangeValue, ignorecase) { }
        public override object GetValue(object key) {
            if (ignorecase && key is string)
                return idic[key.ToString().ToLower()];
            else
                return idic[key];
        }

        public override void SetValue(object key, object value) {
            if (ignorecase && key is string)
                idic[key.ToString().ToLower()] = value;
            else
                idic[key] = value;
        }

        #region IDisposable Members

        public virtual void Dispose() {
            this.Clear();
        }

        #endregion

        ~DictionaryConfig() {
            this.Dispose();
        }

        public void Clear() {
            for (IDictionaryEnumerator ien = idic.GetEnumerator(); ien.MoveNext(); ) {
                this.CloseObject(ien.Value);
            } this.idic.Clear();
        }

        public IDictionaryEnumerator GetEnumerator() {
            return idic.GetEnumerator();
        }

        public override void Merge(AConfig config) {
            if (!(config is DictionaryConfig))
                throw new InvalidOperationException("不属于DictionaryConfig类!");
            DictionaryConfig _c = config as DictionaryConfig;
            //进行迁移，确保数据被移动后数据正常
            lock (this.idic) {
                //this.idic.Clear();
                for (IDictionaryEnumerator ien = _c.idic.GetEnumerator(); ien.MoveNext(); ) {
                    //20130509 白冰更新 不关闭或者删除原对象
                    /*
                    if (idic.Contains(ien.Key)) {
                        //this.CloseObject(idic[ien.Key]);
                    }*/
                    idic[ien.Key] = ien.Value;
                }
            }
            _c.idic.Clear();
            _c = null;
        }

        /// <summary>
        /// 用于关闭idic里面的值
        /// </summary>
        /// <param name="value"></param>
        protected void CloseObject(object value) {
            if (value is IDisposable)
                try {
                    ((IDisposable)value).Dispose();
                } catch {
                } else
                Bean.BeanTool.Close(value);
        }
    }

    /// <summary>
    /// 默认实现类似这样格式的解析器
    /// <add key="" value="">
    /// </summary>
    public abstract class ADictionaryConfigConvert : AConfigConvert {

        private string nodename = "";
        private string nodeName = "";
        /// <summary>
        /// 是否自动忽略大小写
        /// </summary>
        private bool ignorecase = true;
        /// <summary>
        /// 默认实现类似这样的解析器
        /// <add key="" value="">
        /// </summary>
        /// <param name="nodeName">节点名 比如（AppSettings,connectionStrings）</param>
        public ADictionaryConfigConvert(string nodeName, bool ignorecase) {
            this.nodename = nodeName.ToLower();
            this.nodeName = nodeName;
            this.ignorecase = ignorecase;
        }

        public abstract DictionaryConfig CreateInstance(bool ignorecase);
        /// <summary>
        /// 解析制定格式但节点名不同的Xml
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override AConfig ToConfig(System.Xml.XmlNode data) {
            AConfig config = CreateInstance(ignorecase);
            if (data.LocalName.ToLower().Trim().Equals(nodename)) {
                for (IEnumerator ie = data.ChildNodes.GetEnumerator(); ie.MoveNext(); ) {
                    XmlNode node = ie.Current as XmlNode;
                    if (node.LocalName.Trim().ToLower().Equals("add"))
                        if (ignorecase)
                            config.SetValue(node.Attributes["key"].InnerText.ToLower(), node.Attributes["value"].InnerText);
                        else
                            config.SetValue(node.Attributes["key"].InnerText, node.Attributes["value"].InnerText);
                }
            } else throw new InvalidOperationException(string.Format("错误的节点{0}！", data.Name));
            return config;
        }

        /// <summary>
        /// 重新生成制定格式节点名不同的Xml
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public override System.Xml.XmlNode ToXML(AConfig config) {
            DictionaryConfig aconfig = config as DictionaryConfig;
            XmlDocument doc = CreateEmptyXmlDocument(nodeName);
            lock (aconfig) {
                for (IDictionaryEnumerator iden = aconfig.GetEnumerator(); iden.MoveNext(); ) {
                    XmlElement node = doc.CreateElement("add");
                    node.SetAttribute("key", iden.Key.ToString());
                    node.SetAttribute("value", iden.Value.ToString());
                    doc.DocumentElement.AppendChild(node);
                }
            }
            return doc.DocumentElement;
        }
    }
}
