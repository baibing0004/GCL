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
using GCL.Project.VESH.V.Control.Moduler;
using GCL.Project.VESH.V.Control.Session;
namespace GCL.Project.VESH.E.Entity.Power.Security {
    public class SecurityModuler : IModuler {
        #region IModuler Members

        public void Init(HttpApplication context) {
        }

        public void BeginRequest(HttpContext context) {
            SessionDataManager sd = SessionDataManager.GetCurrentSessionDataManager();
            //除了被排除的URL都需要经过匹配,且登录时间在1天之内时。鉴权失败和主动退出不同
            if (!sd.IsLogin) {
                sd.Update();
                sd.Module.RedirectLoginURL(context.Request.Url.PathAndQuery);
            }
        }

        public void EndRequest(HttpContext context) {
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion

        public bool IsSecurity {
            get { return true; }
        }
    }
}
