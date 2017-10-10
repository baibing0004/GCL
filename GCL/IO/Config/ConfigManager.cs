using GCL.Common;
using GCL.Event;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GCL.IO.Config {
    /// <summary>
    ///Config管理者负责实现对同一Config的递归调用的获取与设置。
    ///
    ///@author 白冰
    ///
    /// </summary>
    public class ConfigManager : IDisposable {

        private ConfigManager parent;

        /// <summary>
        ///@return parent
        /// </summary>
        protected ConfigManager GetParent() {
            return parent;
        }


        private AConfigResource resource;

        /// <summary>
        ///@return resource
        /// </summary>
        internal AConfigResource GetResource() {
            return resource;
        }

        private ConfigAdapter adapter = ConfigAdapter.GetInstance();

        private IDictionary<string, AConfig> configMap, proxyMap;

        internal IDictionary<string, AConfig> Configs {
            get { return configMap; }
            set { configMap = value; }
        }

        public ConfigManager(IDictionary<string, AConfig> configMap, ConfigManager parent,
                AConfigResource resource) {
            this.parent = parent;
            this.configMap = configMap;
            this.configMap.Clear();
            if (!Tool.IsEnable(this.parent)) {
                AConfig convertConfigDefault = new ClassConfig();
                convertConfigDefault.SetValue(ConfigAdapter.convertKey, typeof(ConfigConvert).FullName + ",");
                this.configMap[ConfigAdapter.convertKey] = convertConfigDefault;
            }

            this.proxyMap = new Dictionary<string, AConfig>();
            this.resource = resource;
            if (this.parent != null) this.parent.ConfigManagerFillEvent += new EventHandle(parent_ConfigManagerFillEvent);
            this.resource.TriggerEvent += new EventHandle(resource_TriggerEvent);
            this.ConfigManagerFillEvent += new EventHandle(EventArg._EventHandleDefault);
            this.adapter.Fill(this);
        }

        /// <summary>
        /// 特别处理在配置文件因为信息文件变更而自动刷新的情况下，不会影响已有的引用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void parent_ConfigManagerFillEvent(object sender, EventArg e) {
            lock (this) {
                this.parent = sender as ConfigManager;
            }
        }

        void resource_TriggerEvent(object sender, EventArg e) {
            lock (this) {
                this.proxyMap.Clear();
                this.adapter.Fill(this);
            }
            EventArg.CallEventSafely(this.ConfigManagerFillEvent, this, new EventArg());
        }

        /// <summary>
        /// 用于说明ConfigManager自动更新
        /// </summary>
        public event EventHandle ConfigManagerFillEvent;

        /// <summary>
        ///提供对应节点的代理实例
        ///
        ///@param key
        ///@return
        /// </summary>
        public AConfig GetConfig(string key) {
            lock (this.proxyMap) {
                if (!this.proxyMap.ContainsKey(key))
                    this.proxyMap[key] = new ConfigProxy(this, key);
            }
            return this.proxyMap[key];
        }


        /// <summary>
        ///提供对应节点 属性的递归调用式访问
        ///
        ///@param config
        ///@param key
        ///@return
        /// </summary>
        public object GetConfigValue(string config, object key) {
            this.resource.Taste();
            config = config.Trim();
            AConfig con = this.configMap.ContainsKey(config) ? this.configMap[config] : null;
            if (Tool.IsEnable(con)) {
                object value = con.GetValue(key);
                if (value != null)
                    return value;

            }
            if ((!Tool.IsEnable(con) || con.IsAllowCascade())
                    && Tool.IsEnable(this.parent))
                return this.parent.GetConfigValue(config, key);
            else
                return null;
        }

        /// <summary>
        ///提供对应节点 属性的递归调用式设置
        ///
        ///@param config
        ///@param key
        ///@param value
        ///@throws GCL.common.InvalidOperationException
        /// </summary>
        public void SetConfigValue(string config, object key, object value) {
            config = config.Trim();
            AConfig con = this.configMap.ContainsKey(config) ? this.configMap[config] : null;
            if (!Tool.IsEnable(con)) {
                if (Tool.IsEnable(this.parent))
                    this.parent.SetConfigValue(config, key, value);
                else
                    throw new InvalidOperationException(string.Format("没有找到可以设置的{0}节点", config));
            } else {
                if (con.IsAllowChangeValue()) {
                    con.SetValue(key, value);
                    con.SetChangeValue(true);
                    return;
                } else
                    throw new InvalidOperationException(string.Format("{0}节点不允许设置!", config));
            }

        }



        public virtual void Update() {
            bool isChangeValue = false;
            for (IEnumerator<KeyValuePair<string, AConfig>> enu = this.configMap.GetEnumerator(); enu.MoveNext(); isChangeValue = isChangeValue || enu.Current.Value.ChangeValue()) ;
            if (isChangeValue)
                this.adapter.Update(this);
            if (parent != null)
                parent.Update();
        }

        ~ConfigManager() {
            try {
                this.Dispose();
            } catch {
            }
        }

        #region IDisposable Members

        public virtual void Dispose() {
            //this.Update();
            this.resource.SetEnable(false);
            for (IEnumerator<KeyValuePair<string, AConfig>> enu = this.configMap.GetEnumerator(); enu.MoveNext(); ) {
                if (enu.Current.Value is IDisposable)
                    ((IDisposable)enu.Current.Value).Dispose();
            }
            this.configMap.Clear();
            this.proxyMap.Clear();
            this.resource.Dispose();
        }

        #endregion
    }
}
