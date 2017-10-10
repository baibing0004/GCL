using System.Collections;
namespace GCL.Collections.Pool {

    public class CollectionPoolStaregy : IPoolStaregy {

        protected IList coll;

        public CollectionPoolStaregy(IList coll) {
            this.coll = coll;
        }

        public CollectionPoolStaregy()
            : this(new Collections.DictionarySet()) {
        }

        #region IPoolStaregy Members

        ///
        /// （非 Javadoc）
        /// 
        /// @see GCL.Collections.Pool.IPoolStaregy#clear()
        ///
        public virtual void Clear() {
            this.coll.Clear();
        }

        ///
        /// （非 Javadoc）
        /// 
        /// @see GCL.Collections.Pool.IPoolStaregy#contains(java.lang.object)
        ///
        public virtual bool Contains(object value) {
            return this.coll.Contains(value);
        }

        ///
        /// （非 Javadoc）
        /// 
        /// @see GCL.Collections.Pool.IPoolStaregy#get()
        ///
        public virtual object Get()  {
		lock (coll) {
            System.Collections.IEnumerator ite = this.coll.GetEnumerator();
			if (ite.MoveNext()) {
				object v = ite.Current;
				Remove(v);
				return v;
			} else
				return null;
		}
	}

        ///
        /// （非 Javadoc）
        /// 
        /// @see GCL.Collections.Pool.IPoolStaregy#remove(java.lang.object)
        ///
        public virtual void Remove(object value) {
            this.coll.Remove(value);
        }

        ///
        /// （非 Javadoc）
        /// 
        /// @see GCL.Collections.Pool.IPoolStaregy#set(java.lang.object)
        ///
        public virtual object Set(object value) {
            this.coll.Add(value);
            return null;
        }

        /// （非 Javadoc）
        /// @see GCL.Collections.Pool.IPoolStaregy#createNewInstance()
        ///
        public virtual IPoolStaregy CreateNewInstance() {            
            return new CollectionPoolStaregy((IList)System.Activator.CreateInstance(this.coll.GetType()));
        }
        #endregion
    }
}
