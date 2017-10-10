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
using System.Collections;

namespace GCL.Project.VESH.E.Entity.Power.Permission.String {

    /// <summary>
    /// C121221.1.2
    /// 字符串权值串判断
    /// </summary>
    public class StringPermissionCollection : APermissionCollection {
        private IDictionary idic;

        public StringPermissionCollection(string ids)
            : base(ids) {
            this.idic = new System.Collections.Generic.Dictionary<string, string>();
            foreach (string key in ids.Split(',', ';'))
                if (!string.IsNullOrEmpty(key)) this.idic.Add(key, "1");
        }

        #region APermissionCollection Members

        public override bool HasRight(string id) {
            return this.idic.Contains(id);
        }

        #endregion
    }
}
