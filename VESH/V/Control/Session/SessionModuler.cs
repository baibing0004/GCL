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

namespace GCL.Project.VESH.V.Control.Session {

    /// <summary>
    /// 一般用于生成SessionDataManager和更新SessionDataManager.Update()
    /// </summary>
    public class SessionModuler : IModuler {
        #region IModuler Members

        public void Init(HttpApplication context) {
        }

        public void BeginRequest(HttpContext context) {
            SessionDataManager sdm = SessionDataManager.GetCurrentSessionDataManager();
        }

        public void EndRequest(HttpContext context) {
            SessionDataManager sdm = SessionDataManager.GetCurrentSessionDataManager();
            sdm.Update();
        }

        public bool IsSecurity {
            get { return false; }
        }
        
        #endregion

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion

        
    }
}
