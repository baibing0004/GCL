using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using GCL.Common;
using GCL.Project.VESH.V.Control.Session;
namespace GCL.Project.VESH.E.Entity.MLang {

    /// <summary>
    /// 用于处理多语言包
    /// </summary>
    public class MLanguage {

        private SessionDataAdapter adapter;
        private IDictionary idic;
        private IDictionary nv;
        public MLanguage(SessionDataAdapter adapter) {
            this.adapter = adapter;
            this.nv = new System.Collections.Generic.Dictionary<string,SessionData>();
            this.idic = new Hashtable();
        }

        private string lang;
        /// <summary>
        /// 设定获取当前语言
        /// throws
        ///     NullReferenceException 尚未设定语言包
        /// </summary>
        public string Lang {
            get {
                if (string.IsNullOrEmpty(this.lang)) throw new NullReferenceException("尚未设定语言包");
                return this.lang;
            }
            set {
                this.lang = value;
            }
        }

        public void Include(string package) {
            lock (idic) {
                if (!this.idic.Contains(package)) {
                    idic[package] = this.adapter.GetSessionData(this.Lang + "." + package, "GCL.Project.VESH.E.Entity.MLang.MLanguage");
                    foreach (IDictionaryEnumerator kv in idic[package] as SessionData)
                        this.nv.Add(kv.Key, kv.Value);
                }
            }
        }

        /// <summary>
        /// 获取引用的包名数组
        /// </summary>
        /// <returns></returns>
        public string[] GetPackages() {
            lock (idic) {
                string[] ret = new string[idic.Count];
                idic.Keys.CopyTo(ret, 0);
                return ret;
            }
        }
        /// <summary>
        /// 获取与本次登陆相关SessionData
        /// 其不同Name的SessionData可以配置SessionDataAdapter中对应不同的存取途径。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name] {
            get {
                return this.nv[name] as string;
            }
        }

        //web多语言方式的渲染使用主要依靠代码的调用而不依靠遍历控件属性方式处理，另外其MLang属性也不能实现一个控件的多个属性赋值
    }
}

