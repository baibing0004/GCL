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
using GCL.Project.VESH.V.Control.Session;

namespace GCL.Project.VESH.V.Control.Session.Resource {
    public class NoStoreSessionResource : ISessionResource {
        #region ISessionResource Members

        public void Save(SessionData data, string text) {
        }

        public string Load(SessionData data) {
            return "";
        }

        public void Clear(SessionData data) {
        }

        #endregion


        public SessionData CreateSessionData(string id, string name) {
            return new SessionData(id, name);
        }
    }
}
