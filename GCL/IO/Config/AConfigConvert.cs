using GCL.IO;
using System;
using System.Xml;
using System.Collections;

namespace GCL.IO.Config {
    /// <summary>
    ///����������
    ///
    ///@author �ױ�
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
        /// ��Ҫ���ڶ�ȡname����
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key"></param>
        /// <param name="isNeedName"></param>
        /// <returns></returns>
        protected string GetAttribute(XmlNode node, string key, bool isNeedName) {
            return GetAttribute(node, key, isNeedName ? null : "");
        }

        /// <summary>
        /// ���ر�������ֵ
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetAttribute(XmlNode node, string key) {
            return GetAttribute(node, key, null);
        }

        /// <summary>
        /// ��ȡNode����key��������Բ�������ôʹ�÷���Ĭ��ֵ���������ֵ����Ϊnull����ô��Ϊ�����Ա���
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
                    throw new InvalidOperationException(string.Format("{0}�ڵ�{1}���Բ���Ϊ��\r\n{2}", node.LocalName, key, node.OuterXml));
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
                        throw new InvalidOperationException("����ʧ�ܣ��˽ڵ����ݷ�ConfigConvert��\r\n" + node.InnerXml);
                    else
                        config.SetValue(node.Attributes["name"].InnerText, node.Attributes["type"].InnerText + "," + node.Attributes["dll"].InnerText);
                }
            } else
                throw new InvalidOperationException("����ʧ�ܣ��˽ڵ����ݷ�" + ConfigAdapter.convertKey + "��");

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
