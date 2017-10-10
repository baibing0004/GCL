using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using GCL.Project.VESH.V.Control.Session.Resource;

namespace GCL.Project.VESH.V.Control.Session.Cache {

    /// <summary>
    /// C121221
    /// 使用微软Cache进行缓存对象。
    /// </summary>
    /// <param name="setter"></param>        
    public class MSCacheSessionResource : ISessionResource {

        /// <summary>
        /// 用于进行复杂的Cache设置
        /// </summary>
        public interface ICacheSetter {
            void SetCache(System.Web.Caching.Cache cache, string key, string v);
        }

        /// <summary>
        /// 默认0/12时Cache过期
        /// </summary>
        private class _DefaultCacheSetter : ICacheSetter {

            #region ICacheSetter Members

            public void SetCache(System.Web.Caching.Cache cache, string key, string v) {
                cache.Add(key, v, null
                    , DateTime.Now.Date.AddHours(12)
                    , TimeSpan.Zero
                    , CacheItemPriority.Normal
                    , null);
            }

            #endregion

            public static _DefaultCacheSetter Instance = new _DefaultCacheSetter();
        }

        private ICacheSetter setter;

        public MSCacheSessionResource(ICacheSetter setter) { this.setter = setter; }
        public MSCacheSessionResource() : this(_DefaultCacheSetter.Instance) { }
        private string GenKey(SessionData data) {
            return data.SessionID + ":" + data.Name;
        }

        #region ISessionResource Members

        public void Save(SessionData data, string text) {
            this.setter.SetCache(HttpRuntime.Cache, this.GenKey(data), text);
        }

        public string Load(SessionData data) {
            return Convert.ToString(HttpRuntime.Cache.Get(this.GenKey(data)));
        }

        public void Clear(SessionData data) {
            HttpRuntime.Cache.Remove(this.GenKey(data));
        }

        #endregion


        public SessionData CreateSessionData(string id, string name) {
            return new SessionData(id, name);
        }
    }
}
