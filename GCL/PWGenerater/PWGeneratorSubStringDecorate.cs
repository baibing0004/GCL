using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PWGenerater {
    public class PWGeneratorSubStringDecorate : IPassWordGenerator {
        private IPassWordGenerator gen;
        private int length = 1;
        private int start = 0;
        public PWGeneratorSubStringDecorate(IPassWordGenerator generator, int maxlength)
            : this(generator, 0, maxlength) {
        }
        public PWGeneratorSubStringDecorate(IPassWordGenerator generator, int start, int maxlength) {
            this.gen = generator;
            this.start = start;
            this.length = maxlength;
        }
        #region IPassWordGenerator Members

        public string GeneratePwd(string source) {
            string data = gen.GeneratePwd(source);
            if (data.Length < this.start + this.length)
                return data.Substring(this.start);
            else
                return data.Substring(this.start, this.length);
        }

        #endregion
    }
}
