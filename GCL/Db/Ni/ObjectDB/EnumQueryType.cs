using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCL.Db.Ni.ObjectDB {
    /// <summary>
    /// 用于记录Command的调用
    /// </summary>
    public enum EnumQueryType {
        ExecuteNonQuery,
        ExecuteScalar,
        ExecuteReader,
        ExecuteQuery,
        Undefined
    }
}
