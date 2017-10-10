using System;
using System.Collections.Generic;
using System.Text;
using GCL.Module.Trigger;
namespace GCL.IO.Log {
    /// <summary>
    /// 太矛盾了这里要求既是Trigger又是INameTrigger
    /// </summary>
    public interface INameTrigger {
        string Taste(string file);
    }
}
