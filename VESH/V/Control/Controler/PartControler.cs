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
    /// WebInterface后续页面处理控制器
    /// </summary>
    public class PartControler : PageControler {

        private string partURL;

        /// <summary>
        /// 默认参数名为_page
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="extend"></param>
        public PartControler(string paraName, string partURL)
            : base(string.IsNullOrEmpty(paraName) ? "part" : paraName, "ascx") {
            this.partURL = partURL;
        }

        #region IControler Members

        public override void Execute(HttpRequest request, HttpResponse response, HttpContext context, GCL.Project.VESH.V.Control.Session.SessionDataManager session) {
            string path = GetResourcePath(request, context, session, this.extend);
            try {
                session.Status = path;
                context.Server.Execute(string.Format("{0}/{1}", context.Request.ApplicationPath, this.partURL));
            } catch (System.Threading.ThreadAbortException) {
            } catch (HttpException ex) {
                if (ex.GetHttpCode() == 404) throw ex;
                throw new Exception(string.Format("GCL.Project.VESH.V.Control.Controler.PartControler\r\nNext URL:{0}\r\nInnerException:{1}", path, ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString()));
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion
    }
}
