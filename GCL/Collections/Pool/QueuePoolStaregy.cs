using System.Collections;
namespace GCL.Collections.Pool {


    public class QueuePoolStaregy:IPoolStaregy {

        protected Queue coll;
        protected IList dataColl;
        public QueuePoolStaregy(System.Collections.Queue coll,System.Collections.IList list){
            this.coll = coll;
            this.coll.Clear();
        }

        public QueuePoolStaregy(){
            this.coll = new Queue();
        }

        ///// £¨·Ç Javadoc£©
        ///// @see GCL.Collections.Pool.CollectionPoolStaregy#get()
        /////
        //public override object Get() {
        //    try {
        //        return ((System.Collections.Queue)this.coll).Dequeue();
        //    } catch (System.InvalidOperationException ex) {
        //        return null;
        //    }
        //}

        #region IPoolStaregy Members

        public virtual object Set(object value) {
            this.coll.Enqueue(value);
            return null;
        }

        public virtual object Get() {
            try {
                return this.coll.Dequeue();
            } catch (System.InvalidOperationException) {
                return null;
            }
        }

        public virtual void Remove(object value) {
            lock (this.coll) {
                try {
                    if (this.coll.Contains(value)) {
                        ArrayList list = new ArrayList(coll);
                        list.Remove(value);
                        this.coll.Clear();
                        this.coll = new Queue(list);
                        list.Clear();
                    }
                } catch (System.NotSupportedException) {
                }
            }
        }

        public virtual void Clear() {
            this.coll.Clear();
        }

        public virtual bool Contains(object value) {
            return this.coll.Contains(value);
        }

        public virtual IPoolStaregy CreateNewInstance() {
            return new QueuePoolStaregy();
        }

        #endregion
    }
}
