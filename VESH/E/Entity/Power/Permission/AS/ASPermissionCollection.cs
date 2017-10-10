using System;
using System.Data;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace GCL.Project.VESH.E.Entity.Power.Permission.AS {

    /// <summary>
    /// 使用假名方式进行权限转换判定
    /// </summary>
    public class ASPermissionCollection : String.StringPermissionCollection {

        private NameValueCollection idic;
        public ASPermissionCollection(string ids, NameValueCollection idi) : base(ids) { this.idic = idi; }

        public override bool HasRight(string id) {
            return base.HasRight(this.idic[id]);
            //idic.GetObjectData(new System.Runtime.Serialization.SerializationInfo(typeof(NameValueCollection)),new System.Runtime.Serialization.IFormatterConverter(){}, System.Runtime.Serialization.StreamingContext);
            //@dis
        }
    }
}
