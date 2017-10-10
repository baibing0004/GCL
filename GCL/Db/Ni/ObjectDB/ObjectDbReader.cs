using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GCL.Db.Ni.ObjectDB {
    public class ObjectDbReader : GCL.Db.Ni.NoSQL.NoSQLReader {
        public ObjectDbReader(System.Data.DataSet dataSet, System.Data.CommandBehavior behavior) : base(dataSet, behavior) { }

        public ObjectDbReader(System.Data.DataSet dataSet) : base(dataSet) { }

        public ObjectDbReader(DataTable dt) : base(dt) { }
    }
}
