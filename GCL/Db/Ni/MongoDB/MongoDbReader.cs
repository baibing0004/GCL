using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCL.Db.Ni.MongoDB {
    public class MongoDbReader : GCL.Db.Ni.NoSQL.NoSQLReader {
        public MongoDbReader(System.Data.DataSet dataSet, System.Data.CommandBehavior behavior) : base(dataSet, behavior) { }

        public MongoDbReader(System.Data.DataSet dataSet) : base(dataSet) { }

        public MongoDbReader(DataTable dt) : base(dt) { }
    }
}
