using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.IO.Config {
    public class ConfigManagement {
        public static AConfig GetConfig(string key) {
            return ConfigManagerFactory.GetApplicationConfigManager().GetConfig(key);
        }
        /// <summary>
        /// 获取AppSettings里面的数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AppSettings(string key) {
            return Convert.ToString(GetConfig("AppSettings").GetValue(key));
        }

        /// <summary>
        /// 设置AppSettings里面的数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void SetAppSettings(string key, string v) {
            GetConfig("AppSettings").SetValue(key, v);
        }

        /// <summary>
        /// 获取ConnectionStrings里面的数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ConnectionStrings(string key) {
            return Convert.ToString(GetConfig("ConnectionStrings").GetValue(key));
        }

        /// <summary>
        /// 设置ConnectionStrings里面的数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void SetConnectionStrings(string key, string v) {
            GetConfig("ConnectionStrings").SetValue(key, v);
        }

        /// <summary>
        /// 获取AppSettings里面的数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AppSettings(ConfigManager cm, string key) {
            return Convert.ToString(cm.GetConfig("AppSettings").GetValue(key));
        }

        /// <summary>
        /// 设置AppSettings里面的数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void SetAppSettings(ConfigManager cm, string key, string v) {
            GetConfig("AppSettings").SetValue(key, v);
        }

        /// <summary>
        /// 获取ConnectionStrings里面的数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ConnectionStrings(ConfigManager cm, string key) {
            return Convert.ToString(cm.GetConfig("ConnectionStrings").GetValue(key));
        }

        /// <summary>
        /// 设置ConnectionStrings里面的数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void SetConnectionStrings(ConfigManager cm, string key, string v) {
            cm.GetConfig("ConnectionStrings").SetValue(key, v);
        }


        /// <summary>
        /// 删除ConfigManager
        /// </summary>
        public static void Dispose() {
            ConfigManagerFactory.GetApplicationConfigManager().Dispose();
        }
    }
}
