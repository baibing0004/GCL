using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using GCL.Common;

namespace GCL.Collections {
    /// <summary>
    /// �����࣬���ڴ������������Ƶ�Stack�� ����ʹ��notifyAll��wait������ɽ��ӳ���
    /// </summary>
    public class LimitStack:Stack,ICoordinateLimit {

        protected Stack coll = null;

        protected LimitNum num = null;

        private ICoordinateLimit sLimit = null;

        /// <summary>
        /// �Ƿ��Ѿ�������
        /// </summary>
        /// <returns></returns>
        public bool IsFull() {
            return this.num.IsFull();
        }

        /// <summary>
        /// �Ƿ�Ϊ0
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty() {
            return this.num.IsEmpty();
        }

        /// <summary>
        /// �Ƿ��ѽ����ײ�
        /// </summary>
        /// <returns></returns>
        public bool IsButtom() {
            return this.num.IsButtom();
        }

        /// <summary>
        /// ����һ������������ICollection
        /// </summary>
        /// <param name="coll">�����Ƶ�Collection</param>
        /// <param name="size">����</param>
        /// <exception >��ʼ������</exception>
        public LimitStack(Stack coll,int size) {
            if(coll == null)
                throw new Exception("null coll!!!");
            this.coll = coll;
            this.num = new LimitNum(size);
            if(coll.Count > size)
                throw new Exception("coll������ֵ�Ѿ�����Size������");
        }

        internal LimitStack(Stack coll,LimitNum e,ICoordinateLimit sl) {
            if(coll == null)
                throw new Exception("null");
            this.coll = coll;
            this.num = e;
            if(coll.Count > e.GetMax())
                throw new Exception("coll������ֵ�Ѿ�����Size������");
            this.sLimit = sl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>�������ֵ</returns>
        public int GetMax() {
            return this.num.GetMax();
        }

        public int Max {
            get {
                return this.GetMax();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>������Сֵ</returns>
        public int GetMin() {
            return this.num.GetMin();
        }

        public int Min {
            get {
                return this.GetMin();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>���ڵ�ֵӦ����Count��ͬ����</returns>
        public int GetNow() {
            return this.num.GetNow();
        }

        public int Now {
            get {
                return this.GetNow();
            }
        }

        #region ICoordinateLimit Members

        public virtual void LimitNotifyAll() {
            Tool.ObjectPulseAll(this);
            if(Tool.IsEnable(this.sLimit))
                this.sLimit.LimitNotifyAll();
        }

        #endregion

        #region ICollection Members

        public override void CopyTo(Array array,int index) {
            this.coll.CopyTo(array,index);
        }

        public override int Count {
            get {
                return this.coll.Count;
            }
        }

        public override bool IsSynchronized {
            get {
                return true;
            }
        }

        public override object SyncRoot {
            get {
                return this;
            }
        }

        #endregion

        #region IEnumerable Members

        public override System.Collections.IEnumerator GetEnumerator() {
            return this.coll.GetEnumerator();
        }

        #endregion


        #region ICloneable Members

        public override object Clone() {
            return new LimitStack(this.coll,this.num,this);
        }

        #endregion

        public override void Clear() {
            lock(this) {
                this.coll.Clear();
                this.num.SetNow(0);
                this.LimitNotifyAll();
                return;
            }
        }

        public override bool Contains(object key) {
            return this.coll.Contains(key);
        }

        public override object Pop() {
            lock(this) {
                this.num.Decrease();
                object result;
                try {
                    result = this.coll.Pop();
                } catch(Exception ex) {
                    this.num.Increase();
                    throw ex;
                }
                this.LimitNotifyAll();
                return result;
            }
        }

        public override void Push(object obj) {
            lock(this) {
                this.num.Increase();
                try {
                    this.coll.Push(obj);
                } catch(Exception ex) {
                    this.num.Decrease();
                    throw ex;
                }
                this.LimitNotifyAll();
                return;
            }
        }

        public override bool Equals(object obj) {
            if(obj is Stack)
                return this.coll.Equals(obj);
            else if(obj is LimitStack)
                return this.Equals((LimitStack)obj);
            else
                return false;
        }

        public bool Equals(LimitStack arg0) {
            lock(this) {
                return this.coll.Equals(arg0.coll) && this.num.Equals(arg0.num);
            }
        }


        public override object Peek() {
            return this.coll.Peek();
        }

        public override object[] ToArray() {
            return this.coll.ToArray();
        }

        public override string ToString() {
            return this.coll.ToString();
        }

        public override int GetHashCode() {
            return this.coll.GetHashCode();
        }
    }
}
