using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GCL.IO.Config {
    /// <summary>
    /// 生成AppSettings转换器
    /// </summary>
    public class AppSettingsConfigConvert : ADictionaryConfigConvert {
        public AppSettingsConfigConvert() : base("AppSettings", true) { }
        public override DictionaryConfig CreateInstance(bool ignorecase) {
            return new DictionaryConfig(true, true, ignorecase);
        }
    }
}