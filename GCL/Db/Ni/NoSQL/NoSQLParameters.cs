using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

//NoSQL CacheModuler VJ3层 ESB workflow  
namespace GCL.Db.Ni.NoSQL {

    public abstract class NoSQLParameters : IDataParameterCollection, ICloneable {
        protected IDictionary<string, NoSQLParameter> dic;
        /// <summary>
        /// 需要严格控制顺序的参数列
        /// </summary>
        protected LinkedList<NoSQLParameter> paras;
        public NoSQLParameters(int capacity) { this.dic = new Dictionary<string, NoSQLParameter>(capacity); this.paras = new LinkedList<NoSQLParameter>(); }
        public NoSQLParameters() { this.dic = new Dictionary<string, NoSQLParameter>(); this.paras = new LinkedList<NoSQLParameter>(); }

        public virtual bool Contains(string parameterName) {
            if (string.IsNullOrEmpty(parameterName)) { throw new InvalidOperationException("不能判断空白参数"); }
            return this.dic.ContainsKey(parameterName);
        }

        public virtual int IndexOf(string parameterName) {
            if (string.IsNullOrEmpty(parameterName)) { throw new InvalidOperationException("不能判断空白参数"); }
            if (this.dic.Count() == 0) return -1;
            return IndexOf(this[parameterName]);
        }

        public virtual void RemoveAt(string parameterName) {
            if (dic.ContainsKey(parameterName)) {
                lock (dic) {
                    if (dic.ContainsKey(parameterName)) {
                        var val = this.dic[parameterName];
                        this.dic.Remove(parameterName);
                        if (val != null) {
                            this.paras.Remove(val);
                        }
                    }
                }
            }
        }

        public virtual object this[string parameterName] {
            get {
                if (this.dic.ContainsKey(parameterName))
                    return this.dic[parameterName];
                else return null;
            }
            set {
                if (!(value is NoSQLParameter)) throw new InvalidOperationException("请设置ObjectParameter类型实例");
                if (this.dic.ContainsKey(parameterName)) {
                    lock (this.dic) {
                        this.RemoveAt(parameterName);
                    }
                }
                this.dic[parameterName] = value as NoSQLParameter;
                this.paras.AddLast(value as NoSQLParameter);
            }
        }

        protected NoSQLParameter Check(object value) {
            if (value == null) {
                throw new InvalidOperationException("不能设置空对象!");
            } else
                if (!(value is NoSQLParameter)) {
                    throw new InvalidOperationException("仅能设置ObjectParameter!");
                } else return value as NoSQLParameter;
        }

        public virtual int Add(object value) {
            var param = this.Check(value);
            this[param.ParameterName] = param;
            return IndexOf(value);
        }

        public virtual void Clear() {
            lock (this.dic) {
                this.dic.Clear();
                this.paras.Clear();
            }
        }

        public virtual bool Contains(object value) {
            if (!(value is NoSQLParameter)) {
                return false;
            }
            var param = value as NoSQLParameter;
            return this.dic.ContainsKey(param.ParameterName);
        }

        public virtual int IndexOf(object value) {
            var param = this.Check(value);
            lock (this.paras) {
                LinkedListNode<NoSQLParameter> node = this.paras.First;
                for (var w = 0; w < this.paras.Count; w++) {
                    if (param == node.Value) return w;
                    node = node.Next;
                }
                return -1;
            }
        }

        public virtual void Insert(int index, object value) {
            var val = this.Check(value);
            if (index > this.paras.Count()) { this.Add(value); } else if (this.Contains(value)) { this[val.ParameterName] = val; } else {
                lock (this.dic) {
                    this.dic[val.ParameterName] = val;
                    if (index >= this.paras.Count) {
                        this.paras.AddLast(val);
                    }
                    lock (this.paras) {
                        LinkedListNode<NoSQLParameter> node = this.paras.First;
                        for (var w = 0; w < index; w++) {
                            node = node.Next;
                        }
                        this.paras.AddAfter(node, val);
                    }
                }
            }
        }

        public virtual bool IsFixedSize {
            get { return false; }
        }

        public virtual bool IsReadOnly {
            get { return false; }
        }

        public virtual void Remove(object value) {
            var val = this.Check(value);
            if (this.dic.ContainsKey(val.ParameterName)) {
                lock (this.dic) {
                    this.dic.Remove(val.ParameterName);
                    this.paras.Remove(val);
                }
            }
        }

        public virtual void RemoveAt(int index) {
            if (this.paras.Count() > index) {
                var val = this.paras.ElementAt(index);
                lock (this.dic) {
                    if (this.dic.ContainsKey(val.ParameterName))
                        this.dic.Remove(val.ParameterName);
                    this.paras.Remove(val);
                }
            }
        }

        public virtual object this[int index] {
            get {
                if (index >= this.paras.Count) {
                    throw new IndexOutOfRangeException("超出边界");
                }
                lock (this.dic) {
                    LinkedListNode<NoSQLParameter> node = this.paras.First;
                    for (var w = 0; w < index; w++) {
                        node = node.Next;
                    }
                    return node;
                }
            }
            set {
                this.Insert(index, value);
            }
        }

        public virtual void CopyTo(Array array, int index) {
            if (array.Length - index < this.paras.Count()) { throw new IndexOutOfRangeException("数组的值小于源数据界限"); }
            if (!(array is NoSQLParameter[])) { throw new InvalidOperationException("目标数组应该是ObjectParameter[]类型"); }
            this.paras.CopyTo((NoSQLParameter[])array, index);
        }

        public virtual int Count {
            get { return this.paras.Count(); }
        }

        public virtual bool IsSynchronized {
            get { return true; }
        }

        public virtual object SyncRoot {
            get { return this.dic; }
        }

        public virtual System.Collections.IEnumerator GetEnumerator() {
            return this.paras.GetEnumerator();
        }

        public abstract object Clone();

        public virtual object Clone(NoSQLParameters paras) {
            NoSQLParameters odp = paras;
            lock (this.dic) {
                var array = this.paras.ToArray().Where(p => {
                    odp.Add(p.Clone());
                    return false;
                }).Count();
            };
            return odp;
        }
    }
}
