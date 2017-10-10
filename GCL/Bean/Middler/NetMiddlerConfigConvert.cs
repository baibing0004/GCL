using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace GCL.Bean.Middler {
    enum NetParameterEnum {
        MSConfig
    }
    public class NetMiddlerConfigConvert : MiddlerConfigConvert {

        #region 自动解析参数类型类型
        private static IDictionary CParams = new Hashtable();
        static NetMiddlerConfigConvert() {
            CParams[NetParameterEnum.MSConfig.ToString().ToLower()] = NetParameterEnum.MSConfig;
        }
        #endregion

        protected override ACreaterParameter CreateParameter(IDictionary<string, AObjectContainer> idic, System.Xml.XmlNode node, DefaultValues df, bool isNeedName) {
            if (CParams.Contains(node.LocalName.Trim().ToLower()))
                switch ((NetParameterEnum)CParams[node.LocalName.Trim().ToLower()]) {
                    case NetParameterEnum.MSConfig:
                        string[] name3 = GetAttribute(node, "ref").Split('/');
                        if (name3.Length < 2)
                            throw new Exception("MSConfig解析节点错误,ref属性中必须有config/key" + BeanTool.LineSeparator + node.OuterXml);
                        else
                            return new MSConfigCreaterParameter(GetAttribute(node, "name", isNeedName), name3[0], name3[1]);
                    default:
                        throw new Exception(string.Format("{0}解析节点错误,该属性没有对应的解析器\r\n{1}", node.LocalName.Trim().ToLower(), node.OuterXml));
                } else
                return base.CreateParameter(idic, node, df, isNeedName);

        }
    }
}
