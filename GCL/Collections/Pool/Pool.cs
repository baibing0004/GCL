using System;
using System.Collections;
using GCL.Collections;
using GCL.Event;
using GCL.Common;
namespace GCL.Collections.Pool {

    /// <summary>
    /// 基本的多实例池，实现池方式的存取。但是不管理对象的新建和销毁
    /// </summary>
    public class Pool {

        protected IList valueSet;

        private IPoolStaregy poolStaregy;

        public event EEventHandle<PoolEventArg> OnGet;
        public event EEventHandle<PoolEventArg> OnSet;
        public event EEventHandle<PoolEventArg> OnRemove;
        public event EEventHandle<PoolEventArg> OnGetLimit;
        public event EEventHandle<PoolEventArg> OnSetLimit;
        public event EEventHandle<PoolEventArg> OnClose;
        public event EEventHandle<PoolEventArg> OnObjectClose;

        protected LimitNum num = null;

        ///
        /// @param poolStaregy
        ///            获取策略
        /// @param valueSet
        ///            值得集合
        /// @param size
        ///            池大小
        ///
        public Pool(IPoolStaregy poolStaregy, IList valueSet, int size)
            : this(poolStaregy, valueSet, new LimitNum(size)) {
        }

        ///
        /// @param poolStaregy
        /// @param valueSet
        ///
        public Pool(IPoolStaregy poolStaregy, IList valueSet)
            : this(poolStaregy, valueSet, new LimitNum()) {
        }

        public Pool(IPoolStaregy poolStaregy)
            : this(poolStaregy, new DictionarySet(), new LimitNum()) {
        }

        ///
        /// @param poolStaregy
        ///            获取策略
        /// @param valueSet
        ///            值得集合
        /// @param size
        ///            池大小
        ///
        public Pool(IPoolStaregy poolStaregy, int size)
            : this(poolStaregy, new DictionarySet(), new LimitNum(size)) {
        }


        ///
        /// @param poolStaregy
        /// @param valueSet
        /// @param num
        ///
        protected Pool(IPoolStaregy poolStaregy, IList valueSet, LimitNum num)
            : base() {
            this.poolStaregy = poolStaregy;
            this.valueSet = valueSet;
            this.valueSet.Clear();
            this.num = num;
            this.OnClose += new EEventHandle<PoolEventArg>(PoolEventArg._EventHandleDefault);
            this.OnGet += new EEventHandle<PoolEventArg>(PoolEventArg._EventHandleDefault);
            this.OnGetLimit += new EEventHandle<PoolEventArg>(PoolEventArg._EventHandleDefault);
            this.OnObjectClose += new EEventHandle<PoolEventArg>(PoolEventArg._EventHandleDefault);
            this.OnRemove += new EEventHandle<PoolEventArg>(PoolEventArg._EventHandleDefault);
            this.OnSet += new EEventHandle<PoolEventArg>(PoolEventArg._EventHandleDefault);
            this.OnSetLimit += new EEventHandle<PoolEventArg>(PoolEventArg._EventHandleDefault);
        }

        ///
        /// @return 是否已经到顶部
        ///
        public bool IsFull() {
            return this.num.IsFull();
        }

        ///
        /// @return 是否已经到顶部
        ///
        public bool IsEmpty() {
            return this.num.IsEmpty();
        }

        ///
        /// @return 是否已近到底部
        ///
        public bool IsButtom() {
            return this.num.IsButtom();
        }

        public int GetSize() {
            return this.num.GetNow();
        }

        public int GetCapacity() {
            return this.num.GetMax();
        }

        public virtual object Get(TimeSpan span) {
            DateTime lTime = DateTime.Now.Add(span);
            do {
                try {
                    return this.Get();
                } catch (PoolClosedException) {
                    throw;
                } catch (IndexOutOfRangeException) {
                    int ms = (int)lTime.Subtract(DateTime.Now).TotalMilliseconds;
                    if (ms > 0)
                        Tool.ObjectWait(this, ms);
                    else
                        throw;
                }
            } while (true);
        }

