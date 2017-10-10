using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GCL.Project.VESH.V.Control.Session;

namespace GCL.Project.VESH.E.Entity.Power.Security {

    /// <summary>
    /// C121221.1.1
    /// 用于设置登录总时间限制在一定时间内
    /// </summary>
    public class LoginTimeSecurity : ISecurity {

        private int hours;
        public LoginTimeSecurity() : this(6) { }
        public LoginTimeSecurity(int hours) {
            this.hours = hours;
        }

        #region ISecurity Members

        public void Login(SessionDataManager manager) {
        }

        public bool CheckLogin(SessionDataManager manager) {
            return Convert.ToDateTime(manager.UserInfo.LoginTime).AddHours(this.hours).CompareTo(DateTime.Now) > 0;
        }

        public void Logout(SessionDataManager manager) {
        }

        #endregion
    }
}
