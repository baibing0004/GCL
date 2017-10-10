using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using GCL.Db.Ni;
using GCL.Project.VESH.E.Module;
namespace GCL.Project.VESH.V.Control {


    /// <summary>
    /// 适用于域名/app名/类空间名/类名/方法名.操作类型+操作（_a有值说明使用Action，_n有值说明使用template,_ck jsonp方式callback名，_e事件什么都没有说明无数据调用）
    /// 修改_a参数判断为 不带_a进行判断
    /// 
    /// </summary>
    public class HttpHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState {
        private IDictionary<string, IControler> idic = new Dictionary<string, IControler>();
        //Action缓存池
        private IDictionary<string, IAction> idicACTCache = new Dictionary<string, IAction>();
        private IDictionary<string, MethodInfo> idicMethod = new Dictionary<string, MethodInfo>();
        private DateTime LastTime = DateTime.Now;
        private Type[] types = new Type[] { typeof(HttpRequest), typeof(HttpResponse), typeof(HttpContext), typeof(Session.SessionDataManager) };
        public HttpHandler() {
            object[] iwics = AModule.Root.Middler.GetObjectsByAppName("VESH", "GCL.Project.VESH.V.Control.HttpHandler.IControlers");
            if (iwics == null || iwics.Length < 1) throw new InvalidOperationException("必须声明至少一个IControler，进行参数处理");
            foreach (IControler wi in iwics)
                idic[wi.ParaName.ToLower()] = wi;
        }

        static HttpHandler() {
            //解决初次启动，资源共享冲突的问题
            var sdm = AModule.GetCurrentModule().Middler.GetObjectByAppName("VESH", Session.SessionDataManager.SESSIONCACHENAME) as Session.SessionDataManager;
            sdm.Dispose();
        }

        #region IHttpHandler Members

        public bool IsReusable {
            get { return true; }
        }

        private string _ckey = "_ckey";

        /// <summary>
        /// 用于产生Action方法
        /// 对象生成 计算对应类与dll，先使用middler获取对应的对象，然后自动生成对象。否则生成Ni对象
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="context"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        protected virtual IAction GetAction(HttpRequest request, HttpResponse response, HttpContext context, Session.SessionDataManager session, string classType) {
            //if (LastTime.CompareTo(DateTime.Now) < 0) {
            //    //已有10分钟以上没有请求 进行缓存清理 问题是到这中间的时候 action都在等待。基本上是谁先用谁清理。 没考虑Action lock
            //    lock (idicACTCache) {
            //        foreach (IAction action2 in idicACTCache.Values)
            //            try {
            //                action2.Dispose();
            //            } catch (Exception) { }
            //        idicACTCache.Clear();
            //    }
            //}

            IAction action = null;
            //string classType = session.Module.GetRequestClassType(context);
            //带.Action的
            string classType2 = string.Format("{0}.Action", classType);
            if (idicACTCache.ContainsKey(classType))
                return idicACTCache[classType];
            //未锁定idicACTCache 防止出现空值
            else {
                for (int _i = 0; _i < 4; _i++) {
                    object obs = null;
                    //进行多线程判断
                    if (action == null && !idicACTCache.ContainsKey(classType)) {
                        try {
                            switch (_i) {
                                case 0:
                                    obs = session.Module.GetType().Assembly.CreateInstance(classType2, true);
                                    break;
                                case 1:
                                    obs = session.Middler.GetObjectByAppName("VESH", classType);
                                    break;
                                case 2:
                                    string classType3 = string.Format("{0}.Action", AModule.RootModule.GetRequestClassType(context));
                                    obs = AModule.RootModule.GetType().Assembly.CreateInstance(classType3, true);
                                    break;
                                case 3:
                                    obs = Activator.CreateInstance(Type.GetType(classType2, true, true));
                                    break;
                            }
                            if (obs is IAction) {
                                //只处理IAction的子类
                                action = obs as IAction;
                                //这里是生成成功后单线程处理
                                if (action != null && action.IsStatic)
                                    lock (idicACTCache) {
                                        if (!idicACTCache.ContainsKey(classType))
                                            idicACTCache.Add(classType, action);
                                        else {
                                            if (action != null)
                                                try {
                                                    action.Dispose();
                                                } catch {
                                                }
                                            action = idicACTCache[classType];
                                        }
                                    }
                                //退出循环
                                _i = 5;
                            }
                        } catch (Exception ex) {
                            var ss = ex.ToString();
                        }
                    } else if (idicACTCache.ContainsKey(classType)) {
                        if (action != idicACTCache[classType]) {
                            if (action != null)
                                try {
                                    action.Dispose();
                                } catch {
                                }
                            action = idicACTCache[classType];
                        }
                        _i = 5;
                    } else {
                        //这里只有 action 不为空的情况 由前已知应该是没有路径进入的！ 
                    }
                }
            }
            //this.LastTime = DateTime.Now.AddMinutes(10);
            //if (action == null) throw new InvalidOperationException("GetAction 未能确认合理的类" + classType + "来执行处理方法!");
            if (action == null) idicACTCache[classType] = null;
            return action;
        }

