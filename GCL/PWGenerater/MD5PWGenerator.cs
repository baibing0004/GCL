using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PWGenerater {
    public class MD5PWGenerator : IPassWordGenerator {

        #region IPassWordGenerator Members

        public string GeneratePwd(string source) {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(source, "MD5");
        }

        #endregion
    }
}
