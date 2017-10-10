using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GCL.Project.VESH.Db.Ni.WebSocketDB {
    public class WebSocketDbReader : GCL.Db.Ni.NoSQL.NoSQLReader {
        public WebSocketDbReader(System.Data.DataSet dataSet, System.Data.CommandBehavior behavior) : base(dataSet, behavior) { }

        public WebSocketDbReader(System.Data.DataSet dataSet) : base(dataSet) { }

        public WebSocketDbReader(DataTable dt) : base(dt) { }
    }
}
