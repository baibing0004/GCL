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
    /// 主要用于添加标题头与标题脚注
    /// 用于创建导航页面
    /// 禁止页面上在控件外独立使用&lt;% %&gt;等动态内容，控件内可以使用如 &lt;%#DataBinder.Eval(Container.DataItem,"content")%&gt;
    /// </summary>
    public abstract class APage : AScriptPage {
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            if (GCL.Common.Tool.IsEnable(HeaderAscx))
                Page.Controls.AddAt(2, Page.LoadControl(HeaderAscx));
            if (GCL.Common.Tool.IsEnable(FooterAscx))
                Page.Controls.AddAt(Page.Controls.Count - 1, Page.LoadControl(FooterAscx));
        }

        /// <summary>
        /// 此属性用于定义需要Header内显示的公共资源控件地址
        /// /Module/Header.ascx
        /// </summary>
        public virtual string HeaderAscx {
            get {
                return string.Format("{0}{1}", Request.ApplicationPath, string.Format(ConfigManagement.AppSettings(this.SessionData.Module.ConfigManager, "GCL.Project.VESH.V.View.APage.HeaderAscx"), SystemID));
            }
        }

        /// <summary>
        ///此属性用于定义需要页面底部显示的公共资源控件地址
        /// /Module/Footer.ascx
        /// </summary>
        public virtual string FooterAscx {
            get {
                return string.Format("{0}{1}", Request.ApplicationPath, string.Format(ConfigManagement.AppSettings(this.SessionData.Module.ConfigManager, "GCL.Project.VESH.V.View.APage.FooterAscx"), SystemID));
            }
        }
    }
}
