using System;
using GCL.Module.Trigger;

namespace GCL.IO.Config {
    /// <summary>
    ///�����������Դ��֪ͨManager����Դ�ĸı�ͱ���Manager������Դ
    ///
    ///@author �ױ�
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