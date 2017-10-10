using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCL.Project.VESH.V.Control.Session.Resource {
    /// <summary>
    /// 用于定义加密与解密的方式方法 Encrypt/Dncrypt英语不好请见谅
    /// </summary>
    public interface IXcrypt {
        /// <summary>
        /// 用于加密其字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Encrypt(string data);

        /// <summary>
        /// 用于解密其字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Decrypt(string data);
    }
}
