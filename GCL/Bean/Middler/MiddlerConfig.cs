using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using GCL.IO.Config;
namespace GCL.Bean.Middler {
    /// <summary>
    /// 中介者使用的具体Config类
    /// </summary>
    public class MiddlerConfig : AConfig, IClosable {

        public readonly static string NodeName = "Middler";
        private IDictionary<string, IDictionary<string, AObjectContainer>> idic = null;
        public MiddlerConfig(IDictionary<string, IDictionary<string, AObjectContainer>> idic)
            : base(true, true) {
            this.idic = idic;
        }

        public override object GetValue(object key) {
            return ((MiddlerGetParameter)key).GetValue(this);
        }

        #region 被MiddlerGetParameter访问者调用的具体方法

        public object GetValueByName(string key, string name) {
            if (idic.ContainsKey(key.Trim().ToLower()) && idic[key.Trim().ToLower()].ContainsKey(name.Trim().ToLower()))
                return idic[key.Trim().ToLower()][name.Trim().ToLower()].GetValue();
            else
                return null;
        }

        public object[] GetValueByType(string key, Type type) {
            IList list = new ArrayList();
            if (idic.ContainsKey(key.Trim().ToLower()))
                for (IEnumerator<KeyValuePair<string, AObjectContainer>> ienum2 = idic[key.Trim().ToLower()].GetEnumerator(); ienum2.MoveNext(); ) {
                    try {
                        object v = ienum2.Current.Value.GetValue();
                        if (type.IsInstanceOfType(v))
                            list.Add(v);
                        else
                            ienum2.Current.Value.SetValue(v);
                    } catch {
                    }
                }
            if (list.Count > 0) {
                object[] v = new object[list.Count];
                list.CopyTo(v, 0);
                return v;
            }
            return null;
        }
        #endregion

        public override void SetValue(object key, object value) {
            ((MiddlerSetParameter)key).SetValue(this, value);
        }

        #region 被MiddlerSetParameter访问者调用的具体方法

        public void SetValue(string key, string name, object v) {
            if (idic.ContainsKey(key.Trim().ToLower()) && idic[key.Trim().ToLower()].ContainsKey(name.Trim().ToLower()))
                idic[key.Trim().ToLower()][name.Trim().ToLower()].SetValue(v);
        }

        #endregion

        public override void Merge(AConfig config) {
            if (!(config is MiddlerConfig))
                throw new InvalidOperationException(config.GetType().Name + "不是MiddlerConfig或其子类!");
            MiddlerConfig con = config as MiddlerConfig;
            lock (idic) {
                //idic.Clear();不能处理Match情况
                //更新内部idic
                for (IEnumerator<KeyValuePair<string, IDictionary<string, AObjectContainer>>> ienum = con.idic.GetEnumerator(); ienum.MoveNext(); ) {
                    idic[ienum.Current.Key] = ienum.Current.Value;
                    //20130509 baibing 取消Match 而改为直接替换
                    continue;
                    if (!idic.ContainsKey(ienum.Current.Key))
                        idic[ienum.Current.Key] = ienum.Current.Value;
                    else
                        for (IEnumerator<KeyValuePair<string, AObjectContainer>> ienum2 = ienum.Current.Value.GetEnumerator(); ienum2.MoveNext(); ) {
                            try {
                                idic[ienum.Current.Key][ienum2.Current.Key].Close();
                            } catch {
                            }
                            try {
                                idic[ienum.Current.Key][ienum2.Current.Key].Dispose();
                            } catch {
                            }
                            idic[ienum.Current.Key][ienum2.Current.Key] = ienum2.Current.Value;
                        }
                }
            }
            con.idic.Clear();
        }

        #region IClosable Members

        public void Close() {
            IDictionary<string, IDictionary<string, AObjectContainer>> idic = this.idic;
            this.idic = null;
            try {
                if (idic != null)
                    for (IEnumerator<KeyValuePair<string, IDictionary<string, AObjectContainer>>> ie = idic.GetEnumerator(); ie.MoveNext(); )
                        for (IEnumerator<KeyValuePair<string, AObjectContainer>> ie2 = ie.Current.Value.GetEnumerator(); ie2.MoveNext(); ) {
                            try {
                                ie2.Current.Value.Close();
                            } catch {
                            }
                            try {
                                ie2.Current.Value.Dispose();
                            } catch {
                            }
                        }
            } catch {
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            Close();
        }

        #endregion
    }
}