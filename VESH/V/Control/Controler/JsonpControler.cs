using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Text;
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
    /// 跨域调用的Jsonp JSON格式数据。
    /// </summary>
    public class JsonpControler : JsonControler {
        public JsonpControler(string paraName)
            : base(string.IsNullOrEmpty(paraName) ? "jsonp" : paraName) {
        }

        public override string ToJSON(DataSet result, HttpContext context,SessionDataManager session) {
            if (string.IsNullOrEmpty(context.Request["_bk"])) throw new ArgumentNullException("JsonpHandler:URL丢失_bk参数");
            //可能会有错误
            return string.Format("{0}('{1}');", context.Request["_bk"], base.ToJSON(result, context, session));
        }
    }
}
