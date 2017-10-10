using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using GCL.Common;
using GCL.IO.Config;
using GCL.Bean.Middler;
using GCL.Db;
using GCL.Project.VESH;
using GCL.Project.VESH.V.Control.Session;
using System.Collections.Generic;
namespace GCL.Project.VESH.V.View {
    /// <summary>
    /// C121221.1.3
    /// 主要用于公共类与鉴权
    /// 如果直接继承本页面可以在控件外自由使用&gt;% %&lt;等动态内容
    /// 用于用户扩展自定义的页面
    /// </summary>
    public abstract class APageBase : System.Web.UI.Page {

        internal IList<string> lstCss = new List<string>();
        internal IList<string> lstJS = new List<string>();
        /// <summary>
        /// 必须要实现以说明该页面所处的系统ID，用于配合多语言包使用。
        /// 其风格应受到Permission控制，但是这里不做特别的处理由用户重载进行处理
        /// </summary>
        public abstract string SystemID { get; }

        protected override void OnPreLoad(EventArgs e) {
            base.OnPreLoad(e);
            this.session = SessionDataManager.GetCurrentSessionDataManager();
        }
        /// <summary>
        /// 添加了各种语言包 UA Version信息等说明
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e) {
            try {
                //进行多语言包处理
                foreach (string pac in this.SessionData.MLang.GetPackages())
                    this.ClientScript.RegisterClientScriptInclude("MLang", this.VersionFormat(string.Format("{3}/scripts/MLang/{0}/{1}.js{{2}}", this.SessionData.MLang.Lang, pac, 0, Request.ApplicationPath)));
            } catch {
            }
            //添加UA信息 添加Version信息 供前台JS配合调用 重复引用因为VJ.form方法导致已修改
            this.ClientScript.RegisterStartupScript(this.GetType(), "bodyScript", string.Format("document.body.className = document.body.className + 'g_{0} g_{0}{1}';document.body.setAttribute('Version','{2}');document.body.setAttribute('MLang','{3}');", Request.Browser.Browser, Request.Browser.MajorVersion, this.Version, this.session.MLang.Lang), true);
            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer) {
            int i = 0;
            foreach (string cssurl in lstCss.Distinct<string>()) {
                HtmlGenericControl css = new HtmlGenericControl("link");
                css.Attributes["rel"] = "stylesheet";
                css.Attributes["type"] = "text/css";
                css.Attributes["href"] = cssurl;
                if (this.Header != null)
                    this.Header.Controls.Add(css);
                else { this.Controls.AddAt(i, css); i++; }
            }
            foreach (string jsurl in lstJS.Distinct<string>()) {
                HtmlGenericControl js = new HtmlGenericControl("script");
                js.Attributes["type"] = "text/javascript";
                js.Attributes["src"] = jsurl;
                this.Controls.AddAt(this.Controls.Count - 1, js);
            }
            base.Render(writer);
        }

        private SessionDataManager session;
        /// <summary>
        /// 会话管理器
        /// </summary>
        public SessionDataManager SessionData {
            get { return session; }
        }



        /// <summary>
        /// 获取统一版本号 请定义web.pcf中 SessionDataAdapter下
        /// &lt;Params xmlns="" name="Resource"&gt;
        ///  &lt;String name="name" value="version"/&gt;
        ///  &lt;object type=".V.Control.Session.Resource.ConstSessionResource" mode="static" method="constructor"&gt;
        ///    &lt;String value="value=20130123"/&gt;
        ///  &lt;/object&gt;
        /// &lt;/Params&gt;
        /// </summary>
        public string Version { get { return Convert.ToString(SessionData["version"]["value"]); } }

        /// <summary>
        /// 获取统一版本号 请定义web.pcf中 SessionDataAdapter下
        ///  &lt;Params xmlns="" name="Resource"&gt;
        ///  &lt;String name="name" value="version"/&gt;
        ///  &lt;object type=".V.Control.Session.Resource.ConstSessionResource" mode="static" method="constructor"&gt;
        ///    &lt;String value="value=20130123"/&gt;
        ///  &lt;/object&gt;
        /// &lt;/Params&gt;
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string VersionFormat(string format) {
            return string.Format(format, string.Format("{0}_={1}", format.IndexOf("?") >= 0 ? "&" : "?", SessionData["version"]["value"]));
        }

        private DataSet[] dss = null;
        /// <summary>
        /// 重载用于获得session中的DBResult的Value数组
        /// </summary>
        /// <returns></returns>
        protected DataSet[] GetDBResultArray() {
            if (dss == null) {
                dss = session.DBResult.Values.ToArray<DataSet>();
            }
            return dss;
        }

        /// <summary>
        /// 按序号获得session中的DBResult的DataSet的JSON
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetDBResultJson(int index) {
            return DBTool.ToJSON(GetDBResultArray()[index]);
        }

        /// <summary>
        /// 按序号获得session中的DBResult的首个DataSet的JSON
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetDBResultJson() {
            return DBTool.ToJSON(GetDBResultArray()[0]);
        }

        /// <summary>
        /// 按序号获得session中的DBResult的DataSet的JSON
        /// </summary>
        /// <param name="name">一般是请求对象加.请求的具体status的名字，譬如VESHTest.Module.abc.abc.content</param>
        /// <returns></returns>
        public string GetDBResultJson(string name) {
            return DBTool.ToJSON(session.DBResult[name]);
        }

        /// <summary>
        /// 按序号获得session中的DBResult的DataSet的TJSON
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetDBResultTJson(int index) {
            return DBTool.ToTJSON(GetDBResultArray()[index]);
        }

        /// <summary>
        /// 按序号获得session中的DBResult的首个DataSet的TJSON
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetDBResultTJson() {
            return DBTool.ToTJSON(GetDBResultArray()[0]);
        }
        /// <summary>
        /// 按序号获得session中的DBResult的DataSet的TJSON
        /// </summary>
        /// <param name="name">一般是请求对象加.请求的具体status的名字，譬如VESHTest.Module.abc.abc.content</param>
        /// <returns></returns>
        public string GetDBResultTJson(string name) {
            return DBTool.ToTJSON(session.DBResult[name]);
        }
    }
}
