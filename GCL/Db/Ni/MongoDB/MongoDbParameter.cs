using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCL.Db.Ni.MongoDB {
    public class MongoDbParameter : GCL.Db.Ni.NoSQL.NoSQLParameter {
        public override object Clone() {
            return new MongoDbParameter {
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
