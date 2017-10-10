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
using GCL.Project.VESH.V.Control.Session;

namespace GCL.Project.VESH.E.Entity.Power.Security {

    /// <summary>
    /// C121221.1.1
    /// 用于登录和取消登录
    /// </summary>
    public class DefaultSecurity : ISecurity {
        #region ISecurity Members
        public void Login(SessionDataManager manager) {
            if (string.IsNullOrEmpty(manager.UserInfo.UserID))
                throw new InvalidOperationException("请设置User的UserID,以作为默认登陆检验码的源码");
            if (string.IsNullOrEmpty(manager.UserInfo.SessionID))
                throw new InvalidOperationException("请设置User的SessionID,以作为默认登陆检验码的源码");
            if (string.IsNullOrEmpty(manager.UserInfo.LoginTime))
                manager.UserInfo.LoginTime = DateTime.Now.ToString();
        }

        public bool CheckLogin(SessionDataManager manager) {
            return !string.IsNullOrEmpty(manager.UserInfo.UserID);
            //return PermissionData.GetMD5HashCode(manager.UserInfo.Serialize()).Equals(manager.SecurityInfo.Code);
        }

        public void Logout(SessionDataManager manager) {
        }
        #endregion
    }
}
