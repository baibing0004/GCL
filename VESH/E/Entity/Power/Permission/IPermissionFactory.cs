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

namespace GCL.Project.VESH.E.Entity.Power.Permission {
    public interface IPermissionFactory {
        /// <summary>
        /// 由于后置APermissionCollection需要ids参数生成的原因
        /// 设计本类用来生成APermissionCollection对象
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        APermissionCollection CreatePermmisonCollection(string ids);
    }
}
