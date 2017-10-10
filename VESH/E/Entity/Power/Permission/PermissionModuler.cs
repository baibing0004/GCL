using System;
using System.Data;
using System.Data.Common;
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
using GCL.Project.VESH.V.Control.Moduler;
using GCL.Project.VESH.V.Control.Session;

namespace GCL.Project.VESH.E.Entity.Power.Permission {

    /// <summary>
    /// 通过todo PagePermission表(全局性的访EntityPermission表设计)进行协作进行permission权限校验的处理 需要内存数据库支持，或者SQL优化 与 比页面代码内的HasRight效率低
    /// 需要内部维持 EntityPermission表，建议使用Path按照/分割再按照HashTable+树遍历方式存储建立Path关系。 必须由Request.Path.SubString(Request.ApplicationPath.Length).Trim('/','\\').Split('/','\\')进行 大小写不敏感
    /// </summary>
    public class PermissionModuler : IModuler {

        HashTree ht;
        #region IModuler Members

        public void Init(HttpApplication context) {
        }

        static readonly string NAME = "GCL.Project.VESH.E.Entity.Power.Permission.PermissionModuler.XAuPermission";
        public void BeginRequest(HttpContext context) {
            SessionDataManager sdm = SessionDataManager.GetCurrentSessionDataManager();
            if (ht == null) lock (this) {
                    //使用HashTree进行内部权限对应关系初始化
                    if (ht == null) {
                        ht = new HashTree();
                        foreach (DataRow row in GCL.Db.Ni.NiTemplateManager.ExcuteQuery(sdm.Middler, "Ni", "template", NAME).DataSet.Tables[0].Rows) {
                            ht.Add(row["Path"].ToString(), row["PerID"].ToString());
                        }
                    }
                }
            if (!ht.Check(context.Request.Path.Substring(context.Request.ApplicationPath.Length + 1), sdm.PermissionData.PermissionCollection)) {
                //无权访问时不能设定为退出

                context.Response.Clear();
                //VESH特有权限不足错误码
                context.Response.StatusCode = 450;
                context.ApplicationInstance.CompleteRequest();
            };
            return;
        }

        public void EndRequest(HttpContext context) {
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion

        public bool IsSecurity {
            get { return true; }
        }
    }
}
