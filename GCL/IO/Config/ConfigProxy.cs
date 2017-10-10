using System;
using System.Collections;

namespace GCL.IO.Config {

    /// <summary>
    ///ConfigProxy������Ĭ�ϵ���ConfigManagerĳ��Config��ʵ�ֶԾ���Config�ݹ���ò���
    ///
    ///@author �ױ�
    ///@version 2.0.81212.1
    ///
    /// </summary>
    public class ConfigProxy : AConfig {

        private ConfigManager manager;

        private string configKey;

        /// <summary>
        ///@param allowCascade
        ///@param manager
        ///@param configKey
        /// </summary>
        public ConfigProxy(ConfigManager manager, string configKey) {
            this.manager = manager;
            this.configKey = configKey;
        }

        public override object GetValue(object key) {
            return this.manager.GetConfigValue(configKey, key);
        }

        public override void SetValue(object key, object value) {
            this.manager.SetConfigValue(configKey, key, value);
        }

        public override void Merge(AConfig config) {
            throw new NotImplementedException();
        }
    }
}