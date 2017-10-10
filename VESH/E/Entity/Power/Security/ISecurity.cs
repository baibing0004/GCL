using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using GCL.Project.VESH.V.Control.Session;

namespace GCL.Project.VESH.E.Entity.Power.Security {
    /// <summary>
    /// 用于实现管理用户登录信息的状态
    /// </summary>
    public interface ISecurity {

        /// <summary>
        /// 记录登录信息
        /// </summary>
        void Login(SessionDataManager manager);
        /// <summary>
        /// 判断登录信息
        /// </summary>
        /// <returns></returns>
        bool CheckLogin(SessionDataManager manager);
        /// <summary>
        /// 取消登录信息
        /// </summary>
        void Logout(SessionDataManager manager);
    }
}
