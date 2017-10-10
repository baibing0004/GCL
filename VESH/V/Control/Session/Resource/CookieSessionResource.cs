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
using GCL.Common;

namespace GCL.Project.VESH.V.Control.Session.Resource {
    /// <summary>
    /// C121221.1.1
    /// SessionData的Cookie存放方式 中文或者存放特殊文字的Cookie
    /// </summary>
    public class CookieSessionResource : ISessionResource {
        private string domain;
        private string defaultValue = "";
        //排除IE6下同一域名超出4K即报错的问题。
        private bool isExceptIE = true;

        /// <summary>
        /// 其分别用于设置Cookie
        /// </summary>
        public CookieSessionResource(bool isExceptIE) : this(isExceptIE, "") { }

        /// <summary>
        /// 其分别用于设置Cookie，范围/值
        /// </summary>
        /// <param name="domain"></param>
        public CookieSessionResource(bool isExceptIE, string domain) : this(isExceptIE, domain, "") { }

        /// <summary>
        /// 其分别用于设置Cookie，范围/值
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="defaultValue"></param>
        public CookieSessionResource(bool isExceptIE, string domain, string defaultValue) {
            this.isExceptIE = isExceptIE;
            this.domain = domain;
            this.defaultValue = defaultValue;
        }

        /// <summary>
        /// 可以用来设置过期时间，默认为0时 无效
        /// </summary>
        public long ExpiresSecends { get; set; }

        #region ISessionResource Members

        public void Save(SessionData data, string text) {
            if (System.Web.HttpUtility.UrlEncodeToBytes(Convert.ToString(text)).Length >= (this.isExceptIE && HttpContext.Current.Request.Browser.Type.ToLower().StartsWith("ie") ? 1096 : 4000)) {
                this.Clear(data);
                return;
            }
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = new HttpCookie(data.Name, text);
            if (!string.IsNullOrEmpty(domain))
                cookie.Domain = domain;
            if (this.ExpiresSecends > 0)
                cookie.Expires = DateTime.Now.AddSeconds(ExpiresSecends);
            if (context.Response.Cookies.Get(data.Name) != null)
                context.Response.Cookies.Remove(data.Name);
            context.Response.Cookies.Add(cookie);
        }

        public string Load(SessionData data) {
            HttpContext context = HttpContext.Current;

            return Tool.GetValue(context.Response.Cookies.AllKeys.Contains<string>(data.Name) ? context.Response.Cookies[data.Name].Value :
                context.Request.Cookies.AllKeys.Contains<string>(data.Name) ? context.Request.Cookies[data.Name].Value : this.defaultValue, this.defaultValue);
        }

        public void Clear(SessionData data) {
            HttpContext context = HttpContext.Current;
            HttpCookie cookie = new HttpCookie(data.Name, "");
            if (!string.IsNullOrEmpty(domain))
                cookie.Domain = domain;
            cookie.Expires = DateTime.Now;
            if (context.Response.Cookies.Get(data.Name) != null)
                context.Response.Cookies.Remove(data.Name);
            context.Response.Cookies.Add(cookie);
            data.Deserialize("");
        }

        #endregion



        public SessionData CreateSessionData(string id, string name) {
            return new SessionData(id, name);
        }
    }
}