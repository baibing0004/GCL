using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCL.Db.Ni.NoSQL {
    /// <summary>
    /// 添加公共基类方法
    /// </summary>
    public static class Method {
        public static IDictionary<string, object> Clone(this IDictionary<string, object> docu, QueryEntity entity, string paramSign) {
            var rets = new Dictionary<string, object>();
            GCL.Event.DynamicEventFunc func = null;
            //递归赋值函数
            func = new Event.DynamicEventFunc(p2 => {
                var doc = p2[0] as IDictionary<string, object>;
                var ret = p2[1] as IDictionary<string, object>;
                doc.Where(p => {
                    if (p.Value is IDictionary<string, object>) {
                        ret[p.Key] = new Dictionary<string, object>();
                        func.Invoke(p.Value, ret[p.Key]);
                    } else if (p.Value is IDictionary<string, object>[]) {
                        var arrays = (p.Value as IDictionary<string, object>[]);
                        var arrays2 = new IDictionary<string, object>[arrays.Length];
                        ret[p.Key] = arrays2;
                        int w = 0;
                        arrays.Where(p3 => {
                            arrays2[w] = new Dictionary<string, object>();
                            func.Invoke(p3, arrays2[w]);
                            w++;
                            return false;
                        }).Count();

                    } else {
                        ret[p.Key] = p.Value;
                        if (!string.IsNullOrEmpty(paramSign)) {
                            if (Convert.ToString(p.Value).Trim().StartsWith(paramSign)) {
                                entity.AddParams(Convert.ToString(p.Value), ret);
                            } else if (p.Key.Trim().StartsWith(paramSign)) {
                                entity.AddParams(p.Key, ret);
                            }
                        }
                    }
                    return false;
                }).Count();
            });
            func.Invoke(docu, rets);
            return rets;
        }

        public static QueryEntity[] Clone(this  QueryEntity[] entitis) {
            return null;
        }
    }
    public class QueryEntity {
        public QueryEntity Clone(string paramSign) {
            var ret = new QueryEntity();
            ret.IDs = this.IDs.Clone(ret, paramSign);
            ret.Table = this.Table;
            ret.MethodParam = this.MethodParam.Clone(ret, paramSign);
            ret.WhereParam = this.WhereParam.Clone(ret, paramSign);
            ret.OrderParam = this.OrderParam.Clone(ret, paramSign);
            ret.SkipParam = this.SkipParam.Clone(ret, paramSign);
            ret.LimitParam = this.LimitParam.Clone(ret, paramSign);
            ret.DateTimeParam = this.DateTimeParam.Clone(ret, paramSign);
            ret.Method = this.Method;
            return ret;
        }
        public QueryEntity() {
            this.IDs = new Dictionary<string, object>();
            this.MethodParam = new Dictionary<string, object>();
            this.WhereParam = new Dictionary<string, object>();
            this.OrderParam = new Dictionary<string, object>();
            this.SkipParam = new Dictionary<string, object>();
            this.LimitParam = new Dictionary<string, object>();
            this.DateTimeParam = new Dictionary<string, object>();
            this.Method = string.Empty;
            this.Params = new Dictionary<string, IList<IDictionary<string, object>>>();
        }
        /// <summary>
        /// 表名
        /// </summary>
        public string Table { get; set; }
        /// <summary>
        /// 表主键列
        /// </summary>
        public IDictionary<string, object> IDs { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 查询列说明
        /// </summary>
        public IDictionary<string, object> MethodParam { get; set; }
        /// <summary>
        /// 条件列说明
        /// </summary>
        public IDictionary<string, object> WhereParam { get; set; }
        /// <summary>
        /// 排序列说明
        /// </summary>
        public IDictionary<string, object> OrderParam { get; set; }
        /// <summary>
        /// skip列
        /// </summary>
        public IDictionary<string, object> SkipParam { get; set; }
        /// <summary>
        /// limit列
        /// </summary>
        public IDictionary<string, object> LimitParam { get; set; }
        /// <summary>
        /// 缓存过期时间
        /// </summary>
        public IDictionary<string, object> DateTimeParam { get; set; }

        public IDictionary<string, IList<IDictionary<string, object>>> Params { get; set; }

        public void AddParams(string p, IDictionary<string, object> ret) {
            p = p.Trim();
            if (!Params.ContainsKey(p)) {
                lock (this) {
                    if (!Params.ContainsKey(p)) {
                        Params.Add(p, new List<IDictionary<string, object>>());
                    }
                }
            }
            if (!Params[p].Contains(ret)) {
                lock (this) {
                    if (!Params[p].Contains(ret)) {
                        Params[p].Add(ret);
                    }
                }
            }
        }

    }
}
