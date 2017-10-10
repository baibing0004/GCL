using System;
using System.Collections;
using System.Collections.Specialized;
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
using GCL.Project.VESH.E.Module;
namespace GCL.Project.VESH.V.Control.NiParamFilter
{

    /// <summary>
    /// POST 时 选择Form,否则选择QueryString 不过滤参数数据容易出问题
    /// </summary>
    public class FormNiParamFiller : IHttpNiParamFilter
    {

        #region IHttpNiParamFilter Members

        public void Filt(HttpContext context, System.Collections.IDictionary idic)
        {
            if (context.Request.HttpMethod.Equals("get", StringComparison.InvariantCultureIgnoreCase)) Fill(context.Request.QueryString, idic);
            if (context.Request.HttpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase)) { Fill(context.Request.QueryString, idic); Fill(context.Request.Form, idic); }
        }

        public void Fill(NameValueCollection nc, System.Collections.IDictionary idic)
        {
            for (IEnumerator ie = nc.GetEnumerator(); ie.MoveNext(); )
            {
                string name = ie.Current.ToString();
                idic[name] = nc[name];
            }
        }


        #endregion
    }
}
