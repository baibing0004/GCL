using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using GCL.Project.VESH.E.Module;
using GCL.Project.VESH.V.Control.Session;
using GCL.Project.VESH.E.Entity;
using GCL.Project.VESH.E.Entity.Power.Security;
using GCL.Project.VESH.E.Entity.Power.Permission;

namespace GCL.Project.VESH.V.View.Theme.Back {


    public partial class Menu : System.Web.UI.UserControl {

        //protected override void OnPreRender(EventArgs e) {
        //    proxy = AModule.Root.MenuProvider;
        //    base.OnPreRender(e);
        //}

        //private APermissionCollection PermissionCollection {
        //    get { return SessionDataManager.GetCurrentSessionDataManager().PermissionData.PermissionCollection; }
        //}
        //private IMenuProvider proxy;
        //public IMenuProvider MenuItem {
        //    get { return proxy; }
        //}

        //public void BindMenu(System.Web.UI.WebControls.Menu menu) {
        //    proxy.BindMenu(PermissionCollection, menu);
        //}

        //public void BindXML(System.Web.UI.WebControls.Xml xml) {
        //    proxy.BindXML(PermissionCollection, xml);
        //}

        //public string ToJSON() {
        //    return proxy.ToJSON(PermissionCollection);
        //}

        //public string ToXML() {
        //    return proxy.ToXML(PermissionCollection);
        //}

        //public System.Xml.XmlDocument ToXMLDocument() {
        //    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        //    doc.LoadXml(ToXML());
        //    return doc;
        //}

    }
}