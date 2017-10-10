using System;
using System.Data;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using GCL.Db.Ni;
using GCL.Project.VESH.V.Control.Session;

namespace GCL.Project.VESH.E.Entity.Power.Permission.AS {

    /// <summary>
    /// ASPermissionFactory的简单实现! 可根据业务需求进行进一步封装!
    /// </summary>
    public class ASPerColFactory : IPermissionFactory {
        NameValueCollection nvs;
        /// <summary>
        /// 由外部实现nvs的实例化和初始化，并装载进入ASPermissionFactory
        /// </summary>
        /// <param name="nvs"></param>
        public ASPerColFactory(NameValueCollection nvs) { this.nvs = nvs; }

        /// <summary>
        /// 根据Permission.js文件进行初始化操作
        /// </summary>
        /// <param name="filePath"></param>
        public ASPerColFactory(string filePath)
            : this(new NameValueCollection()) {
            HttpContext context = HttpContext.Current;

            foreach (string s in System.IO.File.ReadAllText(context.Server.MapPath(filePath)).Remove(0, "var pers={".Length).TrimEnd(';', '}').Split(',')) {
                string[] con = s.Split(':');
                this.nvs[con[0].Trim()] = con[1].Trim();
            }
        }

        /// <summary>
        /// 根据SessionDataManager，template，command获得数据初始化操作。
        /// </summary>
        /// <param name="sdm"></param>
        /// <param name="template"></param>
        /// <param name="command"></param>
        public ASPerColFactory(NiTemplate template, string command)
            : this(new NameValueCollection()) {
                try {
                    DataTable dt = template.ExcuteQuery(command).DataSet.Tables[0];
                    foreach (DataRow row in dt.Rows) {
                        this.nvs[Convert.ToString(row["Name"])] = Convert.ToString(row["ID"]);
                    }
                } finally{
                    template.Dispose();
                }
        }

        /// <summary>
        /// 重置内存中的字典对象
        /// </summary>
        /// <param name="nvc"></param>
        public void Reset(NameValueCollection nvc) { 
            lock(this.nvs){
                NameValueCollection _nv = this.nvs;
                this.nvs = nvc;
                _nv.Clear();
                _nv = null;
            }
        }

        /// <summary>
        /// 通过DBTable 进行字典重置
        /// </summary>
        /// <param name="dt"></param>
        public void Reset(DataTable dt) {
            NameValueCollection nc = new NameValueCollection();
            foreach (DataRow row in dt.Rows) {
                nc[Convert.ToString(row["Name"])] = Convert.ToString(row["ID"]);
            }
            Reset(nc);
        }

        public void Reset(string filePath) {
            HttpContext context = HttpContext.Current;
            NameValueCollection nc = new NameValueCollection();
            foreach (string s in System.IO.File.ReadAllText(context.Server.MapPath(filePath)).Remove(0, "var pers={".Length).TrimEnd(';', '}').Split(',')) {
                string[] con = s.Split(':');
                nc[con[0].Trim()] = con[1].Trim();
            }
            Reset(nc);
        }

        #region IPermissionFactory Members

        public APermissionCollection CreatePermmisonCollection(string ids) {
            return new ASPermissionCollection(ids, this.nvs);
        }

        #endregion
    }
}
