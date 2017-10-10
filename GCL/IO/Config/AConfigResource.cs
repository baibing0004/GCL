using System;
using GCL.Module.Trigger;

namespace GCL.IO.Config {
    /// <summary>
    ///负责管理数据源和通知Manager数据源的改变和保存Manager到数据源
    ///
    ///@author 白冰
    ///
    /// </summary>
    public abstract class AConfigResource : TriggerProxy {
        public AConfigResource(ATrigger trigger)
            : base(trigger) {
        }

        protected AConfigResource() : base() { }

        public abstract string Load();

        public abstract void Save(string value);
    }
}