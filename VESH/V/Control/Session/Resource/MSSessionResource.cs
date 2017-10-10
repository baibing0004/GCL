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
using GCL.Project.VESH.V.Control.Session.Resource;

namespace GCL.Project.VESH.V.Control.Session.Cache {
    /// <summary>
    /// C121221.Session.MSSessionResource
    /// SessionData的微软Session存放方式
    /// </summary>
    public class MSSessionResource : ISessionResource {

        #region ISessionResource Members

        public void Save(SessionData data, string text) {
            HttpContext.Current.Session[data.Name] = text;
        }

        public string Load(SessionData data) {
            return Convert.ToString(HttpContext.Current.Session[data.Name]);
        }

        public void Clear(SessionData data) {
            HttpContext.Current.Session.Remove(data.Name);
        }

        #endregion



        public SessionData CreateSessionData(string id, string name) {
            return new SessionData(id, name);
        }
    }
}
