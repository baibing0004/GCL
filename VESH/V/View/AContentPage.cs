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
using GCL.IO.Config;
namespace GCL.Project.VESH.V.View {
    /// <summary>
    /// C121221.1.3
    /// 这里主要用于添加Menu控件。以创建普通页面
    /// 禁止页面上在控件外独立使用&lt;% %&gt;等动态内容，控件内可以使用如 &lt;%#DataBinder.Eval(Container.DataItem,"content")%&gt;
    /// </summary>
    public abstract class AContentPage : APage {
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            if (GCL.Common.Tool.IsEnable(MenuAscx))
                Page.Controls.AddAt(3, Page.LoadControl(MenuAscx));
        }

        /// <summary>
        /// 此属性用于定义需要Header内显示的公共资源控件地址
        /// </summary>
        public virtual string MenuAscx {
            get {
                return string.Format("{0}{1}", Request.ApplicationPath, string.Format(ConfigManagement.AppSettings(this.SessionData.Module.ConfigManager, "GCL.Project.VESH.V.View.AContentPage.MenuAscx"), SystemID));
            }
        }
    }
}
