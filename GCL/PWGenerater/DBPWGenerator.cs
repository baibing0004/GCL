using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PWGenerater {
    public class DBPWGenerator : PWGenerater.IPassWordGenerator {

        #region IPassWordGenerator Members

        public string GeneratePwd(string source) {
            return Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(source)).TrimEnd('=');
        }

        #endregion
    }
}
