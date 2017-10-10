using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

//RedisDB CacheModuler VJ3层 ESB workflow  
namespace GCL.Db.Ni.RedisDB {

    public class RedisDbParameters : GCL.Db.Ni.NoSQL.NoSQLParameters {
        public override object Clone() {
            return this.Clone(new RedisDbParameters());
        }
    }
}
