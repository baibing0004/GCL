using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GCL.Db.Ni.MemcacheDB {
    public class MemDbReader : GCL.Db.Ni.NoSQL.NoSQLReader {
        public MemDbReader(System.Data.DataSet dataSet, System.Data.CommandBehavior behavior) : base(dataSet, behavior) { }

        public MemDbReader(System.Data.DataSet dataSet) : base(dataSet) { }

        public MemDbReader(DataTable dt) : base(dt) { }
    }
}
