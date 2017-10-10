using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace GCL.IO.Config {
    /// <summary>
    /// 生成ConnectionStrings转换器
    /// </summary>
    public class ConnectionStringsConvert : ADictionaryConfigConvert {
        public ConnectionStringsConvert() : base("ConnectionStrings", true) { }
        public override DictionaryConfig CreateInstance(bool ignorecase) {
            return new DictionaryConfig(true, true, ignorecase);
        }
    }
}
