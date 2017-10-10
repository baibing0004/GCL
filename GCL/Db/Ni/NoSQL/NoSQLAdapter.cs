using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GCL.Db.Ni.NoSQL {
    public abstract class NoSQLAdapter : IDbDataAdapter {
        protected ITableMappingCollection _tableMappings;

        public virtual IDbCommand DeleteCommand {
            get;
            set;
        }

        public virtual IDbCommand InsertCommand {
            get;
            set;
        }

        public virtual IDbCommand SelectCommand {
            get;
            set;
        }

        public virtual IDbCommand UpdateCommand {
            get;
            set;
        }

        public virtual int Fill(DataSet dataSet) {
            if (dataSet == null) throw new InvalidOperationException("DataSet不能为空!");
            if (this.SelectCommand == null) throw new InvalidOperationException("SelectCommand对象不能为空!");
            dataSet.Merge((this.SelectCommand as NoSQLCommand).ExecuteQuery());
            return dataSet.Tables.Count;
        }

        public virtual DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType) {
            throw new NotImplementedException("FillSchema");
        }

        public virtual IDataParameter[] GetFillParameters() {
            if (this.SelectCommand == null) throw new InvalidOperationException("SelectCommand对象不能为空!");
            lock (this.SelectCommand.Parameters.SyncRoot) {
                IDataParameter[] paras = new IDataParameter[this.SelectCommand.Parameters.Count];
                this.SelectCommand.Parameters.CopyTo(paras, 0);
                return paras;
            }
        }

        public virtual MissingMappingAction MissingMappingAction {
            get;
            set;
        }

        public virtual MissingSchemaAction MissingSchemaAction {
            get;
            set;
        }




        public virtual ITableMappingCollection TableMappings {
            get {
                throw new InvalidOperationException("暂不支持此属性");
            }
        }

        public virtual int Update(DataSet dataSet) {
            throw new InvalidOperationException("操作原理上不支持DataSet直接回传数据库。");
        }
    }
}
