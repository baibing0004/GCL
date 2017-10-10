using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Bean.Middler {
    public class StaticObjectContainer : AObjectContainer {

        private object value = null;
        /// <summary>
        /// 静态方式保存的对象
        /// </summary>
        /// <param name="creater"></param>
        public StaticObjectContainer(AObjectCreater creater) : base(creater) { }

        private object key = DateTime.Now;
        public override object GetValue() {
            if (value == null)
                lock (key) {
                    if (value == null)
                        value = this.CreateObject();
                }
            return value;
        }

        public override void SetValue(object v) {
        }

        public override void Dispose() {
            try {
                this.Close();
            } catch {
            }
        }

        public override void Close() {
            if (this.value is IClosable) {
                IClosable close = value as IClosable;
                try {
                    close.Close();
                } catch {
                }
                try {
                    close.Dispose();
                } catch {
                }
            } else BeanTool.Close(value);
        }
    }
}
