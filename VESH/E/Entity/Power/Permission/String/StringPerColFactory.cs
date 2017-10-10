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

namespace GCL.Project.VESH.E.Entity.Power.Permission.String {
    public class StringPerColFactory : IPermissionFactory {
        #region IPermissionFactory Members

        public APermissionCollection CreatePermmisonCollection(string ids) {
            return new StringPermissionCollection(ids);
        }

        #endregion
    }
}
