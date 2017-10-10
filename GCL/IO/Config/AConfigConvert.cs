using GCL.IO;
using System;
using System.Xml;
using System.Collections;

namespace GCL.IO.Config {
    /// <summary>
    ///配置生成器
    ///
    ///@author 白冰
    ///
    /// </summary>
    public abstract class AConfigConvert {
        public AConfig ToConfig(string data) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            return this.ToConfig(doc.DocumentElement);
        }

        public abstract AConfig ToConfig(XmlNode data);

        public string ToString(AConfig config) {
            return this.ToXML(config).OuterXml;
        }

        public abstract XmlNode ToXML(AConfig config);

        System.Text.Encoding encoding = IOTool.DefaultEncoding;

        public System.Text.Encoding Encoding {
            get { return encoding; }
            set { encoding = value; }
        }

        /// <summary>
        /// 主要用于读取name属性
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key"></param>
        /// <param name="isNeedName"></param>
        /// <returns></returns>
        protected string GetAttribute(XmlNode node, string key, bool isNeedName) {
            return GetAttribute(node, key, isNeedName ? null : "");
        }

        /// <summary>
        /// 返回必添属性值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetAttribute(XmlNode node, string key) {
            return GetAttribute(node, key, null);
        }

        /// <summary>
        /// 获取Node属性key，如果属性不存在那么使用返回默认值，如果返回值设置为null，那么认为该属性必添
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key"></param>
        /// <param name="defaultvalue"></param>
        /// <returns></returns>
        protected string GetAttribute(XmlNode node, string key, string defaultvalue) {
            if (defaultvalue != null)
                return (node.Attributes[key] != null ? node.Attributes[key].InnerText : defaultvalue);
            else
                if (node.Attributes[key] == null)
                    throw new InvalidOperationException(string.Format("{0}节点{1}属性不能为空\r\n{2}", node.LocalName, key, node.OuterXml));
                else
                    return node.Attributes[key].InnerText;
        }

        protected XmlDocument CreateEmptyXmlDocument(string root) {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", IOTool.IsEnable(this.Encoding) ? Encoding.EncodingName : IOTool.DefaultEncoding.EncodingName, "yes"));
            doc.LoadXml("<" + root + "></" + root + ">");
            return doc;
        }
    }

    public class ConfigConvert : AConfigConvert {
        public override AConfig ToConfig(XmlNode data) {
            ClassConfig config = new ClassConfig();
            if (data.LocalName.Trim().ToLower().Equals(ConfigAdapter.convertKey.ToLower())) {
                for (IEnumerator enu = data.ChildNodes.GetEnumerator(); enu.MoveNext(); ) {
                    XmlNode node = enu.Current as XmlNode;
                    if (!node.LocalName.Trim().ToLower().Equals("configconvert"))
                        throw new InvalidOperationException("解析失败，此节点内容非ConfigConvert！\r\n" + node.InnerXml);
                    else
                        config.SetValue(node.Attributes["name"].InnerText, node.Attributes["type"].InnerText + "," + node.Attributes["dll"].InnerText);
                }
            } else
                throw new InvalidOperationException("解析失败，此节点内容非" + ConfigAdapter.convertKey + "！");

            return config;
        }

        public override XmlNode ToXML(AConfig config) {
            XmlDocument doc = CreateEmptyXmlDocument(ConfigAdapter.convertKey);
            XmlElement root = doc.DocumentElement;
            for (IDictionaryEnumerator ienum = ((ClassConfig)config).GetDictionary().GetEnumerator(); ienum.MoveNext(); ) {
                XmlElement _mode = doc.CreateElement("ConfigConvert");
                _mode.SetAttribute("name", IOTool.ToStringValue(ienum.Key));

                string[] values = ienum.Value.ToString().Split(',');
                _mode.SetAttribute("type", IOTool.ToStringValue(values[0]));
                _mode.SetAttribute("dll", IOTool.ToStringValue(values[1]));
                root.AppendChild(_mode);
            }
            return doc.DocumentElement;
        }
    }

    class ClassConfig : DictionaryConfig {
        public ClassConfig(IDictionary idic) : base(idic, true, false, false) { }
        public ClassConfig() : base(true, false, false) { }

        public IDictionary GetDictionary() {
            return idic;
        }
        public override object GetValue(object key) {
            if (!IOTool.IsEnable(base.GetValue(key)))
                return null;
            string[] values = base.GetValue(key).ToString().Split(',');
            return Bean.BeanTool.CreateInstance(values[1], values[0]);
        }
    }
}
