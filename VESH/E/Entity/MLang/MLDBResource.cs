using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using GCL.Db.Ni;
using GCL.Project.VESH.V.Control.Session;
namespace GCL.Project.VESH.E.Entity.MLang {

    /// <summary>
    /// MLang专用多语言DB数据源 从数据库中获取指定的多语言说明，其列字段为 MLang,PackageID,Key,Value
    /// 缺少能根据ID 进行缓存的对象。需要扩展s
    /// </summary>
    public class MLDBResource : ISessionResource {

        private NiTemplate template;
        public MLDBResource(NiTemplate t) {
            this.template = t;
        }

        #region ISessionResource Members

        public void Save(SessionData data, string text) {
        }

        public string Load(SessionData data) {
            string s = data.SessionID.Split('.')[0];
            IDictionary idic = new Hashtable();
            idic["@PackageID"] = data.SessionID.Substring(s.Length + 1);
            idic["@MLang"] = s;
            System.Data.DataTable dt = this.template.ExcuteScalar("GCL.Project.VESH.E.Entity.MLang.MLDBResource.GetMLangPackage", idic).DataSet.Tables[0];
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try {
                foreach (DataRow row in dt.Rows) {
                    sb.AppendFormat("&{0}={1}", row["mKey"], row["mValue"]);
                }
                data.Deserialize(sb.ToString());
                return sb.ToString();
            } finally {
                sb.Remove(0, sb.Length);
                sb = null;
            }
        }

        public void Clear(SessionData data) {
        }

        #endregion

        public SessionData CreateSessionData(string id, string name) {
            return new SessionData(id, name);
        }
    }
}
