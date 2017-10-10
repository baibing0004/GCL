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

namespace GCL.Project.VESH.V.Control.Session.Resource {
    /// <summary>
    /// C121221.1.1
    /// 定值Session源
    /// </summary>
    public class ConstSessionResource : ISessionResource {

        private string defaultValue;
        /// <summary>
        /// 这里定义的值为定值
        /// </summary>
        /// <param name="defaultValue"></param>
        public ConstSessionResource(string defaultValue) {
            this.defaultValue = defaultValue;
        }

        #region ISessionResource Members

        public void Save(SessionData data, string text) {
        }

        public string Load(SessionData data) {
            return defaultValue;
        }

        public void Clear(SessionData data) {
        }

        #endregion




        public SessionData CreateSessionData(string id, string name) {
            return new SessionData(id, name);
        }
    }
}
