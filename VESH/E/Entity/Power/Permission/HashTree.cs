using System;
using System.Collections;
using System.Linq;
using System.Web;

namespace GCL.Project.VESH.E.Entity.Power.Permission {

    /// <summary>
    /// 用于拆解EntityPermission的表字段进行内部处理 按照文件夹级别进行比较
    /// </summary>
    internal class HashTree {
        private IDictionary root = new Hashtable();

        /// <summary>
        /// 输入时应不包含项目名
        /// </summary>
        /// <param name="url"></param>
        /// <param name="value"></param>
        public void Add(string url, string value) {
            string[] _s = url.Trim().ToLower().Trim('/', '\\').Split('/', '\\');
            IDictionary idic = root;
            for (int w = 0; w < _s.Length; w++) {
                bool isEnd = (w == _s.Length - 1);
                //去除.page等返回说明 而关注功能
                string key = _s[w];
                if (idic.Contains(key)) {
                    if (idic[key] is IDictionary) {
                        //说明有下级节点
                        idic = idic[key] as IDictionary;
                    } else {
                        //说明是个字符串value 但是不可能是1
                        IDictionary _id = new Hashtable();
                        _id.Add("", idic[key].ToString());
                        idic[key] = _id;
                        idic = _id;
                    }

                    if (isEnd)
                        if (idic.Contains("")) {
                            if (idic[""] is IDictionary) ((IDictionary)idic[""]).Add(value, 1);
                            else {
                                IDictionary _id = new Hashtable();
                                _id.Add(idic[""].ToString(), 1);
                                _id.Add(value, 1);
                                idic[""] = _id;
                            }
                        } else {
                            idic.Add("", value);
                        }
                } else {
                    if (isEnd)
                        idic.Add(key, value);
                    else { IDictionary _id = new Hashtable(); idic.Add(key, _id); idic = _id; }
                }
            }
        }

        public void Reset() {
            this.root.Clear();
        }

        /// <summary>
        /// 根据HashTree结构进行比较
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pc"></param>
        /// <returns></returns>
        public bool Check(string url, APermissionCollection pc) {
            if (pc == null) return false;
            string[] _s = url.Trim().ToLower().Trim('/', '\\').Split('/', '\\');
            IDictionary idic = root;
            for (int w = 0; w < _s.Length; w++) {
                bool isEnd = (w == _s.Length - 1);
                //去除.page等返回说明 而关注功能
                string key = isEnd && _s[w].IndexOf(".") >= 0 ? _s[w].Substring(0, _s[w].LastIndexOf(".")) : _s[w];
                if (idic.Contains("")) {
                    if (idic[""] is IDictionary) {
                        for (IDictionaryEnumerator ie = (idic[""] as IDictionary).GetEnumerator(); ie.MoveNext(); ) {
                            if (!pc.HasRight(ie.Key.ToString())) return false;
                        }
                    } else if (!pc.HasRight(idic[""].ToString())) return false;
                }
                if (idic.Contains(key)) {
                    if (idic[key] is IDictionary) {
                        idic = idic[key] as IDictionary;
                        //最后一次可能会漏掉一次
                        if (isEnd && idic.Contains("")) {
                            if (idic[""] is IDictionary) {
                                for (IDictionaryEnumerator ie = (idic[""] as IDictionary).GetEnumerator(); ie.MoveNext(); ) {
                                    if (!pc.HasRight(ie.Key.ToString())) return false;
                                }
                            } else if (!pc.HasRight(idic[""].ToString())) return false;
                        }
                    } else return pc.HasRight(idic[key].ToString());
                } else return true;
            }
            return true;
        }
    }
}