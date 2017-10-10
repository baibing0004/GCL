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
using GCL.Project.VESH.V.Control;
using GCL.Project.VESH.V.Control.Session;

namespace GCL.Project.VESH.V.Control.Controler {

    /// <summary>
    /// SPA压缩
    /// </summary>
    public class PageControler : IControler {

        private string paraName;
        protected readonly string extend;


        /// <summary>
        /// 默认参数名为_page
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="extend"></param>
        public PageControler(string paraName, string extend) {
            this.paraName = string.IsNullOrEmpty(paraName) ? "page" : paraName;
            this.extend = extend;
        }

        #region IControler Members
        protected string GetResourcePath(HttpRequest request, HttpContext context, SessionDataManager session, string extend) {
            string path = "";
            if (session.Status.StartsWith(".")) {
                path = GCL.IO.Config.ConfigManagement.AppSettings(session.Module.ConfigManager, session.Module.GetRequestClassType(context) + session.Status);
                if (string.IsNullOrEmpty(path)) {
                    //计算路径 放置在其class相同位置 名称为 状态.aspx等
                    string[] s = request.Path.Split('\\', '/');
                    string[] _path = s[s.Length - 1].Split('.');
                    _path[0] = "";
                    if (_path.Length > 2) {
                        _path[_path.Length - 2] = string.Format("{0}.{1}", session.Status.TrimStart('.'), extend);
                        _path[_path.Length - 1] = "";
                    } else {
                        _path[_path.Length - 1] = string.Format("{0}.{1}", session.Status.TrimStart('.'), extend);
                    }
                    s[s.Length - 1] = string.Join(".", _path).Trim('.');
                    path = string.Join("/", s).TrimEnd('/');
                }
            } else if (!string.IsNullOrEmpty(session.Status)) {
                //如果Status为空值，那么不作任何处理
                path = string.Format("{0}/{1}.{2}", request.ApplicationPath, session.Status, extend);
            } else {
                //如果Status为空值 直接替换后缀名
                string[] s = request.Path.Split('\\', '/');
                string[] _path = s[s.Length - 1].Split('.');
                _path[_path.Length - 1] = extend;
                s[s.Length - 1] = string.Join(".", _path);
                path = string.Join("/", s);
            }

            //context.Response.Write(session.Module.GetRequestClassType(context)+"<br/>");
            //context.Response.Write(path);
            //context.Server.Execute(path, true);
            //context.Server.Transfer(new V.View.ASPermissionHandler(), true);
            return path;
        }

        public virtual void Execute(HttpRequest request, HttpResponse response, HttpContext context, SessionDataManager session) {            
            string path = GetResourcePath(request, context, session, this.extend);
            try {
                //context.Response.Write(session.Module.GetRequestClassType(context)+"<br/>");
                //context.Response.Write(path);
                context.Server.Execute(path, true);                
                //context.Server.Transfer(new V.View.ASPermissionHandler(), true);
            } catch (System.Threading.ThreadAbortException) {
            } catch (HttpException ex) {
                if (ex.GetHttpCode() == 404) throw ex;
                throw new Exception(string.Format("GCL.Project.VESH.V.Control.Controler.PageControler\r\nNext URL:{0}\r\nInnerException:{1}", path, ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString()));
            }
        }

        public string ParaName {
            get { return this.paraName; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion

    }
}
