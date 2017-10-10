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
    /// 主要用于在页面Header内与Html外设置公共Script代码内容,其引用的用户控件ascx路径由分站点web.pcf中HeaderScriptAscx、FooterScriptAscx定义,如果不定义就自动复用根目录下相关定义内容
    /// 如果直接继承本页且设置分站点web.pcf中HeaderScriptAscx、FooterScriptAscx为空字符串，那么可以在控件外自由使用&gt;% %&lt;等动态内容
    /// 否则禁止页面上在控件外独立使用&gt;% %&lt;等动态内容，控件内可以使用如 &gt;%#DataBinder.Eval(Container.DataItem,"content")%&lt等绑定信息;
    /// 主要用于不可见页面编辑!
    /// </summary>
    public abstract class AScriptPage : APageBase {

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            if (GCL.Common.Tool.IsEnable(HeaderScriptAscx)) {
                int i = 0;
                foreach (Object control in Header.Controls) {
                    if (control is HtmlLink) break; else i++;
                }
                if (i <= Header.Controls.Count)
                    Header.Controls.AddAt(i, Page.LoadControl(HeaderScriptAscx));
                else
                    Header.Controls.Add(Page.LoadControl(HeaderScriptAscx));
            }

            if (GCL.Common.Tool.IsEnable(FooterScriptAscx)) {
                int i = 1;
                foreach (Object control in Page.Controls) {
                    if (control is HtmlForm) break; else i++;
                }
                Page.Controls.AddAt(i, Page.LoadControl(FooterScriptAscx));
            }
        }

        /// <summary>
        /// 此属性用于定义需要Header内显示的公共资源控件地址
        /// </summary>
        public virtual string HeaderScriptAscx {
            get {
                return string.Format("{0}{1}", Request.ApplicationPath, string.Format(ConfigManagement.AppSettings(this.SessionData.Module.ConfigManager, "GCL.Project.VESH.V.View.AScriptPage.HeaderScriptAscx"), SystemID));
            }
        }

        /// <summary>
        ///此属性用于定义需要页面底部显示的公共资源控件地址
        /// </summary>
        public virtual string FooterScriptAscx {
            get {
                return string.Format("{0}{1}", Request.ApplicationPath, string.Format(ConfigManagement.AppSettings(this.SessionData.Module.ConfigManager, "GCL.Project.VESH.V.View.AScriptPage.FooterScriptAscx"), SystemID));
            }
        }

    }
}
