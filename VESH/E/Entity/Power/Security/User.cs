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
    /// 登陆后需要存放的一系列数据
    /// </summary>
    public class User : SessionData {

        /// <summary>
        /// 默认的属性名
        /// </summary>
        public static readonly string SNAME = ".User";

        /// <summary>
        /// 用于定义一切通用的信息，比如UserID，SessionID，登陆时间等等
        /// </summary>
        public User() : base("", SNAME) { }

        /// <summary>
        /// 用来同域设置其SessionID
        /// </summary>
        new public string SessionID {
            get { return base.SessionID; }
            set {
                if (string.IsNullOrEmpty(this.id))
                    this.id = value;
                else
                    throw new InvalidOperationException("SessionID只能设置一次！");
            }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID {
            get { return this["UID"] as string; }
            set { this["UID"] = value; }
        }

        /// <summary>
        /// 系统ID
        /// </summary>
        public string SystemID {
            get { return this["SID"] as string; }
            set { this["SID"] = value; }
        }

        /// <summary>
        /// 对象ID
        /// </summary>
        public string EntityID {
            get { return this["EID"] as string; }
            set { this["EID"] = value; }
        }

        /// <summary>
        /// 角色串
        /// </summary>
        public string EIDS {
            get { return this["EIDS"] as string; }
            set { this["EIDS"] = value; }
        }

        /// <summary>
        /// 登陆时间
        /// </summary>
        public string LoginTime {
            get { return this["LT"] as string; }
            set { this["LT"] = value; }
        }

        /// <summary>
        /// 记录SessionID进入数据中
        /// </summary>
        /// <returns></returns>
        public override string Serialize() {
            if (string.IsNullOrEmpty(SessionID))
                throw new InvalidOperationException("必须设置SessionID!");
            //if (string.IsNullOrEmpty(UserID))
            //    throw new InvalidOperationException("必须设置UserID!");
            if (string.IsNullOrEmpty(LoginTime))
                LoginTime = DateTime.Now.ToString();
            return string.Format(".ID={0}&{1}", SessionID, base.Serialize());
        }

        /// <summary>
        /// 重新解析SessionID
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isClear"></param>
        public override void Deserialize(string data, bool isClear) {
            base.Deserialize(data, isClear);
            if (string.IsNullOrEmpty(SessionID))
                lock (this) {
                    if (string.IsNullOrEmpty(id))
                        SessionID = Convert.ToString(this[".ID"]);
                } else if (!string.IsNullOrEmpty((string)this[".ID"]) && !string.IsNullOrEmpty(SessionID) && !SessionID.Equals(this[".ID"]))
                throw new InvalidOperationException(string.Format("SessionID不同：{0}!={1}", SessionID, this[".ID"]));
            Remove(".ID");
        }
    }
}
