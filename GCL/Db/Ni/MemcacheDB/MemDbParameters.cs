using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

//MemDb CacheModuler VJ3层 ESB workflow  
namespace GCL.Db.Ni.MemcacheDB {

    public class MemDbParameters : GCL.Db.Ni.NoSQL.NoSQLParameters {
        public override object Clone() {
            return this.Clone(new MemDbParameters());
        }
    }
}
