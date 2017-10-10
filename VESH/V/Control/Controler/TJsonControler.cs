using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Data.SqlClient;
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
    /// 用于远程调用返回TJSON格式数据。 需要前台调用 TJSON转换成JSON格式使用
    /// </summary>
    public class TJsonControler : JsonControler {
        public TJsonControler(string paraName)
            : base(string.IsNullOrEmpty(paraName) ? "tjson" : paraName) {
        }

        /// <summary>
        /// 统一显示
        /// </summary>
        /// <param name="result"></param>
        /// <param name="context"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public override string ToJSON(DataSet result, HttpContext context, GCL.Project.VESH.V.Control.Session.SessionDataManager session) {
            return string.Format("[{0}]", GCL.Db.DBTool.ToTJSON(result));
        }
    }
}
