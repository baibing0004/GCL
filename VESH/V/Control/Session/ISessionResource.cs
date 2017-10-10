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

namespace GCL.Project.VESH.V.Control.Session {
    /// <summary>
    /// C121221.1.1
    /// SessionData存取数据源
    /// </summary>
    public interface ISessionResource {
        /// <summary>
        /// 用于保存数据
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        void Save(SessionData data, string text);

        /// <summary>
        /// 用于获取数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        string Load(SessionData data);

        /// <summary>
        /// 用于清除数据
        /// </summary>
        /// <param name="context"></param>
        void Clear(SessionData data);

        /// <summary>
        /// 用于生成SessionData同时很可能生成缓存用
        /// TODO 生成专有SessionData譬如MonoDB或者Redis或者Memcached使用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        SessionData CreateSessionData(string id, string name);
    }
}
