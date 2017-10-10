using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Bean.Middler {
    public class InstanceObjectContainer : AObjectContainer {
        public InstanceObjectContainer(AObjectCreater creater) : base(creater) { }


        public override object GetValue() {
            return this.CreateObject();
        }

        public override void SetValue(object v) {
            if (v is IClosable) {
                IClosable close = v as IClosable;
                try {
                    close.Close();
                } catch {
                }
                try {
                    close.Dispose();
                } catch {
                }
            } else BeanTool.Close(v);
        }

        public override void Dispose() {
        }

        public override void Close() {
        }
    }
}
