using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GCL.Module.Trigger;
using GCL.Project.VESH.V.Control.Session.Resource;
namespace GCL.Project.VESH.V.Control.Session.Cache {

    /// <summary>
    /// C121221 会话源数据责任链模式装饰器
    /// </summary>
    public class LineSessionResourceDecorator : ISessionResource {

        private ISessionResource[] srs;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="firstSR"></param>
        /// <param name="secSR"></param>
        public LineSessionResourceDecorator(ISessionResource[] srs) {
            this.srs = srs;
        }

        #region ISessionResource Members

        public void Save(SessionData data, string text) {
            foreach (ISessionResource ir in srs)
                try {
                    ir.Save(data, text);
                    return;
                } catch {
                }
            throw new Exception("所有数据保存措施都失败!");
        }

        public string Load(SessionData data) {
            foreach (ISessionResource ir in srs)
                try {
                    string v = ir.Load(data);
                    if (!string.IsNullOrEmpty(v))
                        return v;
                } catch { }
            throw new Exception("所有数据提取措施都失败!");
        }

        public void Clear(SessionData data) {
            foreach (ISessionResource ir in srs)
                try {
                    ir.Clear(data);
                } catch { }
        }
        #endregion


        public SessionData CreateSessionData(string id, string name) {
            return new SessionData(id,name);
        }
    }
}
