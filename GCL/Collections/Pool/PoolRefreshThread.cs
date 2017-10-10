using System;
using GCL.Common;
using GCL.Event;
using GCL.Threading;
namespace GCL.Collections.Pool {

    ///
    /// @author baibing
    /// 
    ///
    public class PoolRefreshThread : TimerThread {

        ///
        /// @param waitTime
        ///
        public PoolRefreshThread(Pool pool, int waitTime, IPoolRefreshStaregy rps)
            : base(waitTime) {
            this.SetPool(pool);
            this.SetRefreshPoolStaregy(rps);
            this.SetRun(this.Check);
        }

        ///
        /// 
        ///
        public PoolRefreshThread(Pool pool, IPoolRefreshStaregy rps)
            : this(pool, 1000, rps) {
        }

        protected PoolRefreshThread()
            : base(1000) {
        }

        private Pool pool;

        private IPoolRefreshStaregy rps;

        ///
        /// @param box
        ///            要设置的 box
        ///
        protected void SetRefreshPoolStaregy(IPoolRefreshStaregy rps) {
            this.rps = rps;
        }

        ///
        /// @return pool
        ///
        public Pool GetPool() {
            return pool;
        }

        ///
        /// @param pool
        ///            要设置的 pool
        ///
        protected void SetPool(Pool pool) {
            this.pool = pool;
            this.pool.OnGet += new EEventHandle<PoolEventArg>(pool_OnGet);
            this.pool.OnSet += new EEventHandle<PoolEventArg>(pool_OnSet);
            this.pool.OnRemove += new EEventHandle<PoolEventArg>(pool_OnRemove);
            this.pool.OnClose += new EEventHandle<PoolEventArg>(pool_OnClose);
        }


        void pool_OnSet(object sender, PoolEventArg e) {
            lock (this) {
                try {
                    this.rps.Set(e.GetValue());
                } catch (Exception e1) {
                }
            }
        }

        void pool_OnGet(object sender, PoolEventArg e) {
            lock (this) {
                try {
                    this.rps.Remove(e.GetValue());
                } catch (Exception e1) {
                }
            }
        }
        void pool_OnRemove(object sender, PoolEventArg e) {
            lock (this) {
                try {
                    this.rps.Remove(e.GetValue());
                } catch (Exception e1) {
                }
            }
        }
        void pool_OnClose(object sender, PoolEventArg e) {
            try {
                this.rps.Clear();
            } catch (Exception e1) {
            } finally {
                this.SetCanRun(false);
                try {
                    this.Thread.Abort();
                } catch {
                }
            }
        }


        protected void Check(object sender, EventArg e) {
            try {
                bool hasContinue = true;
                while (this.IsCanRun() && hasContinue) {
                    lock (pool) {
                        object value = this.rps.AllowDel();
                        if (Tool.IsEnable(value)) {
#if(DEBUG)
                            this.CallThreadEventSimple(301, "池中有" + pool.GetSize() + "个对象");
#endif
                            pool.Remove(value);
                            pool.ObjectClose(value);
                        } else
                            hasContinue = false;
                    }
                }

            } catch {
            }
        }
    }
}