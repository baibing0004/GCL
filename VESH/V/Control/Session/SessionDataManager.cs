using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using GCL.Common;
using GCL.Bean.Middler;
using GCL.Event;
using GCL.IO.Log;
using GCL.Project.VESH.E.Module;
using GCL.Project.VESH.E.Entity.Power.Security;
using GCL.Project.VESH.E.Entity.Power.Permission;
using GCL.Project.VESH.E.Entity.MLang;
using GCL.Db.Ni;
namespace GCL.Project.VESH.V.Control.Session {
    /// <summary>
    /// C121221.1.1
    /// 用于管理某个会话一次处理的会话数据 使用外观模式统一提供数据功能访问接口，原则是由其它组件负责实现功能，这里只提供综合。
    /// </summary>
    public class SessionDataManager : IDisposable {
        private SessionDataAdapter adapter;
        private IDictionary idic;
        private MLanguage lang;
        private ISecurity se;

        public SessionDataManager(SessionDataAdapter adapter, ISecurity se, MLanguage lang) {
            this.adapter = adapter;
            this.idic = new Hashtable();
            this.user = new User();
            this.adapter.GetSessionData(user);
            this.se = se;
            //这里在没有数据时的特别处理
            SetLogin(user.Count > 0 ? se.CheckLogin(this) : false);
            this.lang = lang;
            //if (this.IsLogin) 
            this.lang.Lang = Tool.GetValue(user[".MLang"], "zh-CN").ToString();
            this.permissions = new PermissionData(user, GetCurrentPermissionFactory());
            if (this.IsLogin)
                this.adapter.GetSessionData(permissions);
        }

        /// <summary>
        /// 登陆判断
        /// </summary>
        public ISecurity Security {
            get { return se; }
        }

        /// <summary>
        /// 获取Identity对象工厂
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static IPermissionFactory GetCurrentPermissionFactory(AModule part) {
            return part.Middler.GetObjectByAppName<IPermissionFactory>("VESH", "GCL.Project.VESH.E.Entity.Power.Permission.IPermissionFactory");
        }

        /// <summary>
        /// 默认使用HttpContext的Current获取Identity对象工厂
        /// </summary>
        /// <returns></returns>
        public static IPermissionFactory GetCurrentPermissionFactory() {
            return GetCurrentPermissionFactory(AModule.GetCurrentModule());
        }


        public const string SESSIONCACHENAME = "GCL.Project.VESH.V.Control.Session.SessionDataManager";
        /// <summary>
        /// 从Session中获取这个会话的SessionDataManager的变量，如果其不存在那么从Part中加载新的对象
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static SessionDataManager GetCurrentSessionDataManager(AModule part) {
            HttpContext context = HttpContext.Current;
            if (!context.Request.RequestContext.RouteData.Values.ContainsKey(SESSIONCACHENAME)) {
                context.Request.RequestContext.RouteData.Values.Add(SESSIONCACHENAME, part.Middler.GetObjectByAppName("VESH", SESSIONCACHENAME));
            }
            return context.Request.RequestContext.RouteData.Values[SESSIONCACHENAME] as SessionDataManager;
            //if (context.Session != null) {
            //    if (context.Session[SESSIONCACHENAME] == null)
            //        lock (context.Session) {
            //            if (context.Session[SESSIONCACHENAME] == null)
            //                context.Session.Add(SESSIONCACHENAME, part.Middler.GetObjectByAppName("VESH", SESSIONCACHENAME));
            //        }
            //    return context.Session[SESSIONCACHENAME] as SessionDataManager;
            //} else
            //    throw new InvalidOperationException("不支持Session方式!无法存储和传递中间状态!");
            //return part.Middler.GetObjectByAppName("VESH", SESSIONCACHENAME) as SessionDataManager;
        }

        /// <summary>
        /// 默认使用HttpContext的Current获取Context
        /// </summary>
        /// <returns></returns>
        public static SessionDataManager GetCurrentSessionDataManager() {
            return GetCurrentSessionDataManager(AModule.GetCurrentModule());
        }

        /// <summary>
        /// 用于在页面退出时更新Session数据
        /// </summary>
        public virtual void Update() {
            try {
                for (IDictionaryEnumerator ie = idic.GetEnumerator(); ie.MoveNext(); ) {
                    SessionData data = ie.Value as SessionData;
                    if (data.IsClear)
                        this.adapter.ClearSessionData(data);
                    else
                        this.adapter.SaveSessionData(data);
                }
                if (!this.islogin) { user.UserID = string.Empty; }
                if (string.IsNullOrEmpty(user.SessionID)) { user.SessionID = SessionData.CreateGUIDSessionID(); }
                this.adapter.SaveSessionData(user);
                if (this.islogin) {
                    this.adapter.SaveSessionData(permissions);
                } else {
                    this.adapter.ClearSessionData(permissions);
                }
            } catch (HttpException ex) {
                try {
                    part.Logger.Error(ex.ToString());
                } catch {
                }
            } finally {
                if (HttpContext.Current.Request.RequestContext.RouteData.Values.ContainsKey(SESSIONCACHENAME)) {
                    var sdm = HttpContext.Current.Request.RequestContext.RouteData.Values[SESSIONCACHENAME];
                    HttpContext.Current.Request.RequestContext.RouteData.Values.Remove(SESSIONCACHENAME);
                    if (part != null && part.Middler != null && sdm != null)
                        part.Middler.SetObjectByAppName("VESH", SESSIONCACHENAME, sdm);
                }
                //if (HttpContext.Current.Session != null)
                //    HttpContext.Current.Session.Remove(SESSIONCACHENAME);
            }
        }

