using System;
using System.Collections.Generic;
using System.Collections;
using GCL.Collections;
using GCL.Event;
using GCL.Common;
namespace GCL.Collections.Pool {


    ///
    /// @author baibing
    /// 多键多值池，可以分别设定每个池的大小，或者统一设定大小。
    ///
    public class MapPool<K, V> {
        protected IDictionary<K, KeyValuePool> iKeyMap;

        public event EEventHandle<MapPoolEventArg> OnGet;
        public event EEventHandle<MapPoolEventArg> OnSet;
        public event EEventHandle<MapPoolEventArg> OnRemove;
        public event EEventHandle<MapPoolEventArg> OnGetLimit;
        public event EEventHandle<MapPoolEventArg> OnSetLimit;
        public event EEventHandle<MapPoolEventArg> OnClose;
        public event EEventHandle<MapPoolEventArg> OnObjectClose;

        protected LimitNum num = null;

        protected IKeyValuePoolFactory<K> kvpFactory = null;

        ///
        /// @param kvpFactory
        ///            获取策略
        /// @param valueSet
        ///            值得集合
        /// @param size
        ///            池大小
        ///
        public MapPool(IKeyValuePoolFactory<K> kvpFactory, int size)
            : this(new Dictionary<K, KeyValuePool>(), kvpFactory, new LimitNum(size)) {
        }

        ///
        /// @param kvpFactory
        ///
        public MapPool(IKeyValuePoolFactory<K> kvpFactory)
            : this(new Dictionary<K, KeyValuePool>(), kvpFactory, new LimitNum()) {
        }

        ///
        /// @param kvpFactory
        ///            获取策略
        /// @param valueSet
        /// @param num
        ///
        public MapPool(IDictionary<K, KeyValuePool> keyMap, IKeyValuePoolFactory<K> kvpFactory, LimitNum num)
            : this(keyMap, num) {
            this.SetKeyValuePoolFactory(kvpFactory);
        }

        protected MapPool(IDictionary<K, KeyValuePool> keyMap, LimitNum num)
            : base() {
            this.iKeyMap = keyMap;
            this.iKeyMap.Clear();
            this.num = num;
        }

        protected MapPool(IDictionary<K, KeyValuePool> keyMap, int size)
            : this(keyMap, new LimitNum(size)) {
        }

        protected MapPool(int size)
            : this(new Dictionary<K, KeyValuePool>(), new LimitNum(size)) {
        }

        protected MapPool()
            : this(new Dictionary<K, KeyValuePool>(), new LimitNum()) {
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

        protected KeyValuePool GetKeyValuePool(K key) {
            if (!this.iKeyMap.ContainsKey(key)) {
                KeyValuePool pool = this.kvpFactory.CreateKeyValuePool(key);
                pool.OnClose += new EEventHandle<PoolEventArg>(MapPool_OnClose);
                pool.OnGet += new EEventHandle<PoolEventArg>(MapPool_OnGet);
                pool.OnGetLimit += new EEventHandle<PoolEventArg>(MapPool_OnGetLimit);
                pool.OnObjectClose += new EEventHandle<PoolEventArg>(MapPool_OnObjectClose);
                pool.OnRemove += new EEventHandle<PoolEventArg>(MapPool_OnRemove);
                pool.OnSet += new EEventHandle<PoolEventArg>(MapPool_OnSet);
                pool.OnSetLimit += new EEventHandle<PoolEventArg>(MapPool_OnSetLimit);
                this.iKeyMap.Add(key, pool);
            }
            return this.iKeyMap[key];
        }

        void MapPool_OnSetLimit(object sender, PoolEventArg e) {
            CallEvent(this.OnSetLimit, this, MapPoolEventArg.CreateMapPoolEventArg(((KeyValuePool)sender).GetKey(), e.GetValue()));
        }
        void MapPool_OnSet(object sender, PoolEventArg e) {
            CallEvent(this.OnSet, this, MapPoolEventArg.CreateMapPoolEventArg(((KeyValuePool)sender).GetKey(), e.GetValue()));
        }
        void MapPool_OnRemove(object sender, PoolEventArg e) {
            CallEvent(this.OnRemove, this, MapPoolEventArg.CreateMapPoolEventArg(((KeyValuePool)sender).GetKey(), e.GetValue()));
        }
        void MapPool_OnObjectClose(object sender, PoolEventArg e) {
            CallEvent(this.OnObjectClose, this, MapPoolEventArg.CreateMapPoolEventArg(((KeyValuePool)sender).GetKey(), e.GetValue()));
        }
        void MapPool_OnGetLimit(object sender, PoolEventArg e) {
            CallEvent(this.OnGetLimit, this, MapPoolEventArg.CreateMapPoolEventArg(((KeyValuePool)sender).GetKey(), e.GetValue()));
        }
        void MapPool_OnGet(object sender, PoolEventArg e) {
            CallEvent(this.OnGet, this, MapPoolEventArg.CreateMapPoolEventArg(((KeyValuePool)sender).GetKey(), e.GetValue()));
        }

        void MapPool_OnClose(object sender, PoolEventArg e) {
            CallEvent(this.OnClose, this, MapPoolEventArg.CreateMapPoolEventArg(((KeyValuePool)sender).GetKey(), e.GetValue()));
        }


        protected void CallEvent(
                EEventHandle<MapPoolEventArg> handle,
                object sender, MapPoolEventArg e) {
            MapPoolEventArg.CallEventSafely(handle, sender, e);

        }

        public V Get(K key) {
            lock (this) {
                Tool.ObjectPulseAll(this);
                int _num = this.GetKeyValuePool(key).GetSize();
                V value = (V)this.GetKeyValuePool(key).Get();
                _num = this.GetKeyValuePool(key).GetSize() - _num;
                if (_num > 0)
                    try {
                        this.num.Increase(_num);
                    } catch (Exception) {
                        this.GetKeyValuePool(key).Remove(value);
                        throw;
                    }
                return value;
            }
        }

        public V Set(K key, V value) {
            lock (this) {
                try {
                    Tool.ObjectPulseAll(this);
                    return (V)this.GetKeyValuePool(key).Set(value);
                } catch (IndexOutOfRangeException ex) {
                    throw new IndexOutOfRangeException("Map池内可用对象已满!");
                } catch (Exception ex) {
                    this.num.Decrease();
                    if (!(ex is PoolClosedException))
                        throw ex;
                    else
                        return default(V);
                }
            }
        }

        public void Remove(K key, V value) {
            lock (this) {
                if (this.GetKeyValuePool(key).Contains(value)) {
                    Tool.ObjectPulseAll(this);
                    this.GetKeyValuePool(key).Remove(value);
                    this.num.Decrease();
                }
            }
        }

        private bool close = false;

        public bool IsClose() {
            return this.close;
        }

        public void Close() {
            lock (this) {
                for (IEnumerator<KeyValuePair<K, KeyValuePool>> ide = this.iKeyMap.GetEnumerator(); ide.MoveNext(); )
                    ide.Current.Value.Close();
                Tool.ObjectPulseAll(this);
                this.num.SetNow(0);
                this.close = true;
            }
        }

        ///
        /// @param kvpFactory
        ///            要设置的 kvpFactory
        ///
        protected void SetKeyValuePoolFactory(IKeyValuePoolFactory<K> kvpFactory) {
            this.kvpFactory = kvpFactory;
        }
    }
}
