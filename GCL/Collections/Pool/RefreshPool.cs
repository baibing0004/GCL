using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using GCL.Event;

namespace GCL.Collections.Pool {
    /// <summary>
    /// 用于便利的使用池技术的类
    /// </summary>
    public class RefreshPool : KeyValuePool {
        private PoolRefreshThread thread;

        /// <summary>
        /// 请注意这里，不要对线程作任何处理，否则会引起Dispose事件混乱 默认值为1分钟清理
        /// </summary>
        public PoolRefreshThread Thread {
            get { return thread; }
        }

        /// <summary>
        /// 用于便利的使用池技术的类
        /// </summary>
        /// <param name="valueSet"></param>
        /// <param name="size">池大小</param>
        /// <param name="key">默认键值</param>
        /// <param name="pvFactory">根据默认键值生成对象的类</param>
        /// <param name="waitTime">等待时间(毫秒)</param>
        public RefreshPool(IList valueSet, int size, object key, IPoolValueFactory pvFactory, int waitTime)
            : base(new StackPoolStaregy(), valueSet, size, key, pvFactory) {
            thread = new PoolRefreshThread(this, waitTime, new LRUPoolRefreshStaregy(TimeSpan.FromMilliseconds(waitTime)));
            thread.FinallyDoEvent += new GCL.Event.EventHandle(thread_FinallyDoEvent);
            this.OnDispose += new GCL.Event.EEventHandle<PoolEventArg>(PoolEventArg._EventHandleDefault);
            thread.Start();
        }

        /// <summary>
        /// 刷新线程停止，一般的如果是非多线程使用就认为是池已经销毁了，否则可以认为还需要调用池的用户销毁正在使用的池内对象！
        /// </summary>
        public event EEventHandle<PoolEventArg> OnDispose;
        void thread_FinallyDoEvent(object sender, GCL.Event.EventArg e) {
            PoolEventArg.CallEventSafely(this.OnDispose, this, new PoolEventArg());
        }

        ///
        /// @param poolStaregy
        /// @param valueSet
        ///
        public RefreshPool(int size, object key, IPoolValueFactory pvFactory, int waitTime)
            : this(new DictionarySet(), size, key, pvFactory, waitTime) {
        }


        ///
        /// @param poolStaregy
        ///
        public RefreshPool(int size, object key, IPoolValueFactory pvFactory)
            : this(size, key, pvFactory, 60000) {
        }

        public RefreshPool(int size, IPoolValueFactory pvFactory)
            : this(size, "", pvFactory) {
        }
    }
}
