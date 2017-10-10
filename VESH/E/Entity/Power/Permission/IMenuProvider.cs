using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GCL.Project.VESH.E.Entity.Power.Permission {
    public interface IMenuProvider {
        /// <summary>
        ///  绑定Menu控件
        /// </summary>
        /// <param name="menu"></param>
        void BindMenu(APermissionCollection value, System.Web.UI.WebControls.Menu menu);

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string ToXML(APermissionCollection value);

        /// <summary>
        /// 绑定XSLT控件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="xml"></param>
        void BindXML(APermissionCollection value, System.Web.UI.WebControls.Xml xml);

        /// <summary>
        /// 获得JSON串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string ToJSON(APermissionCollection value);
    }
}