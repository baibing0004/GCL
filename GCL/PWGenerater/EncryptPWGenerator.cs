using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PWGenerater {
    public class EncryptPWGenerator : IPassWordGenerator {


        private dllEncrypt en;
        public EncryptPWGenerator(string key, string iv) {
            en = new dllEncrypt(key, iv);
        }
        #region IPassWordGenerator Members

        public string GeneratePwd(string source) {
            return en.EncryptString(source);
        }

        #endregion
    }
}
