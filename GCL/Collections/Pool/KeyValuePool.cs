using System;
using System.Collections;
using GCL.Collections;
using GCL.Event;
using GCL.Common;
namespace GCL.Collections.Pool {



    ///
    /// @author baibing
    /// ������ֵ�� ���ݵ������ز�����ʱ���½����󲢸�����������
    ///
    public class KeyValuePool : Pool {

        private IPoolValueFactory pvFactory;

        private object key;

        ///
        /// @param poolStaregy
        /// @param valueSet
        /// @param size
        ///
        public KeyValuePool(IPoolStaregy poolStaregy, IList valueSet, int size,
                object key, IPoolValueFactory pvFactory)
            : base(poolStaregy, valueSet, size) {
            this.key = key;
            this.pvFactory = pvFactory;
        }

        ///
        /// @param poolStaregy
        /// @param valueSet
        ///
        public KeyValuePool(IPoolStaregy poolStaregy, IList valueSet, object key,
                IPoolValueFactory pvFactory)
            : base(poolStaregy, valueSet) {
            this.key = key;
            this.pvFactory = pvFactory;

        }


        ///
        /// @param poolStaregy
        ///
        public KeyValuePool(IPoolStaregy poolStaregy, object key,
                IPoolValueFactory pvFactory)
            : base(poolStaregy) {
            this.key = key;
            this.pvFactory = pvFactory;

        }

        ///
        /// @param poolStaregy
        /// @param valueSet
        /// @param num
        ///
        public KeyValuePool(IPoolStaregy poolStaregy, IList valueSet, LimitNum num,
                object key, IPoolValueFactory pvFactory)
            : base(poolStaregy, valueSet, num) {
            this.key = key;
            this.pvFactory = pvFactory;

        }

        ///
        /// @param poolStaregy
        /// @param valueSet
        /// @param size
        ///
        public KeyValuePool(IPoolStaregy poolStaregy, int size,
                object key, IPoolValueFactory pvFactory)
            : base(poolStaregy, size) {
            this.key = key;
            this.pvFactory = pvFactory;
        }

        ///
        /// @return key
        ///
        public object GetKey() {
            return key;
        }

        ///
        /// @param key
        ///            Ҫ���õ� key
        ///
        public void SetKey(object key) {
            this.key = key;
        }

        public object Key {
            get {
                return this.GetKey();
            }
            set {
                this.SetKey(value);
            }
        }

        ///
        /// @return pvFactory
        ///
        public IPoolValueFactory GetPoolValueFactory() {
            return pvFactory;
        }

        ///
        /// @param pvFactory
        ///            Ҫ���õ� pvFactory
        ///
        public void SetPoolValueFactory(IPoolValueFactory pvFactory) {
            if (!Tool.IsEnable(this.pvFactory))
                this.pvFactory = pvFactory;
        }

        public IPoolValueFactory PoolValueFactory {
            get {
                return GetPoolValueFactory();
            }
            set {
                this.SetPoolValueFactory(value);
            }
        }


        ///
        /// ���� Javadoc��
        /// 
        /// @see GCL.Collections.Pool.Pool#get()
        ///
        public override object Get() {
            lock (this) {
                try {
                    return base.Get();
                } catch (IndexOutOfRangeException e) {
                    if (!IsFull()) {
                        int times = 0;
                        Exception ex = null;
                        object value = null;
                        do {
                            times++;
                            try {
                                value = this.pvFactory.CreateObject(this.key);
                            } catch (Exception e2) {
                                ex = e2;
                            }
                        } while (value == null && times < 3);
                        if (value != null) {
                            Set(value);
                            return base.Get();
                        } else
                            throw ex != null ? ex : new Exception("δ�ܳɹ���ö���!");
                    } else
                        throw e;
                }
            }
        }
        ///
        /// ���� Javadoc��
        /// 
        /// @see GCL.Collections.Pool.Pool#objectClose(java.lang.object)
        ///
        protected internal override void ObjectClose(object value) {
            base.ObjectClose(value);
            if (Tool.IsEnable(value))
                try {
                    this.pvFactory.CloseObject(value);
                } catch (Exception e1) {
                }
        }
    }
}
