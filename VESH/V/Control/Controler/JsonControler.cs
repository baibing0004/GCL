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
using GCL.Project.VESH.V.Control.Session;

namespace GCL.Project.VESH.V.Control.Controler {
    /// <summary>
    /// 用于远程调用返回JSON格式数据。
    /// </summary>
    public class JsonControler : IControler {

        private string paraName;
        public JsonControler(string paraName) {
            this.paraName = string.IsNullOrEmpty(paraName) ? "json" : paraName;
        }

        #region IControler Members

        public virtual void Execute(HttpRequest request, HttpResponse response, HttpContext context, SessionDataManager session) {
            try {
                //一般很少有超出1个DataSet的情况
                string json = session.DBResult.Count > 0 ? this.ToJSON(session.FirstDBResult, context, session) : "[]";
                response.Clear();
                response.Write(json);
                response.ContentType = GCL.Net.MIME.js;
                response.Expires = 0;
            } catch (Exception ex) {
                response.Clear();
                response.Write("[False]");
                session.Logger.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 生成结果
        /// </summary>
        /// <param name="result"></param>
        /// <param name="idic"></param>
        /// <returns></returns>
        public virtual string ToJSON(DataSet result, HttpContext context, SessionDataManager session) {
            return GCL.Db.DBTool.ToJSON(result);
        }

        public string ParaName {
            get { return this.paraName; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose() { }

        #endregion
    }
}
