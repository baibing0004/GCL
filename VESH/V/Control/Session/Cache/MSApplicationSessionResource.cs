using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using GCL.Project.VESH.V.Control.Session.Resource;
namespace GCL.Project.VESH.V.Control.Session.Cache {
    /// <summary>
    /// C121221
    /// 使用微软Application对象进行的全局数据缓存
    /// </summary>
    public class MSApplicationSessionResource : ISessionResource {

        /// <summary>
        /// 针对需要针对SessionID进行缓存的对象 譬如MLang缓存
        /// </summary>
        private bool useID;
        public MSApplicationSessionResource(bool sessionCache) { this.useID = sessionCache; }
        public MSApplicationSessionResource() : this(false) { }

        protected string GenKey(SessionData data) {
            return string.Format("_Global.{0}.{1}", this.useID ? data.SessionID : "", data.Name);
        }

        #region ISessionResource Members

        public void Save(SessionData data, string text) {
            if (HttpContext.Current.Application[this.GenKey(data)] == null)
                HttpContext.Current.Application.Add(this.GenKey(data), text);
            else
                HttpContext.Current.Application.Set(this.GenKey(data), text);
        }

        public string Load(SessionData data) {
            return Convert.ToString(HttpContext.Current.Application[this.GenKey(data)]);
        }

        public void Clear(SessionData data) {
            HttpContext.Current.Application.Remove(this.GenKey(data));
        }

        #endregion


        public SessionData CreateSessionData(string id, string name) {
            return new SessionData(id, name);
        }
    }
}
