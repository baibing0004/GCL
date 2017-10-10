using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using GCL.Project.VESH.V.Control.Session;

namespace GCL.Project.VESH.V.Control.Controler {
    public class JnpControler : IControler {
        private string paraName;
        protected readonly string extend;

        /// <summary>
        /// 默认参数名为_page
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="extend"></param>
        public JnpControler(string paraName, string extend) {
            this.paraName = string.IsNullOrEmpty(paraName) ? "jnp" : paraName;
            this.extend = extend;
        }


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

        public void Execute(HttpRequest request, HttpResponse response, HttpContext context, Session.SessionDataManager session) {
            if (string.IsNullOrEmpty(context.Request["_bk"])) throw new ArgumentNullException("JnpControler:URL丢失_bk参数");
            response.Clear();
            response.ContentType = GCL.Net.MIME.js;
            string path = GetResourcePath(request, context, session, this.extend);
            if (string.IsNullOrEmpty(path) || !File.Exists(request.MapPath(path)))
                throw new Exception(string.Format("GCL.Project.VESH.V.Control.Controler.JnpControler 找不到指定的资源！\r\nNext URL:{0}", path));
            path = request.MapPath(path);
            response.Write(string.Format("{0}('{1}');", context.Request["_bk"], GCL.Common.Tool.WebEncode(File.ReadAllText(path).Replace('\'', '"').Replace(Environment.NewLine, ""))));
            response.Expires = 0;
        }

        public string ParaName {
            get { return this.paraName; }
        }

        public void Dispose() {
        }
    }
}