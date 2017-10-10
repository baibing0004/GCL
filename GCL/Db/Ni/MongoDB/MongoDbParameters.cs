using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCL.Db.Ni.MongoDB {
    public class MongoDbParameters : GCL.Db.Ni.NoSQL.NoSQLParameters {
        public override object Clone() {
            return this.Clone(new MongoDbParameters());
        }
    }
}
