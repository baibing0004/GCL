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
namespace GCL.Project.VESH.V.Control.Session.Resource {
    /// <summary>
    /// 加密资源类代理只有此类为SessionResource管理
    /// </summary>
    public class SecuritySessionResourceDecorater : ISessionResource {

        private ISessionResource res;
        private IXcrypt xn;
        public SecuritySessionResourceDecorater(ISessionResource res, IXcrypt xn) {
            this.res = res;
            this.xn = xn;
        }

        /// <summary>
        /// 用于产生随机码
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateRandomCode() {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace(" ", "").Substring(0, 16);
        }


        #region ISessionResource Members

        public void Save(SessionData data, string text) {
            if (GCL.Common.Tool.IsEnable(text))
                this.res.Save(data, xn.Encrypt(text + "&.Code=" + GenerateRandomCode()));
        }

        public string Load(SessionData data) {
            string _data = this.res.Load(data);
            if (!GCL.Common.Tool.IsEnable(_data))
                return _data;
            _data = xn.Decrypt(_data);
            return _data.Substring(0, _data.LastIndexOf("&.Code="));
        }

        public void Clear(SessionData data) {
            this.res.Clear(data);
        }

        #endregion


        public SessionData CreateSessionData(string id, string name) {
            return this.res.CreateSessionData(id, name);
        }
    }
}
