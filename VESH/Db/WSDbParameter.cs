using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GCL.Project.VESH.Db.Ni.WebSocketDB {
    public class WebSocketDbParameter : GCL.Db.Ni.NoSQL.NoSQLParameter {
        public override object Clone() {
            return new WebSocketDbParameter {
                DbType = this.DbType,
                Direction = this.Direction,
                IsNullable = this.IsNullable,
                ParameterName = this.ParameterName,
                Precision = this.Precision,
                Scale = this.Scale,
                Size = this.Size,
                SourceColumn = this.SourceColumn,
                SourceVersion = this.SourceVersion,
                Value = this.Value
            };
        }
    }
}
