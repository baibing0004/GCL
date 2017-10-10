using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace GCL.Collections {
    public class LimitDictionary:LimitCollection,IDictionary {
        /// <summary>
        /// 对实际使用的IDictionary返回调用
        /// </summary>
        /// <returns></returns>
        private IDictionary GetThis() {
            return (IDictionary)this.coll;
        }

        public LimitDictionary(IDictionary list,int size)
            : base(list,size) {
        }

        LimitDictionary(IDictionary coll,LimitNum e,ICoordinateLimit sl)
            : base(coll,e,sl) {
        }

        #region IDictionary Members

        public virtual void Add(object key,object value) {
            lock(this) {
                this.num.Increase();
                try {
                    this.GetThis().Add(key,value);
                } catch(Exception ex) {
                    this.num.Decrease();
                    throw ex;
                }
                this.LimitNotifyAll();
                return;
            }

        }

        public virtual void Clear() {
            lock(this) {
                this.GetThis().Clear();
                this.num.SetNow(0);
                this.LimitNotifyAll();
                return;
            }
        }

        public virtual bool Contains(object key) {
            return this.GetThis().Contains(key);
        }

        public new IDictionaryEnumerator GetEnumerator() {
            return this.GetThis().GetEnumerator();
        }

        public virtual bool IsFixedSize {
            get {
                return this.GetThis().IsFixedSize;
            }
        }

        public virtual bool IsReadOnly {
            get {
                return this.GetThis().IsReadOnly;
            }
        }

        public virtual ICollection Keys {
            get {
                return new LimitCollection(this.GetThis().Keys,this.num,this);
            }
        }

        public virtual void Remove(object key) {
            lock(this) {
                this.num.Decrease();
                try {
                    GetThis().Remove(key);
                } catch(Exception ex) {
                    this.num.Increase();
                    throw ex;
                }
                this.LimitNotifyAll();
                return;
            }
        }

        public virtual ICollection Values {
            get {
                return new LimitCollection(this.GetThis().Values,this.num,this);
            }
        }

        public virtual object this[object key] {
            get {
                return GetThis()[key];
            }
            set {
                GetThis()[key] = value;
            }
        }

        #endregion
    }
}
