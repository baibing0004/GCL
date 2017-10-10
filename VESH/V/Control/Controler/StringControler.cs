using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GCL.Project.VESH.V.Control.Session;

namespace GCL.Project.VESH.V.Control.Controler {
    public class StringControler : IControler {
        private string paraName;
        public StringControler(string paraName) {
            this.paraName = string.IsNullOrEmpty(paraName) ? "string" : paraName;
        }

        public void Execute(HttpRequest request, HttpResponse response, HttpContext context, SessionDataManager session) {
            try {
                //一般很少有超出1个DataSet的情况
                response.Clear();
                response.Write(session.Status);
                response.ContentType = GCL.Net.MIME.js;
                response.Expires = 0;
            } catch (Exception ex) {
                response.Clear();
                response.Write("[False]");
                session.Logger.Error(ex.ToString());
            }
        }

        public string ParaName {
            get { return this.paraName; }
        }

        public void Dispose() {
        }
    }
}