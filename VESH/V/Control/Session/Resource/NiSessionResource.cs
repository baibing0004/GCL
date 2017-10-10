using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GCL.Bean.Middler;
using GCL.IO.Config;
using GCL.Db.Ni;
using System.Collections;

namespace GCL.Project.VESH.V.Control.Session.Resource {
    /// <summary>
    /// C121221.1.1
    /// SessionData的DB存放方式
    /// 参数主要有 @SessionID,@data（仅在save时提供)
    /// </summary>
    public class NiSessionResource : ISessionResource {

        private string niName;
        private string niObjectName;
        private Middler middler;
        private string savecommand;
        private string loadcommand;
        private string clearcommand;

        public NiSessionResource(ConfigManager cm, string NiApp, string NiName, string saveCommand, string loadCommand, string clearCommand) {
            this.middler = new Middler(cm);
            this.niName = NiApp;
            this.niObjectName = NiName;
            this.savecommand = saveCommand;
            this.loadcommand = loadCommand;
            this.clearcommand = clearCommand;
        }

        protected virtual string GetTable(string command, IDictionary idic) {
            NiTemplate template = this.middler.GetObjectByAppName(this.niName, this.niObjectName) as NiTemplate;
            NiDataResult result;
            try {
                result = template.ExcuteQuery(command, idic);
                return Convert.ToString(result.GetFirstCell());
            } finally {
                middler.SetObjectByAppName(this.niName, this.niObjectName, template);
            }
        }

        public void Save(SessionData data, string text) {
            GetTable(this.savecommand, new Hashtable() { { "@SessionID", data.SessionID }, { "@data", text } });
        }

        public string Load(SessionData data) {
            return GetTable(this.loadcommand, new Hashtable() { { "@SessionID", data.SessionID } });
        }

        public void Clear(SessionData data) {
            GetTable(this.clearcommand, new Hashtable() { { "@SessionID", data.SessionID } });
        }


        public SessionData CreateSessionData(string id, string name) {
            return new SessionData(id, name);
        }
    }
}