        /// <summary>
        /// 这里并不会增加池的容量，仅仅是获取新的实例，在实例不可创建时，池满
        /// </summary>
        /// <returns></returns>
        public virtual object Get() {
            lock (this) {
                object value = this.poolStaregy.Get();
                if (!Tool.IsEnable(value)) {
                    this.CallStanderEvent(this.OnGetLimit, this, PoolEventArg
                            .CreatePoolEventArg(value));
                    throw new System.IndexOutOfRangeException("池内可用对象为空!");
                }

                Tool.ObjectPulseAll(this);
                this.CallStanderEvent(this.OnGet, this, PoolEventArg
                        .CreatePoolEventArg(value));
                return value;
            }
        }

        /// <summary>
        /// 增加池的容量，确认使用过的对象已经返回
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual object Set(object value) {
            lock (this) {
                try {
                    if (!this.valueSet.Contains(value)) {
                        this.num.Increase();
                        this.valueSet.Add(value);
                    }
                    if (!this.poolStaregy.Contains(value)) {
                        object _v = this.poolStaregy.Set(value);
                        Tool.ObjectPulseAll(this);
                        this.CallStanderEvent(this.OnSet, this, PoolEventArg
                                .CreatePoolEventArg(value));
                        return _v;
                    } else
                        return null;
                } catch (IndexOutOfRangeException ex) {
                    this.CallStanderEvent(this.OnSetLimit, this, PoolEventArg
                            .CreatePoolEventArg(value));
                    throw new System.IndexOutOfRangeException("池内可用对象已满!");
                } catch (PoolClosedException) {
                    return null;
                }
            }
        }

        public virtual void Remove(object value) {
            lock (this) {
                this.poolStaregy.Remove(value);
                if (valueSet.Contains(value))
                    this.num.Decrease();
                this.valueSet.Remove(value);
                Tool.ObjectPulseAll(this);
                this.CallStanderEvent(this.OnRemove, this, PoolEventArg
                        .CreatePoolEventArg(value));
            }
        }

        public bool IsClose() {
            return this.poolStaregy is ClosePoolStaregy;
        }

        private bool isWaitCustomerClose = false;

        /**
         * @return isWaitCustomerClose
         */
        public bool IsWaitCustomerClose() {
            return isWaitCustomerClose;
        }

        /**
         * @param isWaitCustomerClose 要设置的 isWaitCustomerClose
         */
        public void SetWaitCustomerClose(bool isWaitCustomerClose) {
            this.isWaitCustomerClose = isWaitCustomerClose;
        }

        public bool WaitCustomerClose {
            get { return IsWaitCustomerClose(); }
            set { SetWaitCustomerClose(value); }
        }

        public virtual IList Close() {
            lock (this) {
                foreach (object value in this.valueSet) {
                    if (!this.isWaitCustomerClose || poolStaregy.Contains(value))
                        ObjectClose(value);
                }
                this.num.SetNow(this.num.GetMin());
                //this.valueSet.Clear();
                this.poolStaregy.Clear();
                this.poolStaregy = new ClosePoolStaregy(this);
                this.CallStanderEvent(this.OnClose, this, PoolEventArg
                        .CreatePoolEventArg(null));
                Tool.ObjectPulseAll(this);
                return valueSet;
            }
        }

        public virtual bool Contains(object value) {
            return valueSet.Contains(value);
        }

        protected internal virtual void ObjectClose(object value) {
            this.CallStanderEvent(this.OnObjectClose, this, PoolEventArg
                    .CreatePoolEventArg(value));
        }

        protected virtual void CallStanderEvent(EEventHandle<PoolEventArg> sup,
                object sender, PoolEventArg e) {
            PoolEventArg.CallEventSafely(sup, sender, e);
        }

        protected void CallGetEvent(object value) {
            this.CallStanderEvent(this.OnGet, this, PoolEventArg.CreatePoolEventArg(value));
        }


    }///
    /// 关闭情况下的PoolStaregy
    /// 
    /// @author baibing
    ///
    class ClosePoolStaregy : IPoolStaregy {

        private Pool pool;
        public ClosePoolStaregy(Pool pool) {
            this.pool = pool;
        }
        #region IPoolStaregy Members
        public object Get() {
            throw new PoolClosedException("池已经关闭!");
        }

        public object Set(object value) {
            if (Tool.IsEnable(value))
                pool.ObjectClose(value);
            throw new PoolClosedException("池已经关闭!");
        }

        public void Remove(object value) {
        }

        public void Clear() {
        }

        public bool Contains(object value) {
            return false;
        }

        public IPoolStaregy CreateNewInstance() {
            return new ClosePoolStaregy(this.pool);
        }

        #endregion
    }
}
