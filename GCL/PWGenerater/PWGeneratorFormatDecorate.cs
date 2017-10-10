using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PWGenerater {
    public class PWGeneratorFormatDecorate : IPassWordGenerator {

        private string format;
        private IPassWordGenerator gen;
        public PWGeneratorFormatDecorate(IPassWordGenerator gen, string format) {
            this.gen = gen;
            this.format = format;
        }
        #region IPassWordGenerator Members

        public string GeneratePwd(string source) {
            return gen.GeneratePwd(string.Format(format, source));
        }

        #endregion
    }
}
