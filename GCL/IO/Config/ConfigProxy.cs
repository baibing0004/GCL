using System;
using System.Collections;

namespace GCL.IO.Config {

    /// <summary>
    ///ConfigProxy代理负责默认调用ConfigManager某个Config来实现对具体Config递归调用操作
    ///
    ///@author 白冰
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