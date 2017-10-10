using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GCL.Project.VESH.V.Control {
    /// <summary>
    /// 用于处理page/JSON/JSONP/TJSON/TJSONP/XML/xlst等方式的页面转换参数处理器
    /// </summary>
    public interface IControler : IDisposable {

        /// <summary>
        /// 在Handler处理完成后调用Excute进行跳转
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="context"></param>
        /// <param name="session"></param>
        void Execute(HttpRequest request, HttpResponse response, HttpContext context, Session.SessionDataManager session);

        /// <summary>
        /// 说明其对应处理的参数名 譬如 page
        /// </summary>
        string ParaName { get; }
    }
}
