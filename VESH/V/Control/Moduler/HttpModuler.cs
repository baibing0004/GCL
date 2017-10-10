using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using GCL.Project.VESH.E.Module;
using System.Reflection;
using System.IO.Compression;
namespace GCL.Project.VESH.V.Control.Moduler {
    public class HttpModuler : IHttpModule, System.Web.SessionState.IRequiresSessionState {
        private IModuler[] modulers;
        private IModuler[] rmodulers;

        /// <summary>
        /// WI拦截器，方便进行请求的顺序处理。
        /// </summary>
        public HttpModuler()
            : this(AModule.Root.Middler.GetObjectsByAppName("VESH", "GCL.Project.VESH.V.Control.Module.HttpModuler.IModulers")) {
        }

        public static IModuler[] ToIModuler(object[] o) {
            return Array.ConvertAll<object, IModuler>(o, ToIModuler);
        }

        private static IModuler ToIModuler(object o) {
            return (IModuler)o;
        }

        public HttpModuler(object[] modulers) {
            if (modulers == null || modulers.Length < 1) throw new InvalidOperationException("必须声明至少一个对象，进行顺序数据处理");
            this.modulers = ToIModuler(modulers);
            this.rmodulers = this.modulers.Clone() as IModuler[];
            Array.Reverse(rmodulers);
        }

        static HttpModuler() {
            //解决初次启动，资源共享冲突的问题
            var sdm = AModule.GetCurrentModule().Middler.GetObjectByAppName("VESH", Session.SessionDataManager.SESSIONCACHENAME) as Session.SessionDataManager;
            sdm.Dispose();
        }

        #region IHttpModule Members

        public void Dispose() {
            foreach (IModuler mod in modulers)
                try {
                    mod.Dispose();
                } catch (Exception) {
                }
        }

        /// <summary>
        /// 注册PreRequestHandlerExecute，PostRequestHandlerExecute
        /// 取代BeginRequest,EndRequest方法
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context) {
            context.PreRequestHandlerExecute += new EventHandler(context_BeginRequest);
            context.PostRequestHandlerExecute += new EventHandler(context_EndRequest);
            foreach (IModuler mod in modulers)
                try {
                    mod.Init(context);
                } catch (Exception ex) {
                    AModule.Root.Logger.Error("{0}.{1}:{2}", this.GetType().Name, MethodBase.GetCurrentMethod().DeclaringType.Name, ex);
                }
        }

        void context_BeginRequest(object sender, EventArgs e) {
            try {
                var app = HttpContext.Current;
                if (app != null) {
                    var acceptEncoding = app.Request.Headers["Accept-Encoding"];
                    if (!string.IsNullOrEmpty(acceptEncoding) && acceptEncoding.ToUpperInvariant().Contains("GZIP")) {
                        app.Response.AppendHeader("Content-encoding", "gzip");
                        //自动添加GZIP支持
                        app.Response.Filter = new GZipStream(app.Response.Filter, CompressionMode.Compress);
                    }
                }
                bool match = AModule.Root.HttpSecurityModuleRegex.Match(HttpContext.Current.Request.Path).Success;
                foreach (IModuler mod in modulers)
                    try {
                        if (!mod.IsSecurity || (mod.IsSecurity && !match))
                            mod.BeginRequest(HttpContext.Current);
                    } catch (Exception ex) {
                        AModule.Root.Logger.Error("HttpModuler: {0}.BeginRequest:{1} ", mod.GetType().Name, ex);
                    }
            } catch (System.Threading.ThreadAbortException) {
            }
        }

        void context_EndRequest(object sender, EventArgs e) {
            try {
                bool match = AModule.Root.HttpSecurityModuleRegex.Match(HttpContext.Current.Request.Path).Success;
                foreach (IModuler mod in rmodulers)
                    try {
                        if (!mod.IsSecurity || (mod.IsSecurity && !match))
                            mod.EndRequest(HttpContext.Current);
                    } catch (Exception ex) {
                        AModule.Root.Logger.Error("HttpModuler: {0}.EndRequest:{1} ", mod.GetType().Name, ex);
                    }
            } catch (System.Threading.ThreadAbortException) {
            }
        }



        #endregion
    }
}