        public void ProcessRequest(HttpContext context) {
            try {
                HttpRequest request = context.Request;
                HttpResponse response = context.Response;

                Session.SessionDataManager sdm = Session.SessionDataManager.GetCurrentSessionDataManager();
                #region 参数判断与准备
                IControler control;
                string[] _s = context.Request.Path.Substring(context.Request.ApplicationPath.Length).Split(new char[] { '/', '\\' });
                _s = _s[_s.Length - 1].Split('.');
                string method = _s[0];
                string contype = _s[1].ToLower();
                sdm.Status = !string.IsNullOrEmpty(method) ? ("." + method) : method;
                if (this.idic.ContainsKey(contype))
                    control = idic[contype];
                else
                    throw new InvalidOperationException("未能找到对应" + contype + "类型的控制处理器!");

                string classType = sdm.Module.GetRequestClassType(context);
                #endregion

                IAction action = null;
                //if (!string.IsNullOrEmpty(request["_a"])) {
                #region 对象生成 计算对应类与dll，先使用middler获取对应的对象，然后自动生成对象。否则生成Ni对象
                //_a有值 说明 使用Action方法
                action = GetAction(request, response, context, sdm, classType);

                #endregion
                #region 先查找对象PreLoad方法进行参数初始化，然后调用对象的方法，如果未找对对象的制定方法会试图调用Ni执行对应的数据
                if (action != null) action.PreLoad(request, response, context, sdm);
                #endregion
                //}


                if (!string.IsNullOrEmpty(request["_n"])) {
                    //需要_nitemplate参数说明 调用的Nitemplate名
                    string niname = request["_n"];
                    NiTemplate template = sdm.Middler.GetObjectByAppName("Ni", niname) as NiTemplate;
                    if (template == null) throw new HttpException(404, "HttpHandler:没有找到相关方法" + classType + "." + method + "的情况下二次尝试Ni对象没有发现" + niname + "对应的template对象");
                    //准备参数
                    IDictionary idicParas = PrepareParameters(context, sdm);
                    //产生数据
                    //如果ni命令也执行错误 会抛错 
                    NiDataResult result = Excute(sdm, niname, classType + "." + method, idicParas, template);
                    sdm.DBResult[string.Format("{0}.{1}", classType, method)] = result.DataSet;
                }

                if (action != null && !string.IsNullOrEmpty(method)) {
                    MethodInfo methodInfo = null;
                    string key = action.GetHashCode() + method;
                    if (action.IsStatic && this.idicMethod.ContainsKey(key))
                        methodInfo = this.idicMethod[key];
                    else
                        methodInfo = action.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.CreateInstance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, types, null);
                    if (methodInfo != null)
                        #region 先查找对象PreLoad方法进行参数初始化，然后调用对象的方法，如果未找对对象的制定方法会试图调用Ni执行对应的数据
                        methodInfo.Invoke(action, new object[] { request, response, context, sdm });
                    else if (!string.IsNullOrEmpty(request["_a"]))
                        throw new HttpException(404, "GetAction 未能确认合理的类" + classType + "来执行处理方法!");
                    //GCL.Bean.BeanTool.Invoke(action, method.ToLower(), types, );
                        #endregion
                }

                #region 执行完成后转入Control处理 PageControler注意根据AppSettings 进行状态URL判断
                //response.Clear();
                control.Execute(request, response, context, sdm);
                #endregion

            } catch (System.Threading.ThreadAbortException) {
            } catch (Exception ex2) {
                //只记录 直接抛错
                AModule.GetCurrentModule().Logger.Error(ex2.InnerException == null ? ex2.ToString() : ex2.InnerException.ToString());
                throw ex2;
            } finally {
                try {
                    Session.SessionDataManager sdm = Session.SessionDataManager.GetCurrentSessionDataManager();
                    sdm.Update();
                } catch {
                }
            }
        }

        #endregion

        /// <summary>
        /// 准备参数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        public virtual IDictionary PrepareParameters(HttpContext context, Session.SessionDataManager sdm) {
            IDictionary idic = new Hashtable();
            object[] filters = sdm.Middler.GetObjectsByAppName("VESH", "GCL.Project.VESH.V.Control.HttpHandler.IHttpNiParamFilters");
            if (filters != null)
                foreach (IHttpNiParamFilter filter in filters)
                    filter.Filt(context, idic);

            /*
            //过滤所有名字没有带@号的
            ArrayList list = new ArrayList();
            foreach (object key in idic.Keys)
                if (!key.ToString().StartsWith("@"))
                    list.Add(key);
            foreach (object key in list) {
                idic["@" + key.ToString()] = idic[key];
                idic.Remove(key);
            }
            list.Clear();
            list = null;
            */
            return idic;
        }

        /// <summary>
        /// 用于执行操作产生数据
        /// </summary>
        /// <param name="context"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        public NiDataResult Excute(Session.SessionDataManager part, string niname, string method, IDictionary idic, NiTemplate template) {
            try {
                template.ExcuteQuery(method, idic);
                if (template.Transaction)
                    return template.Commit();
                else
                    return template.Result;
            } finally {
                if (template != null)
                    part.Middler.SetObjectByAppName("Ni", niname, template);
            }
        }


    }
}
