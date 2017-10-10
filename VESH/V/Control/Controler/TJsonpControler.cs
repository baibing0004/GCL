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
using GCL.Project.VESH.V.Control;
using GCL.Project.VESH.V.Control.Session;

namespace GCL.Project.VESH.V.Control.Controler {
    /// <summary>
    /// 跨域调用的Jsonp TJSON格式数据。
    /// </summary>
    public class TJsonpControler : TJsonControler {
        public TJsonpControler(string paraName)
            : base(string.IsNullOrEmpty(paraName) ? "tjsonp" : paraName) {
        }

        public override string ToJSON(DataSet result, HttpContext context, GCL.Project.VESH.V.Control.Session.SessionDataManager session) {
            if (string.IsNullOrEmpty(context.Request["_bk"])) throw new ArgumentNullException("TJsonpWiHandler:URL丢失_ck参数");
            return string.Format("{0}('{1}');", context.Request["_bk"], base.ToJSON(result, context, session));
        }
    }
}
