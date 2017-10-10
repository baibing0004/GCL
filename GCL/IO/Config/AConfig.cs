
namespace GCL.IO.Config {
    /// <summary>
    ///负责具体的获取与设置相关信息
    ///
    ///@author 白冰
    ///
    ///@version 2.0.81212.1
    /// </summary>
    public abstract class AConfig {

        public abstract object GetValue(object key);

        public abstract void SetValue(object key, object value);

        public abstract void Merge(AConfig config);
        /// <summary>
        ///提供是否可以递归调用
        /// </summary>
        private bool allowCascade = true;

        internal bool IsAllowCascade() {
            return this.allowCascade;
        }

        private bool isChangeValue = false;

        /// <summary>
        ///用于判断是否属性改变
        ///
        ///@return isChangeValue
        /// </summary>
        internal bool ChangeValue() {
            return isChangeValue;
        }

        /// <summary>
        ///@param isChangeValue
        ///           要设置的 isChangeValue
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
        ///           要设置的 allowChangeValue
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