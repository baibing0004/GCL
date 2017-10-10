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
using GCL.Project.VESH.V.Control.Session;

namespace GCL.Project.VESH.V.Control.Session {
    /// <summary>
    /// C121221.1.1
    /// 用于从资源中读取SessionData
    /// 这里对Cookie资源的存取需要另外处理
    /// </summary>
    public class SessionDataAdapter {

        private IDictionary idic;
        private ISessionResource res;
        public SessionDataAdapter(IDictionary idic) {
            this.idic = idic;
        }

        public SessionDataAdapter() : this(new Hashtable()) { }

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ISessionResource GetResource(SessionData data) {
            if (idic.Contains(data.Name))
                return idic[data.Name] as ISessionResource;
            else
                return res;
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ISessionResource GetResource(string Name) {
            if (idic.Contains(Name))
                return idic[Name] as ISessionResource;
            else
                return res;
        }

        /// <summary>
        /// 设置Resource 名字与对象
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="res">资源</param>
        public void SetResource(string name, ISessionResource res) {
            idic[name] = res;
        }

        /// <summary>
        /// 设置默认Resource
        /// </summary>
        /// <param name="res">资源</param>
        public void SetResource(ISessionResource res) {
            this.res = res;
        }

        /// <summary>
        /// 获取SessionData 默认清空SessionData 信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        public void GetSessionData(SessionData data) {
            try {
                var text = this.GetResource(data).Load(data);
                if (!string.IsNullOrEmpty(text))
                    data.Deserialize(text);
            } catch (Exception ex) {
                System.Console.Error.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 获取SessionData
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="isClear"></param>
        public void GetSessionData(SessionData data, bool isClear) {
            try {
                var text = this.GetResource(data).Load(data);
                if (!string.IsNullOrEmpty(text))
                    data.Deserialize(text, isClear);
            } catch (Exception ex) {
                System.Console.Error.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 获取SessionData
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public SessionData GetSessionData(string id, string name) {
            SessionData data = this.GetResource(name).CreateSessionData(id, name);
            GetSessionData(data);
            return data;
        }

        public void SaveSessionData(SessionData data) {
            this.GetResource(data).Save(data, data.Serialize());
        }

        public void ClearSessionData(SessionData data) {
            this.GetResource(data).Clear(data);
        }
    }
}
