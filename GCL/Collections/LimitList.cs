using System;
using System.Text;
using System.Collections;

namespace GCL.Collections {

    public class LimitList:LimitCollection,System.Collections.IList {

        /// <summary>
        /// ��ʵ��ʹ�õ�IList���ص���
        /// </summary>
        /// <returns></returns>
        private IList GetThis() {
            return (IList)this.coll;
        }

        public LimitList(IList list,int size)
            : base(list,size) {
        }

        LimitList(IList coll,LimitNum e,ICoordinateLimit sl)
            : base(coll,e,sl) {
        }

        #region IList Members

        public virtual int Add(object value) {
            lock(this) {
                this.num.Increase();
                //����ID
                int _result = 0;
                try {
                    _result = this.GetThis().Add(value);
                } catch(Exception ex) {
                    this.num.Decrease();
                    throw ex;
                }
                this.LimitNotifyAll();
                return _result;
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

        public virtual bool Contains(object value) {
            return this.GetThis().Contains(value);
        }

        public virtual int IndexOf(object value) {
            return
                this.GetThis().IndexOf(value);
        }

        public virtual void Insert(int index,object value) {
            lock(this) {
                this.num.Increase();
                try {
                    this.GetThis().Insert(index,value);
                } catch(Exception ex) {
                    this.num.Decrease();
                    throw ex;
                }
                this.LimitNotifyAll();
                return;
            }
        }

        public virtual bool IsFixedSize {
            get {
                return GetThis().IsFixedSize;
            }
        }

        public virtual bool IsReadOnly {
            get {
                return GetThis().IsReadOnly;
            }
        }

        public virtual void Remove(object value) {
            lock(this) {
                this.num.Decrease();
                try {
                    GetThis().Remove(value);
                } catch(Exception ex) {
                    this.num.Increase();
                    throw ex;
                }
                this.LimitNotifyAll();
                return;
            }
        }

        public virtual void RemoveAt(int index) {
            lock(this) {
                this.num.Decrease();
                try {
                    GetThis().RemoveAt(index);
                } catch(Exception ex) {
                    this.num.Increase();
                    throw ex;
                }
                this.LimitNotifyAll();
                return;
            }
        }

        public virtual object this[int index] {
            get {
                return GetThis()[index];
            }
            set {
                GetThis()[index] = value;
            }
        }

        #endregion
    }
}
