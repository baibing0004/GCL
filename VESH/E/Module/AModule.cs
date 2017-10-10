using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using GCL.Bean.Middler;
using GCL.Event;
using GCL.IO.Config;
using GCL.IO.Log;
using System.Text.RegularExpressions;
using GCL.Db.Ni;

namespace GCL.Project.VESH.E.Module {

    /// <summary>
    /// 每个分站点都必须实现的Web插件对象 
    /// </summary>
    public abstract class AModule : IDisposable {

        //测试结果 当子程序单独实现应用程序时，主文件夹下的子文件夹实现的是用于将在主程序基础上的子程序。其web.config和主程序之间有继承关系，但是执行主体仍然是子程序。
        // http://localhost/VESH/VESHTest2/vs.default.page
        //当子程序仅作为文件夹实现应用程序时，其Web.config的大部分内容受到限制，且通用ascx不能互相使用。
        static AModule() {
            _instance = new RootModule();
            _instance.Init(ConfigManagerFactory.GetBassConfigManager());
            _root = _instance;
            //idic[_instance.Path.ToLower()] = _instance;
            object[] parts = _instance.Middler.GetObjectsByAppName("VESH", "GCL.Project.VESH.E.Module.AModule.Modules");
            if (parts != null)
                foreach (AModule part in parts) {
                    part.Init(_instance.ConfigManager);
                    idic[part.Path.ToLower()] = part;
                }
            if (idic.ContainsKey("")) _instance = idic[""];
        }

        private static readonly AModule _instance;
        private static readonly AModule _root;

        /// <summary>
        /// 获得该站点的根Module对象
        /// </summary>
        public static AModule Root {
            get { return AModule._instance; }
        }

        /// <summary>
        /// 获得根Module对象 一定是RootModule
        /// </summary>
        public static AModule RootModule {
            get { return _root; }
        }



        //private string dllpath;

        ///// <summary>
        ///// 用于返回dll类型或者路径 默认的这里不做load操作
        ///// </summary>
        //public string Dll {
        //    get { return dllpath; }
        //}

        private string package;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">应用名 ApplicationPath的最后一段 写多个/隔开的路径时，默认为第一个</param>
        /// <param name="package">截取路径后转换类名时提供默认的前缀</param>
        public AModule(string path, string package) {
            this.path = path.Trim('/', '\\').Split('/', '\\')[0];
            //this.dllpath = dll;
            this.package = package.TrimEnd('.');
        }

        /// <summary>
        /// 参数初始化 根据 程序运行目录 + path + web.pcf文件 设定配置文件级别
        /// 目前仅支持 VESH/app 这样的结构 且仅提供app名即可，不允许path参数设定为多级文件夹
        /// </summary>
        /// <param name="ma"></param>
        private void Init(ConfigManager ma) {
            //{0}AppDomain.CurrentDomain.BaseDirectory,
            this.cm = ConfigManagerFactory.GetApplicationConfigManager(ma, string.Format("{0}{1}web.pcf", this.path, this.path.Length > 0 ? "/" : ""));
            this.cm.ConfigManagerFillEvent += new EventHandle(cm_ConfigManagerFillEvent);
            this.ma = new Middler(cm);
            this.NiTemplateManager = new NiTemplateManager(this.ma, "Ni");
        }

        /// <summary>
        /// 特别处理在ConfigManger因为配置文件改变而从新初始化后，进行更新引用的处理。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cm_ConfigManagerFillEvent(object sender, EventArg e) {
            lock (this) {
                this.cm = sender as ConfigManager;
                this.ma = new Middler(cm);
                this.NiTemplateManager = new NiTemplateManager(this.ma, "Ni");
            }
        }

        private static readonly IDictionary<string, AModule> idic = new System.Collections.Generic.Dictionary<string, AModule>();
        /// <summary>
        /// 获取当前页面所属的Module对象
        /// 20120613 解决虚拟App找到对应Module的错误。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static AModule FindModule(HttpContext context) {
            string[] _path = context.Request.Path.Substring(context.Request.ApplicationPath.Length).Trim(new char[] { '/', '\\' }).Split(new char[] { '/', '\\' });
            if (_path.Length > 0) {
                string name = _path[0].ToLower();
                if (idic.ContainsKey(name)) return idic[name];
                else return _instance;
            } else return _instance;

