using System;
using System.Collections.Generic;
using System.Text;
using GCL.Collections.Pool;

namespace GCL.Bean.Middler {
    /// <summary>
    /// 池方式对象容器
    /// </summary>
    public class PoolObjectContainer:AObjectContainer,IPoolValueFactory {
        private RefreshPool pool = null;
        private TimeSpan timeSpan;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="waitTime"></param>
        /// <param name="creater"></param>
        public PoolObjectContainer(int size,int waitTime,int timeout,AObjectCreater creater)
            : base(creater) {
            pool = new RefreshPool(size,"",this,waitTime);
            timeSpan = TimeSpan.FromMilliseconds(timeout);
        }

        public override object GetValue() {
            return pool.Get(timeSpan);
        }

        public override void SetValue(object v) {
            pool.Set(v);
        }

        public override void Dispose() {
            try {
                this.Close();
            } catch {
            }
        }

        public override void Close() {
            this.pool.Close();
            try {
                this.pool.Thread.GetThread().Join();
            } catch {
            }
        }

        #region IPoolValueFactory Members

        public object CreateObject(object e) {
            return this.CreateObject();
        }

        public void CloseObject(object obj) {
            if(obj is IClosable) {
                IClosable close = obj as IClosable;
                try {
                    close.Close();
                } catch {
                }
                try {
                    close.Dispose();
                } catch {
                }
            } else BeanTool.Close(obj);
        }

        ~PoolObjectContainer() {
            this.Dispose();
        }
        #endregion
    }
}
