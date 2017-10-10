using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

//ObjectDb CacheModuler VJ3层 ESB workflow  
namespace GCL.Db.Ni.ObjectDB {

    public class ObjectDbParameters : GCL.Db.Ni.NoSQL.NoSQLParameters {
        public override object Clone() {
            return this.Clone(new ObjectDbParameters());
        }
    }
}
