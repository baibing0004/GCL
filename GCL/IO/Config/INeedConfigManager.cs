using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.IO.Config {
    /// <summary>
    /// 用于定义AConfigConvert是否需要ConfigManager等用于设置参数，严重慎用
    /// </summary>
    public interface INeedConfigManager {
        void SetConfigManager(ConfigManager config);
    }
}
