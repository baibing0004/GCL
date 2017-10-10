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
using GCL.Project.VESH.E.Entity.Power.Security;
namespace GCL.Project.VESH.E.Entity.Power.Permission {
    /// <summary>
    /// C121221.1.1
    /// 用于管理需要加密的数据
    /// </summary>
    public class PermissionData : SpecialData {
        /// <summary>
        /// 默认的属性名
        /// </summary>
        public static readonly string SNAME = ".SPIDS";
        /// <summary>
        /// 用户权限串的Key
        /// </summary>
        public static readonly string IDENTITYNAME = "Pers";
        private IPermissionFactory factory;
        public PermissionData(User user, IPermissionFactory factory) : base(user, SNAME) { this.factory = factory; }

        public override string Serialize() {
            if (idcol != null)
                this[IDENTITYNAME] = idcol.PermissionCollections;
            return base.Serialize();
        }

        public override void Deserialize(string data, bool isClear) {
            base.Deserialize(data, isClear);
            if (!string.IsNullOrEmpty((string)this[IDENTITYNAME]))
                idcol = this.factory.CreatePermmisonCollection(this[IDENTITYNAME] as string);
        }

        /// <summary>
        /// 覆盖Init操作重新初始化权限串
        /// </summary>
        /// <param name="data"></param>
        public override void Init(string data) {
            if (data.IndexOf("&") >= 0) base.Deserialize(data, true);
            idcol = this.factory.CreatePermmisonCollection(data);
        }

        private APermissionCollection idcol;
        /// <summary>
        /// 权限串处理对象
        /// </summary>
        public APermissionCollection PermissionCollection {
            get { return idcol; }
        }
    }
}
