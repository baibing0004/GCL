using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace GCL.Project.VESH.V.Control {
    /// <summary>
    /// 为用户所实现的用于说明类的可用身份和缓存以调用其PreLoad方法的接口对象
    /// 需要其实现的方法名如下 public void DOXXXX(HttpRequest request, HttpResponse response, HttpContext context, Session.SessionDataManager session);
    /// </summary>
    public interface IAction : IDisposable {

        /// <summary>
        /// 一般用于参数初始化
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="context"></param>
        /// <param name="session"></param>
        void PreLoad(HttpRequest request, HttpResponse response, HttpContext context, Session.SessionDataManager session);
        /// <summary>
        /// 用于说明是否是静态的! 只有静态的可以缓存和公用!
        /// </summary>
        bool IsStatic { get; }
    }
}
