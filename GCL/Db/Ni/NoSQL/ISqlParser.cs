using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCL.Db.Ni.NoSQL {
    public interface ISqlParser {
        /// <summary>
        /// 管理单条SQL的解析
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        QueryEntity ParseSigalSQL(string p, string sign);
    }
}
