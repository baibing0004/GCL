using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GCL.Module.Trigger;
using GCL.Project.VESH.V.Control.Session.Resource;
namespace GCL.Project.VESH.V.Control.Session.Cache {

    /// <summary>
    /// C121221 会话源数据装饰器
    /// </summary>
    public class SessionResourceDecorator : ISessionResource {

        private ISessionResource cacheSR;
        private ISessionResource realSR;
        private ATrigger trigger;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cacheSR"></param>
        /// <param name="realSR"></param>
        public SessionResourceDecorator(ISessionResource cacheSR, ISessionResource realSR, ATrigger trigger) {
            this.cacheSR = cacheSR;
            this.realSR = realSR;
            this.trigger = trigger;
        }

        #region ISessionResource Members

        public void Save(SessionData data, string text) {
            try {
                this.realSR.Save(data, text);
            } finally {
                if (data.IsCleanCache())
                    this.cacheSR.Clear(data);
                else
                    this.cacheSR.Save(data, text);
            }
        }

        public string Load(SessionData data) {
            string v = this.cacheSR.Load(data);
            if (string.IsNullOrEmpty(v) || (trigger != null && trigger.Taste())) {
                v = this.realSR.Load(data);
                this.cacheSR.Clear(data);
                if (!string.IsNullOrEmpty(v))
                    this.cacheSR.Save(data, v);
            }
            return v;
        }

        public void Clear(SessionData data) {
            try {
                this.realSR.Clear(data);
            } finally {
                this.cacheSR.Clear(data);
            }
        }

        #endregion


        public SessionData CreateSessionData(string id, string name) {
            return realSR.CreateSessionData(id, name);
        }
    }
}
