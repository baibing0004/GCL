using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PWGenerater {
    public class SouGeneratorHashDecorate:ISourceGenerator {

        private ISourceGenerator sou;
        public SouGeneratorHashDecorate(ISourceGenerator gen) {
            sou = gen;
        }

        #region ISourceGenerator Members

        public string GenerateSource() {
            return sou.GenerateSource().GetHashCode().ToString();
        }

        #endregion
    }
}
