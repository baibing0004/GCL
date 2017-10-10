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
using System.Collections.Generic;

namespace GCL.Project.VESH.V.View {
    public abstract class AUserPart : UserControl {

        protected APageBase pbase;
        protected override void OnLoad(EventArgs e) {
            //这里对不是APageBase的页面不予显示，防止控件出现错误!
            if (this.Page is APageBase)
                this.pbase = this.Page as APageBase;
            else
                this.Visible = false;
            base.OnLoad(e);
        }

        /// <summary>
        /// 必须要实现以说明该页面所处的系统ID，用于配合多语言包使用。
        /// </summary>
        protected string SystemID { get { return pbase.SystemID; } }

        /// <summary>
        /// 判断是否有权限显示 一般是this.SessionData.Check()实现
        /// </summary>
        public abstract bool HasRight { get; }

        /// <summary>
        /// 会话管理器
        /// </summary>
        public SessionDataManager SessionData {
            get { return pbase.SessionData; }
        }

        /// <summary>
        /// 设置特有的数据集合，并打开填充控件的datas属性的开关
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ds"></param>
        protected void RenderData(string key, DataSet ds) {
            this.SessionData.DBResult[key] = ds;
            this.IsRenderData = true;
        }

        /// <summary>
        /// 设置是否可以填充默认数据
        /// </summary>
        public bool IsRenderData { get; set; }

        /// <summary>
        /// 重载以实现数据绑定 一般当使用part方式时引用
        /// </summary>
        /// <param name="writer"></param>
        protected override void OnPreRender(EventArgs e) {
            if (this.IsRenderData) {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                try {
                    #region 记录输出结果
                    sb.Append("{");
                    if (this.SessionData.DBResult.Count > 0) {
                        foreach (System.Collections.Generic.KeyValuePair<string, System.Data.DataSet> pair in this.SessionData.DBResult) { sb.Append(pair.Key).Append(":"); GCL.Db.DBTool.ToTJSON(pair.Value, sb); sb.Append(","); }
                        sb.Remove(sb.Length - 1, 1);
                    }
                    sb.Append("}");
                    this.Attributes["DBResult"] = sb.ToString();
                    #endregion
                } finally {
                    sb.Remove(0, sb.Length);
                    sb = null;
                }
            }
            if (this.Visible && !this.HasRight) this.Visible = false;
            if (this.Visible) {
                foreach (string css in lstCss)
                    pbase.lstCss.Add(css);
                foreach (string css in CSS.Split(';'))
                    if (!string.IsNullOrEmpty(css)) pbase.lstCss.Add(css);
                foreach (string js in lstJS)
                    pbase.lstJS.Add(js);
                foreach (string js in JS.Split(';'))
                    if (!string.IsNullOrEmpty(js)) pbase.lstJS.Add(js);
            }

            base.OnPreRender(e);
        }

        private IList<string> lstCss = new List<string>();
        public IList<string> CSSInclude { get { return lstCss; } }
        private string _css = "";
        public string CSS { get { return _css; } set { this._css = value; } }

        private IList<string> lstJS = new List<string>();
        public IList<string> JSInclude { get { return lstJS; } }
        private string _js = "";
        public string JS { get { return _js; } set { this._js = value; } }
    }
}
