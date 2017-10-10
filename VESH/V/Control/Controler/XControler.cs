using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GCL.Project.VESH.V.Control.Controler {
    /// <summary>
    /// 用于处理Action完全负责Responsed的情况，不统一进行任何处理。
    /// </summary>
    public class XControler : IControler {
        public XControler(string param) {
            this.ParaName = string.IsNullOrEmpty(param) ? "x" : param;
        }

        public void Execute(HttpRequest request, HttpResponse response, HttpContext context, Session.SessionDataManager session) {

        }

        public string ParaName {
            get;
            private set;
        }

        public void Dispose() {
        }
    }
}