using System;
using System.Text;
using System.Collections;
using GCL.Common;

namespace GCL.Collections {
    /// <summary>
    /// 限制类，用于处理有容量限制的Collection类 MS的接口没有单独涉及到 Map，Stack，Queue，List，Set 等相关基类的对象
    /// </summary>
    public class LimitCollection:System.Collections.ICollection,ICoordinateLimit,ICloneable {

        protected ICollection coll = null;

        protected LimitNum num = null;

        private ICoordinateLimit sLimit = null;

        /// <summary>
        /// 是否已经到顶部
        /// </summary>
        /// <returns></returns>
        public bool IsFull() {
            return this.num.IsFull();
        }

        /// <summary>
        /// 是否为0
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty() {
            return this.num.IsEmpty();
        }

        /// <summary>
        /// 是否已近到底部
        /// </summary>
        /// <returns></returns>
        public bool IsButtom() {
            return this.num.IsButtom();
        }

        /// <summary>
        /// 建立一个真正的限制ICollection
        /// </summary>
        /// <param name="coll">被限制的Collection</param>
        /// <param name="size">容量</param>
        /// <exception >初始化错误</exception>
        public LimitCollection(ICollection coll,int size) { 
            if(coll == null)
                throw new Exception("null coll!!!");
            this.coll = coll;            
            this.num = new LimitNum(size);
            if(coll.Count > size)
                throw new Exception("coll的现有值已经超过Size的容量");
        }

        internal LimitCollection(ICollection coll,LimitNum e,ICoordinateLimit sl) {
            if(coll == null)
                throw new Exception("null");
            this.coll = coll;
            this.num = e;
            if(coll.Count > e.GetMax())
                throw new Exception("coll的现有值已经超过Size的容量");
            this.sLimit = sl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>容量最大值</returns>
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
        /// <returns>容量最小值</returns>
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
        /// <returns>现在的值应该与Count相同意义</returns>
        public int GetNow() {
            return this.num.GetNow();
        }

        public int Now {
            get {
                return this.GetNow();
            }
        }

        public override int GetHashCode() {
            return this.coll.GetHashCode();
        }

        public override string ToString() {
            return this.coll.ToString();
        }

        /*
         * （非 Javadoc）
         * 
         * @see java.lang.object#equals(java.lang.object)
         */
        public override bool Equals(object arg0) {
            if(arg0 is ICollection)
                return this.coll.Equals(arg0);
            else if(arg0 is LimitCollection)
                return this.Equals((LimitCollection)arg0);
            else
                return false;
        }

        /*
         * （非 Javadoc）
         * 
         * @see java.lang.object#equals(java.lang.object)
         */
        public bool Equals(LimitCollection arg0) {
            lock(this) {                
                return this.coll.Equals(arg0.coll) && this.num.Equals(arg0.num);
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

        public virtual void CopyTo(Array array,int index) {
            this.coll.CopyTo(array,index);
        }

        public virtual int Count {
            get {
                return this.coll.Count;
            }
        }

        public virtual bool IsSynchronized {
            get {
                return true;
            }
        }

        public virtual object SyncRoot {
            get {
                return this;
            }
        }

        #endregion

        #region IEnumerable Members

        public virtual System.Collections.IEnumerator GetEnumerator() {
            return this.coll.GetEnumerator();
        }

        #endregion

        #region ICloneable Members

        public virtual object Clone() {
            return new LimitCollection(this.coll,this.num,this);
        }

        #endregion
    }
}

