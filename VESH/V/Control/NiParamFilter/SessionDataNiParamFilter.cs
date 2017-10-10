using System;
using System.Collections;
using System.Linq;
using System.Web;
using GCL.Project.VESH.V.Control.Session;
namespace GCL.Project.VESH.V.Control.NiParamFilter {
    /// <summary>
    /// 100225.1.5
    /// 用于提前处理会话数据会话    
    /// 规则填充@会话Data名_会话值名
    /// </summary>
    public class SessionDataNiParamFilter : IHttpNiParamFilter {

        private string[] sessionDataNames;
        /// <summary>
        /// 获取会话数据名
        /// </summary>
        /// <param name="sessionDataNames"></param>
        public SessionDataNiParamFilter(string[] sessionDataNames) {
            this.sessionDataNames = sessionDataNames;
        }


        #region SessionDataNiParamFilter Members

        public void Filt(HttpContext context, System.Collections.IDictionary idic) {
            if (sessionDataNames == null || sessionDataNames.Length == 0)
                return;
            SessionDataManager manager = SessionDataManager.GetCurrentSessionDataManager();
            foreach (string name in sessionDataNames)
                if (manager.UserInfo.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    Fill(manager.UserInfo, idic);
                else
                    Fill(manager[name], idic);
        }

        public void Fill(SessionData data, System.Collections.IDictionary idic) {
            foreach (DictionaryEntry entry in data) {
                idic[string.Format("{0}{1}_{2}", "", data.Name, entry.Key)] = entry.Value;

                idic[string.Format("{0}{1}_{2}", "", data.Name.Replace(".", "_"), entry.Key.ToString().Replace(".", "_"))] = entry.Value;
            }
        }
        #endregion
    }
}
