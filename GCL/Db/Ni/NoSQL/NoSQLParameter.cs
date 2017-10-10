using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GCL.Db.Ni.NoSQL {
    public abstract class NoSQLParameter : IDbDataParameter, ICloneable {
        public virtual byte Precision {
            get;
            set;
        }

        public virtual byte Scale {
            get;
            set;
        }

        public virtual int Size {
            get;
            set;
        }

        public virtual DbType DbType {
            get;
            set;
        }

        public virtual ParameterDirection Direction {
            get;
            set;
        }

        public virtual bool IsNullable {
            get;
            set;
        }

        public virtual string ParameterName {
            get;
            set;
        }

        public virtual string SourceColumn {
            get;
            set;
        }

        public virtual DataRowVersion SourceVersion {
            get;
            set;
        }

        protected object val;
        public virtual object Value {
            get { return val; }
            set {
                if (DBNull.Value.Equals(value)) {
                    switch (DbType) {
                        case System.Data.DbType.AnsiString:
                        case System.Data.DbType.String:
                        case System.Data.DbType.Xml:
                            this.val = string.Empty;
                            break;
                        case System.Data.DbType.Int64:
                            this.val = 0L;
                            break;
                        case System.Data.DbType.Int32:
                            this.val = 0;
                            break;
                        case System.Data.DbType.Int16:
                            this.val = Convert.ToInt16(0);
                            break;
                        case System.Data.DbType.Date:
                            this.val = DateTime.Now.Date;
                            break;
                        case System.Data.DbType.Time:
                        case System.Data.DbType.DateTime:
                            this.val = DateTime.Now;
                            break;
                        case System.Data.DbType.DateTime2:
                        case System.Data.DbType.DateTimeOffset:
                            this.val = DateTime.Now.ToFileTimeUtc();
                            break;
                        case System.Data.DbType.Decimal:
                            this.val = new decimal(0);
                            break;
                        case System.Data.DbType.Double:
                        case System.Data.DbType.VarNumeric:
                            this.val = double.Parse("0");
                            break;
                        case System.Data.DbType.Guid:
                            this.val = Guid.Empty;
                            break;
                        case System.Data.DbType.SByte:
                            this.val = new byte();
                            break;
                        case System.Data.DbType.Single:
                            this.val = Convert.ToSingle(0);
                            break;
                        case System.Data.DbType.UInt16:
                            this.val = Convert.ToUInt16(0);
                            break;
                        case System.Data.DbType.UInt32:
                            this.val = Convert.ToUInt32(0);
                            break;
                        case System.Data.DbType.UInt64:
                            this.val = Convert.ToUInt64(0);
                            break;
                        default:
                            throw new InvalidOperationException("不支持的DB类型" + DbType.ToString());
                    }
                } else {
                    switch (DbType) {
                        case System.Data.DbType.AnsiString:
                        case System.Data.DbType.String:
                        case System.Data.DbType.Xml:
                            this.val = Convert.ToString(value);
                            break;
                        case System.Data.DbType.Int64:
                            this.val = Convert.ToInt64(value);
                            break;
                        case System.Data.DbType.Int32:
                            this.val = Convert.ToInt32(value);
                            break;
                        case System.Data.DbType.Int16:
                            this.val = Convert.ToInt16(value);
                            break;
                        case System.Data.DbType.Date:
                            this.val = Convert.ToDateTime(value).Date;
                            break;
                        case System.Data.DbType.Time:
                        case System.Data.DbType.DateTime:
                            this.val = Convert.ToDateTime(value);
                            break;
                        case System.Data.DbType.DateTime2:
                        case System.Data.DbType.DateTimeOffset:
                            this.val = Convert.ToDateTime(value).ToFileTimeUtc();
                            break;
                        case System.Data.DbType.Decimal:
                            this.val = Convert.ToDecimal(value);
                            break;
                        case System.Data.DbType.Double:
                        case System.Data.DbType.VarNumeric:
                            this.val = Convert.ToDouble(value);
                            break;
                        case System.Data.DbType.Guid:
                            Guid guid = Guid.Empty;
                            if (Guid.TryParse(Convert.ToString(value), out guid)) {
                                this.val = guid;
                            } else
                                this.val = Guid.Empty;
                            break;
                        case System.Data.DbType.SByte:
                            this.val = Convert.ToSByte(value);
                            break;
                        case System.Data.DbType.Single:
                            this.val = Convert.ToSingle(value);
                            break;
                        case System.Data.DbType.UInt16:
                            this.val = Convert.ToUInt16(value);
                            break;
                        case System.Data.DbType.UInt32:
                            this.val = Convert.ToUInt32(value);
                            break;
                        case System.Data.DbType.UInt64:
                            this.val = Convert.ToUInt64(value);
                            break;
                        default:
                            throw new InvalidOperationException("不支持的DB类型" + DbType.ToString());
                    }
                }
            }
        }

        public abstract object Clone();
    }
}
