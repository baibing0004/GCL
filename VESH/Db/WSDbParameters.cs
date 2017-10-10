using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

//WebSocketDb CacheModuler VJ3层 ESB workflow  
namespace GCL.Project.VESH.Db.Ni.WebSocketDB {

    public class WebSocketDbParameters : GCL.Db.Ni.NoSQL.NoSQLParameters {
        public override object Clone() {
            return this.Clone(new WebSocketDbParameters());
        }
    }
}
