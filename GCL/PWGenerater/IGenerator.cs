using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PWGenerater {
    public interface ISourceGenerator {
        /// <summary>
        /// 用于在默认情况下产生一个随机的源串
        /// </summary>
        /// <returns></returns>
        string GenerateSource();
    }

    public interface IPassWordGenerator {
        /// <summary>
        /// 用于根据源生成一个散列后的密码串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        string GeneratePwd(string source);
    }
}
