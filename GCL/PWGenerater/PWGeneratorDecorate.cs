using System;
using System.Collections.Generic;
using System.Text;

namespace PWGenerater {
    public class PWGeneratorDecorate : IPassWordGenerator {

        private object[] paras;
        public PWGeneratorDecorate(object[] paras) {
            if (paras == null || paras.Length < 1)
                throw new InvalidOperationException("默认参数为空!");
            foreach (object a in paras)
                if (a == null)
                    throw new InvalidOperationException("不允许有空对象！");
                else if (!(a is IPassWordGenerator))
                    throw new InvalidOperationException(a.GetType().ToString() + "不是IPassWordGenerator的子类!");
            this.paras = paras;
        }
        #region IPassWordGenerator Members

        public string GeneratePwd(string source) {
            foreach (object o in paras)
                source = ((IPassWordGenerator)o).GeneratePwd(source);
            return source;
        }

        #endregion
    }
}
