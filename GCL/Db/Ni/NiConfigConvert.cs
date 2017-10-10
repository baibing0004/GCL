using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GCL.IO.Config;

namespace GCL.Db.Ni {
    /// <summary>
    /// 泥框架配置文件解析器
    /// </summary>
    public class NiConfigConvert : AConfigConvert {

        private static Type staticCommandType = typeof(CommandType);
        public override AConfig ToConfig(System.Xml.XmlNode data) {
            AConfig config = new DictionaryConfig(true, false, true);
            List<ParameterEntity> entities = new List<ParameterEntity>();
            foreach (System.Xml.XmlNode node in data.ChildNodes) {
                if (node.NodeType != System.Xml.XmlNodeType.Element)
                    continue;
                if (node.ChildNodes.Count < 1)
                    throw new InvalidOperationException(string.Format("Command节点不能没有子节点存在!\r\n{0}", node.OuterXml));

                string content = null;
                foreach (System.Xml.XmlNode node2 in node.ChildNodes) {
                    if (node2.NodeType != System.Xml.XmlNodeType.Element)
                        continue;
                    if (node2.LocalName.Trim().ToLower().Equals("content")) {
                        if (string.IsNullOrEmpty(content))
                            content = node2.InnerText.Trim();
                        else
                            throw new InvalidOperationException(string.Format("Content节点不能出现多个!\r\n{0}", node2.OuterXml));
                    } else
                        entities.Add(CreateParameterEntity(node2));
                }
                config.SetValue(GetAttribute(node, "name"), new ParameterCommand(content, GetAttribute(node, "template", false), (CommandType)staticCommandType.GetField(GetAttribute(node, "type", "StoredProcedure"), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.IgnoreCase).GetValue(null), entities.ToArray()));
                entities.Clear();
            }
            return config;
        }

        private static Type dirStaticType = typeof(ParameterDirection);
        /// <summary>
        /// 转换Xml节点成为ParameterEntity对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected virtual ParameterEntity CreateParameterEntity(System.Xml.XmlNode node) {
            ParameterEntity entity = new ParameterEntity();
            entity.ParameterName = GetAttribute(node, "name");
            //if (!entity.ParameterName.StartsWith("@"))
            //    entity.ParameterName = "@" + entity.ParameterName;
            entity.Nullable = Convert.ToBoolean(GetAttribute(node, "Nullable", "false"));
            entity.ParameterDirection = (ParameterDirection)dirStaticType.GetField(GetAttribute(node, "ParameterDirection", "Input"), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.IgnoreCase).GetValue(null);
            entity.Size = Convert.ToInt32(GetAttribute(node, "Size", "0"));
            entity.DBTypeName = node.LocalName;
            if (!string.IsNullOrEmpty(GetAttribute(node, "DefaultValue", false)))
                entity.DefaultValue = GetAttribute(node, "DefaultValue");
            return entity;
        }

        public override System.Xml.XmlNode ToXML(AConfig config) {
            throw new NotImplementedException();
        }
    }
}
