using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using GCL.Common;

namespace GCL.Collections {
    /// <summary>
    /// �����࣬���ڴ������������Ƶ�Queue�� ����ʹ��notifyAll��wait������ɽ��ӳ���
    /// </summary>
    public class LimitQueue:Queue,ICoordinateLimit {

        protected Queue coll = null;

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
        public LimitQueue(Queue coll,int size) {
            if(coll == null)
                throw new Exception("null coll!!!");
            this.coll = coll;
            this.num = new LimitNum(size);
            if(coll.Count > size)
                throw new Exception("coll������ֵ�Ѿ�����Size������");
        }

        internal LimitQueue(Queue coll,LimitNum e,ICoordinateLimit sl) {
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
            return new LimitQueue(this.coll,this.num,this);
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

        public override object Dequeue() {
            lock(this) {
                this.num.Decrease();
                object result;
                try {
                    result = this.coll.Dequeue();
                } catch(Exception ex) {
                    this.num.Increase();
                    throw ex;
                }
                this.LimitNotifyAll();
                return result;
            }
        }

        public override void Enqueue(object obj) {
            lock(this) {
                this.num.Increase();
                try {
                    this.coll.Enqueue(obj);
                } catch(Exception ex) {
                    this.num.Decrease();
                    throw ex;
                }
                this.LimitNotifyAll();
                return;
            }
        }

        public override bool Equals(object obj) {
            if(obj is Queue)
                return this.coll.Equals(obj);
            else if(obj is LimitQueue)
                return this.Equals((LimitQueue)obj);
            else
                return false;
        }

        public bool Equals(LimitQueue arg0) {
            lock(this) {
                return this.coll.Equals(arg0.coll) && this.num.Equals(arg0.num);
            }
        }

        /// <summary>
        /// ���ص�һ������ ���û�ж�����ô�ᵯ�� ��֧�ֵĲ�������
        /// </summary>
        /// <returns></returns>
        public override object Peek() {
            return this.coll.Peek();
        }

        public override object[] ToArray() {
            return this.coll.ToArray();
        }

        public override string ToString() {
            return this.coll.ToString();
        }

        public override void TrimToSize() {
            lock(this) {
                this.coll.TrimToSize();
                this.num.SetNow(this.coll.Count);
                this.LimitNotifyAll();
            }
        }

        public override int GetHashCode() {
            return this.coll.GetHashCode();
        }
    }
}
