using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using System.Xml;
namespace GCL.IO {
    /// <summary>
    /// 本类用于检验Xml是否合理
    /// </summary>
    public class XmlValidate {
        private ValidationType type;
        private string xml;
        /// <summary>
        /// 构造函数 
        /// </summary>
        /// <param name="xml">配置Xml文件路径或者Xml文件</param>
        /// <param name="type">检验类型</param>
        public XmlValidate(string path, ValidationType type) {
            this.xml = path;
            this.type = type;
        }

        private bool isValidate;
        public bool Validate() {
            isValidate = true;
            using (XmlValidatingReader v = new XmlValidatingReader(new XmlTextReader(xml))) {
                v.ValidationEventHandler += new ValidationEventHandler(v_ValidationEventHandler);
                while (v.Read()) ;
            }
            return isValidate;
        }

        /// <summary>
        /// 校验事件
        /// </summary>
        public event GCL.Event.EEventHandle<ValidationEventArgs> ValidationEvent;
        void v_ValidationEventHandler(object sender, ValidationEventArgs e) {
            isValidate = false;
            try {
                ValidationEvent(sender, e);
            } catch {
            }
        }
    }
}
