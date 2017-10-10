using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GCL.Db.Ni.RedisDB {
    public class RedisDbReader : GCL.Db.Ni.NoSQL.NoSQLReader {
        public RedisDbReader(System.Data.DataSet dataSet, System.Data.CommandBehavior behavior) : base(dataSet, behavior) { }

        public RedisDbReader(System.Data.DataSet dataSet) : base(dataSet) { }

        public RedisDbReader(DataTable dt) : base(dt) { }
    }
}
