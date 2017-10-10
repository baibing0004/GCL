using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PWGenerater {
    public class DateTimeSourceGenerator:ISourceGenerator {

        private string format = PublicClass.Common.Tool.yyyy_MM_dd_HH_mm_ss_S;
        public DateTimeSourceGenerator(string format) {
            this.format = format;
        }
        public DateTimeSourceGenerator() { }

        #region ISourceGenerator Members

        public string GenerateSource() {
            return DateTime.Now.ToString(format);
        }

        #endregion
    }
}
