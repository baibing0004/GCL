using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GCL.Db.Ni;
using GCL.Project.VESH.V.Control.Moduler;
using GCL.Project.VESH.V.Control.Session;

namespace GCL.Project.VESH.E.Entity.Power.Security {

    /// <summary>
    /// 专用过滤User详细信息与数据库比较是否一致
    /// </summary>
    public class OnceSecuriy:ISecurity{
        public void Login(SessionDataManager manager) {
             System.Collections.IDictionary idic = new System.Collections.Hashtable();
             idic["@UserID"] = manager.UserInfo.UserID;
             idic["@Code"] = GCL.Common.Tool.GetCRCHashCode(manager.UserInfo.Serialize());
             NiTemplateManager.ExcuteQuery(manager.Middler, "Ni", "SecurityTemplate", "GCL.Project.VESH.E.Entity.Power.Security.OnceSecurity.AddOnceUser", idic);
        }

        public bool CheckLogin(SessionDataManager manager) {
            if (!manager.IsLogin) return false;
            System.Collections.IDictionary idic = new System.Collections.Hashtable();
            idic["@UserID"] = manager.UserInfo.UserID;
            idic["@Code"] = GCL.Common.Tool.GetCRCHashCode(manager.UserInfo.Serialize());
            return Convert.ToBoolean(NiTemplateManager.ExcuteQuery(manager.Middler, "Ni", "SecurityTemplate", "GCL.Project.VESH.E.Entity.Power.Security.OnceSecurity.CheckOnceUser", idic).GetFirstCell());        
        }

        public void Logout(SessionDataManager manager) {
            System.Collections.IDictionary idic = new System.Collections.Hashtable();
            idic["@UserID"] = manager.UserInfo.UserID;
            NiTemplateManager.ExcuteQuery(manager.Middler, "Ni", "SecurityTemplate", "GCL.Project.VESH.E.Entity.Power.Security.OnceSecurity.RemoveOnceUser", idic);        
        }
    }
}