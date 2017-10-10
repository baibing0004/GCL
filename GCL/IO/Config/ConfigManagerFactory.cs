using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
namespace GCL.IO.Config {
    public class ConfigManagerFactory {
        /// <summary>
        /// 用于按照文件生成ConfigManager对象
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="file"></param>
        /// <param name="encoding"></param>
        /// <param name="limit"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static ConfigManager GetConfigManagerFromFile(ConfigManager manager, FileInfo file, Encoding encoding, int limit, TimeSpan timeSpan) {
            FileConfigResource resource = new FileConfigResource(file, limit, timeSpan);
            resource.Encoding = encoding;
            return new ConfigManager(new Dictionary<string, AConfig>(), manager, resource);
        }

        /// <summary>
        /// 用于按照文件生成ConfigManager对象
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="file"></param>
        /// <param name="encoding"></param>
        /// <param name="limit">500</param>
        /// <param name="timeSpan">1分钟</param>
        /// <returns></returns>
        public static ConfigManager GetConfigManagerFromFile(ConfigManager manager, FileInfo file, Encoding encoding) {

#if DEBUG
            return GetConfigManagerFromFile(manager, file, encoding, 0, TimeSpan.FromMinutes(1));
#else
            return GetConfigManagerFromFile(manager, file, encoding, 500, TimeSpan.FromMinutes(1));
#endif

        }

        /// <summary>
        /// 用于按照文件生成ConfigManager对象
        /// </summary>
        /// <param name="manager">可以填写为空</param>
        /// <param name="file"></param>
        /// <param name="encoding">系统字符集</param>
        /// <returns></returns>
        public static ConfigManager GetConfigManagerFromFile(ConfigManager manager, FileInfo file) {
            return GetConfigManagerFromFile(manager, file, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 用于按照文件生成ConfigManager对象
        /// </summary>
        /// <param name="manager">可以填写为空</param>
        /// <param name="file"></param>
        /// <param name="encoding">系统字符集</param>
        /// <returns></returns>
        public static ConfigManager GetConfigManagerFromFile(ConfigManager manager, string file) {
            return GetConfigManagerFromFile(manager, new FileInfo(file));
        }

        /// <summary>
        /// 用于按照文件夹（包含子文件夹）生成ConfigManager组对象
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="file"></param>
        /// <param name="encoding"></param>
        /// <param name="limit"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static IDictionary<string, ConfigManager> GetConfigManagerFromDirectorys(IDictionary<string, ConfigManager> pair, ConfigManager manager, string filter, Encoding encoding, int limit, TimeSpan timeSpan) {
            DirectoryInfo dinfo = new DirectoryInfo(IOTool.GetPath(filter));
            if (pair == null)
                pair = new Dictionary<string, ConfigManager>();
            pair[dinfo.FullName] = GetConfigManagerFromDirectory(manager, filter, encoding, limit, timeSpan);
            foreach (DirectoryInfo dic in dinfo.GetDirectories())
                GetConfigManagerFromDirectorys(pair, pair[dinfo.FullName], dic.FullName + IOTool.GetFileName(filter), encoding, limit, timeSpan);
            return pair;
        }

        /// <summary>
        /// 用于按照文件夹生成ConfigManager对象
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="file"></param>
        /// <param name="encoding"></param>
        /// <param name="limit"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static ConfigManager GetConfigManagerFromDirectory(ConfigManager manager, string filter, Encoding encoding, int limit, TimeSpan timeSpan) {
            DirectoryConfigResource resource = new DirectoryConfigResource(filter, limit, timeSpan);
            resource.Encoding = encoding;
            return new ConfigManager(new Dictionary<string, AConfig>(), manager, resource);
        }

        /// <summary>
        /// 用于按照文件夹生成ConfigManager对象
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="Directory"></param>
        /// <param name="encoding"></param>
        /// <param name="limit">500</param>
        /// <param name="timeSpan">1分钟</param>
        /// <returns></returns>
        public static ConfigManager GetConfigManagerFromDirectory(ConfigManager manager, string filter, Encoding encoding) {
            //pcf中设定struct 属性 调用对象的属性等等功能 未进行详细测试。 暂时取消其定时定量进行测试的功能。
#if DEBUG
            return GetConfigManagerFromDirectory(manager, filter, encoding, 0, TimeSpan.FromMinutes(1));
#else
            return GetConfigManagerFromDirectory(manager, filter, encoding, 500, TimeSpan.FromMinutes(1));
#endif


        }

        /// <summary>
        /// 用于按照文件夹生成ConfigManager对象
        /// </summary>
        /// <param name="manager">可以填写为空</param>
        /// <param name="Directory"></param>
        /// <param name="encoding">系统字符集</param>
        /// <returns></returns>
        public static ConfigManager GetConfigManagerFromDirectory(ConfigManager manager, string filter) {
            return GetConfigManagerFromDirectory(manager, filter, System.Text.Encoding.UTF8);
        }

        private static ConfigManager ma = null;
        /// <summary>
        /// 用于获取与程序同名的pcf配置文件，一般格式为 程序名+".pcf" 如test.exe.pcf 否则可能是 MyProcessControler或者网站的Web.pcf但是建议MyProcessControler内使用的对象使用This节点传入ConfigManager
        /// </summary>
        /// <returns></returns>
        public static ConfigManager GetApplicationConfigManager() {
            if (ma == null)
                lock (key)
                    if (ma == null)
                        ma = GetConfigManagerFromFile(GetBassConfigManager(), File.Exists(System.Windows.Forms.Application.ExecutablePath + ".pcf") ? new FileInfo(System.Windows.Forms.Application.ExecutablePath + ".pcf") :
                            (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "MyProcessController.pcf") ? new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "MyProcessController.pcf") : new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "web.pcf")));
            return ma;
        }

        /// <summary>
        /// 用于获取程序运行目录下的pcf配置文件，一般格式为 程序名+".pcf" 如test.exe.pcf
        /// </summary>
        /// <returns></returns>
        public static ConfigManager GetApplicationConfigManager(string file) {
            //System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase
            return GetApplicationConfigManager(GetBassConfigManager(), file);
        }

        /// <summary>
        /// 用于获取程序运行目录下的pcf配置文件，一般格式为 程序名+".pcf" 如test.exe.pcf
        /// </summary>
        /// <returns></returns>
        public static ConfigManager GetApplicationConfigManager(ConfigManager ma, string file) {
            //System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase
            return GetConfigManagerFromFile(ma, new FileInfo(AppDomain.CurrentDomain.BaseDirectory + file));
        }


        private static ConfigManager basema = null;
        private static object key = DateTime.Now;
        /// <summary>
        /// 用于获取默认的基本配置文件信息，如程序执行文件夹内的base.pcf文件。
        /// </summary>
        /// <returns></returns>
        public static ConfigManager GetBassConfigManager() {
            if (basema == null)
                lock (key)
                    if (basema == null)
                        basema = GetApplicationConfigManager(null, "base.pcf");
            return basema;
        }
    }
}
