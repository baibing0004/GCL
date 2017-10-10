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
using GCL.Project.VESH.V.Control.Session;
using GCL.Project.VESH.V.Control;
using System.Xml;
using System.IO;

namespace GCL.Project.VESH.V.Control.Controler {

    /// <summary>
    /// 用于直接翻译成xml文件
    /// </summary>
    public class XmlControler : PageControler {

        public XmlControler(string paraname)
            : base(string.IsNullOrEmpty(paraname) ? "axml" : paraname, "xsl") {
        }

        #region IControler Members

        public override void Execute(HttpRequest request, HttpResponse response, HttpContext context, GCL.Project.VESH.V.Control.Session.SessionDataManager session) {
            string path = GetResourcePath(request, context, session, this.extend);
            if (!File.Exists(context.Server.MapPath(path)))
                path = GetResourcePath(request, context, session, "xslt");
            try {
                //声明XslTransform类实例
                System.Xml.Xsl.XslCompiledTransform trans = new System.Xml.Xsl.XslCompiledTransform();

                using (StreamReader rdr = new StreamReader(path)) {
                    using (XmlReader xmlRdr = XmlReader.Create(rdr)) {
                        //载入xsl文件
                        trans.Load(xmlRdr);
                    }
                }
                response.Clear();
                response.ContentEncoding = System.Text.Encoding.UTF8;
                response.ContentType = GCL.Net.MIME.xml;
                response.Expires = 0;

                trans.Transform(
                    XmlReader.Create(new StringReader(session.DBResult.Last().Value.GetXml())),
                    XmlWriter.Create(response.Output)
                    );

            } catch (System.Threading.ThreadAbortException) {
            } catch (HttpException ex) {
                if (ex.GetHttpCode() == 404) throw ex;
                throw new Exception(string.Format("GCL.Project.VESH.V.Control.Controler.XmlControler\r\nNext URL:{0}\r\nInnerException:{1}", path, ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString()));
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion
    }
}
