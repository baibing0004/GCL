using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using GCL.Db.Ni;
using GCL.Project.VESH.V.Control.Session;
using System.Web.SessionState;
namespace GCL.Project.VESH.V.View {

    /// <summary>
    /// 用于生成Permisson.js等文件
    /// </summary>
    public class ASPermissionHandler : IHttpHandler, IRequiresSessionState {
        public bool IsReusable {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context) {
            context.Response.Write("OK2!");
            SessionDataManager sdm = SessionDataManager.GetCurrentSessionDataManager();
            HttpRequest request = context.Request;
            if (!string.IsNullOrEmpty(request["_n"])) {
                //需要_nitemplate参数说明 调用的Nitemplate名
                string niname = request["_n"];
                NiTemplate template = sdm.Middler.GetObjectByAppName("Ni", niname) as NiTemplate;
                if (template == null) throw new ArgumentNullException("ASPermissionHandler:没有找到相关Ni对象的" + niname + "对应的template对象");
                //产生数据
                //如果ni命令也执行错误 会抛错 
                NiDataResult result = template.ExcuteQuery("GCL.Project.VESH.E.Entity.Power.Permission.AS.ASPermissionHandler.Permission");
                sdm.DBResult["GCL.Project.VESH.E.Entity.Power.Permission.AS.ASPermissionHandler.Permission"] = result.DataSet;
            }
            //按照文件设定进行 permisson.js的生成
            string file = sdm.AppSettings["V.View.Permission"];
            string json = this.GetJson(sdm.FirstDBResult.Tables[0]);
            System.IO.File.WriteAllText(sdm.Module.MapPath(file), json, System.Text.Encoding.UTF8);
            context.Response.Write(json);
        }

        private string GetJson(DataTable dt) {
            StringBuilder sb = new StringBuilder();
            try {
                foreach (DataRow row in dt.Rows) {
                    sb.AppendFormat("{0}:'{1}',", row["Name"], row["ID"]);
                }
                return string.Format("var pers={{0}};", sb.ToString().TrimEnd(','));
            } finally {
                sb.Remove(0, sb.Length);
                sb = null;
            }
        }
    }
}