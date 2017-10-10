
namespace GCL.IO.Config {
    /// <summary>
    ///�������Ļ�ȡ�����������Ϣ
    ///
    ///@author �ױ�
    ///
    ///@version 2.0.81212.1
    /// </summary>
    public abstract class AConfig {

        public abstract object GetValue(object key);

        public abstract void SetValue(object key, object value);

        public abstract void Merge(AConfig config);
        /// <summary>
        ///�ṩ�Ƿ���Եݹ����
        /// </summary>
        private bool allowCascade = true;

        internal bool IsAllowCascade() {
            return this.allowCascade;
        }

        private bool isChangeValue = false;

        /// <summary>
        ///�����ж��Ƿ����Ըı�
        ///
        ///@return isChangeValue
        /// </summary>
        internal bool ChangeValue() {
            return isChangeValue;
        }

        /// <summary>
        ///@param isChangeValue
        ///           Ҫ���õ� isChangeValue
        /// </summary>
        internal void SetChangeValue(bool isChangeValue) {
            this.isChangeValue = isChangeValue;
        }

        private bool allowChangeValue = true;

        /// <summary>
        ///@return allowChangeValue
        /// </summary>
        internal bool IsAllowChangeValue() {
            return allowChangeValue;
        }

        /// <summary>
        ///@param allowChangeValue
        ///           Ҫ���õ� allowChangeValue
        /// </summary>
        internal void SetAllowChangeValue(bool allowChangeValue) {
            this.allowChangeValue = allowChangeValue;
        }

        public AConfig(bool allowCascade, bool allowChangeValue) {
            this.allowCascade = allowCascade;
            this.allowChangeValue = allowChangeValue;
        }

        public AConfig()
            : this(true, true) {
        }

    }
}