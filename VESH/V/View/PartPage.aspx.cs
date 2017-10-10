using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace GCL.Project.VESH.V.View {
    public partial class PartPage : GCL.Project.VESH.V.View.APageBase {
        //todo 未实现前台页面的 UID初始化 数据绑定
        //todo 未验证UserControl 是否可行!
        protected void Page_Load(object sender, EventArgs e) {
            //加载控件 进行显示!
            string path = this.GetControlPath();
            System.Web.UI.Control control = this.GetUserControl(path);
            if (control is GCL.Project.VESH.V.View.AUserPart) { ((GCL.Project.VESH.V.View.AUserPart)control).IsRenderData = true; }
            this.Controls.Clear();
            this.Controls.Add(control);
        }

        /// <summary>
        /// 获取控件路径
        /// </summary>
        /// <returns></returns>
        protected virtual string GetControlPath() {
            if (this.SessionData.Status.EndsWith(".ascx", StringComparison.CurrentCultureIgnoreCase)) {
                return this.SessionData.Status;
            } else {
                //如果Status为空值，那么不作任何处理
                if (!string.IsNullOrEmpty(this.SessionData.Status)) throw new Exception(string.Format("没有根据会话状态{0}找到确定的控件路径！", this.SessionData.Status));
                else throw new Exception("会话状态为空!");
            }
        }

        protected virtual System.Web.UI.Control GetUserControl(string controlPath) {
            System.Web.UI.Control control = this.LoadControl(controlPath);
            //自动判断缓存
            if (control is PartialCachingControl) {
                control = ((PartialCachingControl)control).CachedControl;
            }
            return control;
        }

        public override string SystemID {
            get { return SessionData.UserInfo.SystemID; }
        }
    }
}
