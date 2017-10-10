using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Text;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace GCL.Project.VESH.V.Control.Session {
    /// <summary>
    /// C121221.1.1
    /// 用于处理记录会话变量
    /// </summary>
    public class SessionData : IEnumerable {

        private IDictionary idic = new Hashtable();

        private string name;

        /// <summary>
        /// 用来获取这个SessionData的名称
        /// </summary>
        public string Name {
            get { return name; }
        }

        protected string id;

        /// <summary>
        /// 用来获取这个Session的唯一ID
        /// </summary>
        public string SessionID {
            get { return id; }
        }

        /// <summary>
        /// 用于初始化和记录其SessionData名字
        /// </summary>
        /// <param name="id">会话ID（区分该业务类型下的不同会话）</param>
        /// <param name="name">会话信息的类型（区分业务类型）</param>
        public SessionData(string id, string name) {
            this.id = id;
            this.name = name;
        }

        /// <summary>
        /// 标记此SessionData是否已经被清除!
        /// </summary>
        private bool isClear = false;

        /// <summary>
        /// 标记SessionDataManager是否需要清除，默认为False
        /// </summary>
        public bool IsClear {
            get { return this.isClear; }
        }

        /// <summary>
        /// 设置其应该被清除
        /// </summary>
        public void Clear() {
            this.isClear = true;
            this.idic.Clear();
        }


        private bool cleanCache = false;
        /// <summary>
        /// 设置为清除缓存。
        /// 此为SessionData对象访问SessionDataAdapter的缓存ResourceDecorator对象之间的交互属性
        /// 设置为真后，则装饰器重新初始化缓存对象
        /// </summary>
        public void CleanCache() {
            this.cleanCache = true;
        }

        public bool IsCleanCache() { return this.cleanCache; }
        ///// <summary>
        ///// 共用一个idic只是兄弟间Name不同
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="source"></param>
        //public SessionData(string name, SessionData source)
        //    : this(name) {
        //    this.idic = source.idic;
        //}

        /// <summary>
        /// 反序列化初始化其自身数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isClear"></param>
        public virtual void Deserialize(string data, bool isClear) {
            if (isClear)
                idic.Clear();
            //这里处理初次登陆空字符串情况。
            if (!string.IsNullOrEmpty(data))
                foreach (string d in data.Split('&')) {
                    string[] p = d.Split('=');
                    if (p.Length <= 1)
                        throw new InvalidOperationException("标示字符的值不正确！");
                    idic[p[0]] = GCL.Common.Tool.WebDecode(p[1]);
                }
        }

        /// <summary>
        /// 反序列化初始化其自身情况 并清理原有数据
        /// </summary>
        /// <param name="data"></param>
        public void Deserialize(string data) {
            this.Deserialize(data, true);
        }

        /// <summary>
        /// 用于生成一个SessionData
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SessionData Deserialize(string id, string name, string data) {
            SessionData _data = new SessionData(id, name);
            _data.Deserialize(data);
            return _data;
        }

        /// <summary>
        /// 获取其临时信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[object key] {
            get { return idic[key]; }
            set { idic[key] = value; }
        }

        /// <summary>
        /// 移除信息
        /// </summary>
        /// <param name="key"></param>
        public void Remove(object key) {
            this.idic.Remove(key);
        }

        /// <summary>
        /// 将其本身序列化 //todo 应该把我们特有的encode方法写入 .Net.Tool
        /// </summary>
        /// <returns></returns>
        public virtual string Serialize() {
            StringBuilder sb = new StringBuilder();
            for (IDictionaryEnumerator ie = idic.GetEnumerator(); ie.MoveNext(); )
                sb.Append(string.Format("{0}={1}&", ie.Key, GCL.Common.Tool.WebEncode(ie.Value.ToString())));
            return sb.ToString().TrimEnd('&');
        }

        public int Count {
            get { return idic.Count; }
        }

        public void Remove(string key) {
            this.idic.Remove(key);
        }

        /// <summary>
        /// 产生GUID随机会话ID
        /// </summary>
        /// <returns></returns>
        public static string CreateGUIDSessionID() {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        #region IEnumerable Members

        public virtual IEnumerator GetEnumerator() {
            return idic.GetEnumerator();
        }

        #endregion
    }
}
