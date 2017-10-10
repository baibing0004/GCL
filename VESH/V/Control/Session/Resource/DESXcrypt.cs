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
    /// 这里只是为了符合IXcrypt的要求
    /// </summary>
    public class DESXcrypt : GCL.Module.DESXcrypt, IXcrypt {

        /// <summary>
        /// 套用继承GCL.Module.Xcrypt，并实现其接口
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        public DESXcrypt(string key, string iv) : base(key, iv) { }
    }
}
