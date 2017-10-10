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
namespace GCL.Project.VESH.E.Entity.Power.Security {
    /// <summary>
    /// C121221.1.1
    /// 用于管理需要严格认证加密的数据
    /// </summary>
    public class SpecialData : SessionData {
        /// <summary>
        /// USER加密串的Key
        /// </summary>
        public static readonly string SCODENAME = ".C";
        private User user;
        public SpecialData(User user, string name) : base(user.SessionID, name) { this.user = user; }

        /// <summary>
        /// 用于外部校验使用
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static string GetMD5HashCode(string source) {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(source, "MD5");
        }

        public override string Serialize() {
            this[SCODENAME] = GetMD5HashCode(string.Format("{0}{1}{2}",user.UserID,user.EntityID,user.LoginTime));
            return base.Serialize();
        }

        public override void Deserialize(string data, bool isClear) {
            base.Deserialize(data, isClear);
            if (!string.IsNullOrEmpty(this.Code))
                //不是未登录
                if (!GetMD5HashCode(string.Format("{0}{1}{2}",user.UserID,user.EntityID,user.LoginTime)).Equals(this.Code))
                    throw new Exception("用户信息不同，该配置信息错误!");
        }

        public virtual void Init(string data) {
            base.Deserialize(data, true);
        }

        /// <summary>
        /// User加密串
        /// </summary>
        public string Code {
            get { return this[SCODENAME] as string; }
        }
    }
}
