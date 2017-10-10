using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using GCL.Event;
using GCL.Common;
using GCL.Collections;
using GCL.Collections.Pool;

namespace GCL.Db {
    public class ConnectionPool<K> : MapPool<K, DbConnection>, IKeyValuePoolFactory<K>, IPoolValueFactory {

        ///
        /// @param kvpFactory
        ///            获取策略
        /// @param valueSet
        ///            值得集合
        /// @param size
        ///            池大小
        ///
        public ConnectionPool(int size)
            : base(size) {
            this.SetKeyValuePoolFactory(this);
        }

        ///
        /// @param kvpFactory
        ///
        public ConnectionPool() {
            this.SetKeyValuePoolFactory(this);
        }

        ///
        /// @param kvpFactory
        ///            获取策略
        /// @param valueSet
        /// @param num
        ///
        public ConnectionPool(IDictionary<K, KeyValuePool> keyMap, LimitNum num)
            : base(keyMap, num) {
            this.SetKeyValuePoolFactory(this);
        }

        protected ConnectionPool(IDictionary<K, KeyValuePool> keyMap, int size)
            : base(keyMap, size) {
            this.SetKeyValuePoolFactory(this);
        }

        #region IPoolValueFactory Members

        public virtual object CreateObject(object e) {
            if (e is ConnectionArg)
                return ((ConnectionArg)e).CreateConnection();
            else
                throw new Exception("e is not a ConnectionArg");
        }

        public virtual void CloseObject(object obj) {
            if (Tool.IsEnable(obj) && obj is DbConnection && ((DbConnection)obj).State != System.Data.ConnectionState.Closed)
                try {
                    ((DbConnection)obj).Close();
                } catch (Exception) {
                }
        }

        #endregion

        #region IPoolFactory<K> Members

        private int refreshTime = 300000;

        public int RefreshTime {
            get { return refreshTime; }
            set { refreshTime = value; }
        }

        public virtual KeyValuePool CreateKeyValuePool(K key) {
            KeyValuePool pool = new KeyValuePool(new StackPoolStaregy(), key,
                this);
            new PoolRefreshThread(pool, new LRUPoolRefreshStaregy(TimeSpan.FromMilliseconds(refreshTime))).Start();
            return pool;
        }

        #endregion
    }
}
