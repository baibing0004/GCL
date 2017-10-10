using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PWGenerater {
    public class GuidSourceGenerator : ISourceGenerator {

        #region ISourceGenerator Members

        public string GenerateSource() {
            return Guid.NewGuid().ToString();
        }

        #endregion
    }
}