            /*
            string[] moduleName = context.Request.ApplicationPath.Split(new char[] { '/', '\\' });
            if (moduleName.Length > 0) {
                //处理不是/Default.aspx这种情况
                string name = moduleName[moduleName.Length - 1].ToLower();
                if (idic.ContainsKey(name)) return idic[name];
            }
            return _instance;
            */
        }

        /// <summary>
        /// 获取当前会话的Module
        /// </summary>
        /// <returns></returns>
        public static AModule GetCurrentModule() {
            return FindModule(HttpContext.Current);
        }

        private Middler ma;

        /// <summary>
        /// 中介者
        /// </summary>
        public Middler Middler {
            get { return ma; }
        }

        private ConfigManager cm;

        /// <summary>
        /// Config管理员
        /// </summary>
        public ConfigManager ConfigManager {
            get { return cm; }
        }

        public NiTemplateManager NiTemplateManager {
            get;
            private set;
        }


        /// <summary>
        /// 日志对象
        /// </summary>
        public Logger Logger {
            get {
                return ma.GetObjectByAppName<Logger>("VESH", "GCL.Project.VESH.E.Module.AModule.Logger");
            }
        }




        /// <summary>
        ///默认安全模式的例外路径正则，否则为/login.aspx -todo 未处理未登陆的例外情况
        /// </summary>
        public Regex HttpSecurityModuleRegex {
            get {
                return ma.GetObjectByAppName<Regex>("VESH", "GCL.Project.VESH.E.Module.AModule.HttpSecurityModuleRegex");
            }
        }

        /// <summary>
        /// 默认登陆页，否则为/login.aspx?return={0} -todo 未处理未登陆的例外情况
        /// </summary>
        public string HttpSecurityModuleLoginURL {
            get {
                return string.Format("{0}/{1}", HttpContext.Current.Request.ApplicationPath, GCL.Common.Tool.GetValue(ConfigManagement.AppSettings(cm, "GCL.Project.VESH.E.Module.AModule.HttpSecurityModuleLoginURL"), "login.aspx?return={0}")); ;
            }
        }

        public void RedirectLoginURL(string back) {
            try {
                HttpContext context = HttpContext.Current;
                context.Response.Redirect(string.Format(this.HttpSecurityModuleLoginURL, string.IsNullOrEmpty(back) ? "" : string.Format("?return={0}", context.Server.UrlEncode(back)), true));
            } catch (System.Threading.ThreadAbortException) {
            }
        }

        private string path;

        /// <summary>
        /// 获取应用名 一般没有任何 格式符 
        /// </summary>
        public string Path {
            get { return path; }
        }

        /// <summary>
        /// 获得绝对路径
        /// </summary>
        /// <param name="mpath"></param>
        /// <returns></returns>
        public string MapPath(string mpath) {
            if (mpath.StartsWith("/"))
                return AppDomain.CurrentDomain.BaseDirectory + mpath;
            else
                return HttpContext.Current.Server.MapPath(mpath);
            /*
            mpath = mpath.TrimStart(new char[] { '/', '\\' });
            return ((!mpath.StartsWith(this.path, true, System.Globalization.CultureInfo.CurrentCulture)) ? ("/" + this.path + "/") : "/") + mpath;
             * */
        }

        /// <summary>
        /// 根据URL获取当前RequestClassType
        /// /app/e/module/abc.do.page
        /// /app/e/module/abc.page
        /// todo 这里未处理方法名
        /// </summary>
        /// <returns></returns>
        public string GetRequestClassType(HttpContext context) {
            string[] _s = context.Request.Path.Substring(context.Request.ApplicationPath.Length).Split(new char[] { '/', '\\' });
            _s[_s.Length - 1] = "";
            return string.Format("{0}.{1}", this.package, string.Join(".", _s).TrimStart('.').TrimEnd('.'));
        }

        /// <summary>
        /// 获取AppSettings
        /// </summary>
        public KeyValueClass<String> AppSettings {
            get {
                return new KeyValueClass<string>(this.GetAppSettings);
            }
        }

        /// <summary>
        /// 获取AppSettings设定
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetAppSettings(string key) {
            return ConfigManagement.AppSettings(this.cm, key);
        }

        /// <summary>
        /// 获取ConnectionStrings
        /// </summary>
        public KeyValueClass<String> ConnectionStrings {
            get {
                return new KeyValueClass<string>(this.GetConnectionStrings);
            }
        }
        /// <summary>
        /// 获取ConnectionStrings设定
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConnectionStrings(string key) {
            return ConfigManagement.ConnectionStrings(this.cm, key);
        }

        #region IDisposable Members

        public void Dispose() {
            if (ma != null) {
                this.ma.Dispose();
                ma = null;
            }
        }

        #endregion
    }
}