        /// <summary>
        /// 获取与本次登陆相关SessionData
        /// 其不同Name的SessionData可以配置SessionDataAdapter中对应不同的存取途径。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SessionData this[string name] {
            get {
                if (name.Equals(user.Name, StringComparison.InvariantCultureIgnoreCase))
                    return user;
                else if (name.Equals(permissions.Name, StringComparison.InvariantCultureIgnoreCase))
                    return permissions;
                else if (!this.idic.Contains(name))
                    lock (idic) {
                        if (!this.idic.Contains(name))
                            idic[name] = this.adapter.GetSessionData(UserInfo.SessionID, name);
                    }
                return idic[name] as SessionData;
            }
        }

        //<summary>
        //获取与本次登陆相关SecurityData
        //其不同Name的SecurityData可以配置SessionDataAdapter中对应不同的存取途径。
        //</summary>
        //<param name="name"></param>
        //<returns></returns>
        public SpecialData SpecialData(string name) {
            if (name.Equals(permissions.Name, StringComparison.InvariantCultureIgnoreCase))
                return permissions;
            else if (!this.idic.Contains(name))
                lock (idic) {
                    if (!this.idic.Contains(name)) {
                        SpecialData data = new SpecialData(user, name);
                        idic[name] = data;
                        this.adapter.GetSessionData(data);
                    }
                }
            return idic[name] as SpecialData;
        }


        private User user;
        /// <summary>
        /// 会话中的公开信息
        /// </summary>
        public User UserInfo {
            get {
                return user;
            }
        }

        private PermissionData permissions;
        /// <summary>
        /// 会话中权限信息
        /// </summary>
        public PermissionData PermissionData {
            get { return permissions; }
        }

        /// <summary>
        /// 判断有无权限
        /// </summary>
        /// <param name="per"></param>
        /// <returns></returns>
        public bool Check(string per) {
            return PermissionData.PermissionCollection.HasRight(per);
        }

        ///<summary>
        ///如果没有权限那么会被退出
        ///真表示拥有权限，假为没有权限，这里只是设定session状态为退出，这样在页面操作完成后
        ///</summary>
        ///<param name="per"></param>
        public bool Out(string per) {
            if (!Check(per)) {
                this.Logout();
                return false;
            }
            return true;
        }

        private bool islogin = true;

        /// <summary>
        /// 代码自动根据是否已经登录记录User和Security的信息
        /// </summary>
        public bool IsLogin {
            get { return islogin; }
        }

        protected void SetLogin(bool login) {
            this.islogin = login;
        }
        /// <summary>
        /// 设定为登录状态
        /// </summary>
        public void Login() {
            this.SetLogin(true);
        }

        /// <summary>
        /// 设定为退出状态
        /// </summary>
        public void Logout() {
            this.SetLogin(false);
            this.UserInfo.Clear();
            for (IDictionaryEnumerator ie = idic.GetEnumerator(); ie.MoveNext(); ) {
                SessionData data = ie.Value as SessionData;
                data.Clear();
            }
            this.adapter.ClearSessionData(this.user);
        }


        private AModule part;
        /// <summary>
        /// 本次请求对应的Module实例
        /// </summary>
        public AModule Module {
            get {
                if (!Tool.IsEnable(part))
                    lock (this) {
                        if (!Tool.IsEnable(part))
                            part = AModule.GetCurrentModule();
                    }
                return part;
            }
        }

        private IDictionary<string, DataSet> dbr = new Dictionary<string, DataSet>();
        private bool islogout;
        /// <summary>
        /// 用来存放整个请求期间的过程数据
        /// </summary>
        public IDictionary<string, DataSet> DBResult {
            get { return dbr; }
        }

        /// <summary>
        /// 使用DBResult.ToArray<DataSet>() 也有效
        /// </summary>
        public DataSet FirstDBResult {
            get { return dbr.Values.ToArray<DataSet>()[0]; }
        }

        /// <summary>
        /// 用于说明当前请求的状态
        /// </summary>
        public string Status {
            get;
            set;
        }

        /// <summary>
        /// 获取中介者对象
        /// </summary>
        public Middler Middler { get { return Module.Middler; } }

        /// <summary>
        /// 获取日志对象
        /// </summary>
        public NiTemplateManager NiTemplateManager { get { return Module.NiTemplateManager; } }

        /// <summary>
        /// 获取日志对象
        /// </summary>
        public Logger Logger { get { return Module.Logger; } }

        /// <summary>
        /// 获取多语言对象
        /// </summary>
        public MLanguage MLang { get { return this.lang; } }

        /// <summary>
        /// 获取web.pcf中递归定义的配置项
        /// </summary>
        public KeyValueClass<string> AppSettings { get { return Module.AppSettings; } }

        /// <summary>
        /// 获取web.pcf中递归定义的数据库连接字符串
        /// </summary>
        public KeyValueClass<string> ConnectionStrings { get { return Module.ConnectionStrings; } }


        public void Dispose() {
            this.idic.Clear();
            this.dbr.Clear();
        }
    }
}