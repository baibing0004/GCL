using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using GCL.Project.VESH.V.Control;
using System.Text;
using System.Data;
using GCL.Db.Ni;
using GCL.Project.VESH.E.Entity.Power.Permission;
namespace GCL.Project.VESH.E.Entity.Power.Permission.AS {

    /// <summary>
    /// 用于获得JS E/Entity/Power/Permission/AS/ASPermissionAction.Update.json
    /// </summary>
    public class ASPermissionAction : IAction {
        public void PreLoad(HttpRequest request, HttpResponse response, HttpContext context, V.Control.Session.SessionDataManager session) {
            //需要_nitemplate参数说明 调用的Nitemplate名
            string niname = "template";
            NiTemplate template = session.Middler.GetObjectByAppName("Ni", niname) as NiTemplate;
            if (template == null) throw new ArgumentNullException("PermissionAction:找到相关方法:" + "PreLoad" + "的情况下尝试Ni对象没有发现" + niname + "对应的template对象");
            //准备参数
            //产生数据
            //如果ni命令也执行错误 会抛错 
            NiDataResult result = template.ExcuteQuery("GCL.Project.VESH.E.Entity.Power.Permission.AS.ASPermissionAction.Permission");
            session.DBResult["GCL.Project.VESH.E.Entity.Power.Permission.AS.ASPermissionAction.Permission"] = result.DataSet;
            IPermissionFactory fac = session.Middler.GetObjectByAppName<IPermissionFactory>("VESH", "GCL.Project.VESH.E.Entity.Power.Permission.IPermissionFactory");
            //更新内存中的对象
            if (fac != null && fac is ASPerColFactory) {
                ((ASPerColFactory)fac).Reset(result.DataSet.Tables[0]);
            }
        }

        public void Update(HttpRequest request, HttpResponse response, HttpContext context, V.Control.Session.SessionDataManager session) {
            //按照文件设定进行 permisson.js的生成
            string file = session.AppSettings["GCL.Project.VESH.E.Entity.Power.Permission.AS.ASPermissionAction.Permission"];
            StringBuilder sb = new StringBuilder();
            try {
                foreach (DataRow row in session.FirstDBResult.Tables[0].Rows) {
                    sb.AppendFormat("{0}:'{1}',", row["Name"], row["ID"]);
                }
                System.IO.File.WriteAllText(session.Module.MapPath(file), string.Format("var pers={{0}};", sb.ToString().TrimEnd(',')), System.Text.Encoding.UTF8);
                session.Status = ".Update";
            } finally {
                sb.Remove(0, sb.Length);
                sb = null;
            }
        }


        public bool IsStatic {
            get { return true; }
        }

        public void Dispose() {
        }
    }
}