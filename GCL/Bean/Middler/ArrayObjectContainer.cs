using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace GCL.Bean.Middler {
    public class ArrayObjectContainer : AObjectContainer {

        //因为Objects下面只有object
        private CreaterParameters paras;
        private ObjectStaticFactory factory;
        public ArrayObjectContainer(GCL.IO.Config.ConfigManager ma, string dll, string type)
            : base(null) {
            paras = new CreaterParameters(ma);
            if (BeanTool.IsEnable(type))
                factory = new ObjectStaticFactory(dll, type);
        }

        //工厂方法示例如下:Convert.ToString需要提供如下方法
        //public static string[] ToString(object[] array) {
        //    return Array.ConvertAll<object, string>(array, Convert.ToString);
        //}

        public void AddParameter(ACreaterParameter para) {
            paras.AddParameter(para);
        }

        public override object GetValue() {
            if (BeanTool.IsEnable(factory))
                return factory.GetObject(new object[] { paras.GetParameters() });
            else
                return paras.GetParameters();
        }

        public override void SetValue(object v) {
            if (v is Array)
                for (IEnumerator ie = ((Array)v).GetEnumerator(); ie.MoveNext(); )
                    if (ie.Current is IClosable) {
                        IClosable close = ie.Current as IClosable;
                        try {
                            close.Close();
                        } catch {
                        }
                        try {
                            close.Dispose();
                        } catch {
                        }
                    } else BeanTool.Close(ie.Current);
            else
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
            try {
                this.paras.Dispose();
            } catch {
            }
        }

        public override void Close() {
            this.paras.Close();
        }
    }
}
