using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GCL.Db.Ni.NoSQL {
    public abstract class NoSQLReader : IDataReader {
        protected System.Data.DataSet dataSet;
        protected System.Data.CommandBehavior behavior;
        protected DataTable dt;
        protected DataRow dr;

        public NoSQLReader(System.Data.DataSet dataSet, System.Data.CommandBehavior behavior)
            : this(dataSet) {
            this.behavior = behavior;
        }

        public NoSQLReader(System.Data.DataSet dataSet) {
            this.dataSet = dataSet;
            if (dataSet.Tables.Count == 0) { dt = null; dr = null; } else { TableIndex = 0; }
            this.IsClosed = false;
        }

        public NoSQLReader(DataTable dt) {
            this.dt = dt;
        }

        public virtual void Close() {
            this.dataSet.Clear();
            this.dt = null;
            this.dr = null;
            this.IsClosed = true;
        }

        protected int tbIndex;
        public virtual int TableIndex {
            get { return this.tbIndex; }
            set {
                if (this.dataSet.Tables.Count > value) {
                    this.dt = this.dataSet.Tables[value];
                    this.tbIndex = value;
                    this.rowIndex = -1;
                } else throw new IndexOutOfRangeException("超出表的最大值");
            }
        }


        protected int rowIndex;
        public virtual int RowIndex {
            get { return rowIndex; }
            set { if (dt.Rows.Count <= value) { rowIndex = dt.Rows.Count - 1; } else { rowIndex = value; } if (rowIndex != -1) dr = dt.Rows[rowIndex]; else dr = null; }
        }

        public virtual DataTable CurrentTable {
            get {
                if (dt != null && !this.IsClosed) return dt; else throw new InvalidOperationException("数据已经关闭，无法返回列的值");
            }
        }

        public virtual DataRow CurrentRow {
            get {
                if (dr != null && !this.IsClosed) return dr; else throw new InvalidOperationException("数据已经关闭，无法返回列的值");
            }
        }

        public virtual int Depth {
            get { return 0; }
        }

        public virtual DataTable GetSchemaTable() {
            if (this.IsClosed) { throw new InvalidOperationException("数据源已经读取完毕，关闭"); }
            var dr = dt.CreateDataReader();
            try {
                return dr.GetSchemaTable();
            } finally {
                dr.Close();
                dr = null;
            }
        }

        public virtual bool IsClosed {
            get;
            protected set;
        }

        protected object _rowkey = DateTime.Now;
        public virtual bool NextResult() {
            lock (_rowkey) {
                RowIndex++;
                if (RowIndex == -1) {
                    //自动巡航下一个表
                    try {
                        //自动处理第一次访问
                        TableIndex++;
                        Read();
                    } catch (IndexOutOfRangeException) {
                        this.IsClosed = true;
                        return false;
                    }
                }
                return true;
            };
        }

        public virtual bool Read() {
            lock (_rowkey) {
                return RowIndex++ == -1;
            }
        }

        public virtual int RecordsAffected {
            get { return 0; }
        }

        public virtual void Dispose() {
            if (dataSet != null) {
                lock (dataSet) {
                    if (dataSet != null) {
                        dataSet.Clear();
                        dataSet = null;
                        IsClosed = true;
                    }
                }
            }
        }

        public virtual int FieldCount {
            get { if (dt != null) return dt.Columns.Count; else throw new InvalidOperationException("数据已经关闭，无法返回列的值"); }
        }

        public virtual bool GetBoolean(int i) {
            return Convert.ToBoolean(CurrentRow[i]);
        }

        public virtual byte GetByte(int i) {
            return Convert.ToByte(CurrentRow[i]);
        }

        public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) {
            var vals = (CurrentRow[i] as byte[]).Skip(Convert.ToInt32(fieldOffset)).Take(length).ToArray();
            vals.CopyTo(buffer, bufferoffset);
            return vals.LongLength;
        }

        public virtual char GetChar(int i) {
            return Convert.ToChar(CurrentRow[i]);
        }

        public virtual long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length) {
            var vals = (Convert.ToString(CurrentRow[i]).ToCharArray()).Skip(Convert.ToInt32(fieldOffset)).Take(length).ToArray();
            vals.CopyTo(buffer, bufferoffset);
            return vals.LongLength;
        }

        public virtual IDataReader GetData(int i) {
            throw new InvalidOperationException("暂时不支持此方法!");
        }

        public virtual string GetDataTypeName(int i) {
            return CurrentTable.Columns[i].DataType.FullName;
        }

        public virtual DateTime GetDateTime(int i) {
            return Convert.ToDateTime(CurrentRow[i]);
        }

        public virtual decimal GetDecimal(int i) {
            return Convert.ToDecimal(CurrentRow[i]);
        }

        public virtual double GetDouble(int i) {
            return Convert.ToDouble(CurrentRow[i]);
        }

        public virtual Type GetFieldType(int i) {
            return CurrentTable.Columns[i].DataType.GetType();
        }

        public virtual float GetFloat(int i) {
            return Convert.ToSingle(CurrentRow[i]);
        }

        public virtual Guid GetGuid(int i) {
            return Guid.Parse(Convert.ToString(CurrentRow[i]));
        }

        public virtual short GetInt16(int i) {
            return Convert.ToInt16(CurrentRow[i]);
        }

        public virtual int GetInt32(int i) {
            return Convert.ToInt32(CurrentRow[i]);
        }

        public virtual long GetInt64(int i) {
            return Convert.ToInt64(CurrentRow[i]);
        }

        public virtual string GetName(int i) {
            return CurrentTable.Columns[i].ColumnName;
        }

        public virtual int GetOrdinal(string name) {
            return Convert.ToInt32(CurrentRow[name]);
        }

        public virtual string GetString(int i) {
            return Convert.ToString(CurrentRow[i]);
        }

        public virtual object GetValue(int i) {
            return CurrentRow[i];
        }

        public virtual int GetValues(object[] values) {
            throw new NotImplementedException();
        }

        public virtual bool IsDBNull(int i) {
            return DBNull.Value.Equals(GetValue(i));
        }

        public virtual object this[string name] {
            get { return CurrentRow[name]; }
        }

        public virtual object this[int i] {
            get { return GetValue(i); }
        }

    }
}
