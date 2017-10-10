using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GCL.Project.VESH.V.Control.Controler {
    public class VoidControler : IControler {
        private string paraName;

        /// <summary>
        /// 默认参数名为_page
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="extend"></param>
        public VoidControler(string paraName) {
            this.paraName = string.IsNullOrEmpty(paraName) ? "void" : paraName;
        }
        public void Execute(HttpRequest request, HttpResponse response, HttpContext context, Session.SessionDataManager session) {
            response.Clear();
            response.ContentType = GCL.Net.MIME.js;
            response.Write("[[1]]");
            response.Expires = 0;
        }

        public string ParaName {
            get { return this.paraName; }
        }

        public void Dispose() {
        }
    }
}