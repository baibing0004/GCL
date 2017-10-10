using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GCL.IO.Config {

    /// <summary>
    ///�����������Manager������Դ�ʹ�����Դ��Manager�ľ���ת����
    ///
    ///@author �ױ�
    ///@version 2.0.81212.1
    ///
    /// </summary>
    public class ConfigAdapter {

        static ConfigAdapter adapter = null;

        public static ConfigAdapter GetInstance() {
            if (adapter == null)
                adapter = new ConfigAdapter();
            return adapter;
        }
        internal static string convertKey = "ConfigConverts";
        public void Fill(ConfigManager manager) {
            manager.GetResource().SetEnable(false);
            string data = manager.GetResource().Load();
            XmlDocument doc = new XmlDocument();
            try {
                doc.LoadXml(data);
            } catch {
                doc.LoadXml(string.Format("<Config>{0}</Config>", data));
            }
            if (doc.DocumentElement.LocalName.ToLower().Equals("config")) {
                for (IEnumerator enu = doc.DocumentElement.ChildNodes.GetEnumerator(); enu.MoveNext(); ) {
                    XmlNode node = enu.Current as XmlNode;
                    string nodeName = node.LocalName.Trim();
                    //��ȡAConfigConvert ������
                    AConfigConvert convert = manager.GetConfig(convertKey).GetValue(nodeName) as AConfigConvert;
                    if (convert == null)
                        throw new InvalidOperationException(string.Format("{0}�ڵ�Ľ����������ڣ�", node.LocalName));
                    else {
                        if (convert is INeedConfigManager)
                            ((INeedConfigManager)convert).SetConfigManager(manager);
                        lock (manager.Configs) {
                            if (manager.Configs.ContainsKey(nodeName))
                                // && manager.Configs[nodeName] is IDisposable
                                //((IDisposable)manager.Configs[node.LocalName]).Dispose();
                                manager.Configs[nodeName].Merge(convert.ToConfig(node));
                            else
                                manager.Configs[nodeName] = convert.ToConfig(node);
                        }
                    }
                }
            } else {
                string nodeName = doc.DocumentElement.LocalName.Trim();
                AConfigConvert convert = manager.GetConfig(convertKey).GetValue(nodeName) as AConfigConvert;
                if (convert == null)
                    throw new InvalidOperationException(string.Format("{0}�ڵ�Ľ����������ڣ�", doc.DocumentElement.LocalName));

                if (convert is INeedConfigManager)
                    ((INeedConfigManager)convert).SetConfigManager(manager);

                lock (manager.Configs) {
                    if (manager.Configs.ContainsKey(nodeName))
                        //&& manager.Configs[doc.DocumentElement.LocalName] is IDisposable)
                        //((IDisposable)manager.Configs[doc.DocumentElement.LocalName]).Dispose();
                        manager.Configs[nodeName].Merge(convert.ToConfig(doc.DocumentElement));
                    else
                        manager.Configs[nodeName] = convert.ToConfig(doc.DocumentElement);
                }
            }

            manager.GetResource().ReSet();
            manager.GetResource().SetEnable(true);
        }

        public void Update(ConfigManager manager) {
            manager.GetResource().SetEnable(false);
            KeyValuePair<string, AConfig>[] arrays;
            lock (manager.Configs) {
                arrays = new KeyValuePair<string, AConfig>[manager.Configs.Count];
                manager.Configs.CopyTo(arrays, 0);
            }
            try {
                StringBuilder sb = new StringBuilder();
                sb.Append("<Config>");
                foreach (KeyValuePair<string, AConfig> cur in arrays) {
                    AConfigConvert convert = manager.GetConfig(convertKey).GetValue(cur.Key) as AConfigConvert;
                    if (convert == null)
                        throw new InvalidOperationException(string.Format("{0}�ڵ�Ľ����������ڣ�", cur.Key));

                    if (convert is INeedConfigManager)
                        ((INeedConfigManager)convert).SetConfigManager(manager);

                    sb.Append(convert.ToString(cur.Value));
                }
                sb.Append("</Config>");
                manager.GetResource().Save(sb.ToString());
            } catch (NotImplementedException) {
            }
            manager.GetResource().ReSet();
            manager.GetResource().SetEnable(true);
        }
    }
}
