﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
namespace GCL.Db.Ni {
    public class ODBCFactory : IDataAbstractFactory {

        #region IDataAbstractFactory Members

        public IDbConnection CreateConnection() {
            return new OdbcConnection();
        }

        #endregion

        #region IDataAbstractBase Members

        public IDbCommand CreateCommand() {
            return new OdbcCommand();
        }

        public IDbDataAdapter CreateAdapter() {
            return new OdbcDataAdapter();
        }

        public IDbDataParameter CreateParameter() {
            return new OdbcParameter();
        }

        private static Type staticOdbcDbType = typeof(System.Data.Odbc.OdbcType);
        private static Type staticDbType = typeof(System.Data.DbType);
        public System.Data.DbType ParseType(string type) {
            try {
                //默认使用默认类型转换
                /*
                    AnsiString	非 Unicode 字符的可变长度流，范围在 1 到 8,000 个字符之间。 
                    AnsiStringFixedLength	非 Unicode 字符的固定长度流。 
                    Binary	二进制数据的可变长度流，范围在 1 到 8,000 个字节之间。如果字节数组多于 8,000 个字节，则 ADO.NET 无法正确推断出类型。当处理多于 8,000 个字节的字节数组时，请显式指定 DbType。
                    Boolean	简单类型，表示 true 或 false 的布尔值。 
                    Byte	一个 8 位无符号整数，范围在 0 到 255 之间。 
                    Currency	货币值，范围在 -2 63（即 -922,337,203,685,477.5808）到 2 63 -1（即 +922,337,203,685,477.5807）之间，精度为千分之十个货币单位。 
                    Date	日期和时间数据，值范围从 1753 年 1 月 1 日到 9999 年 12 月 31 日，精度为 3.33 毫秒。 
                    DateTime	表示一个日期和时间值的类型。 
                    Decimal	简单类型，表示从 1.0 x 10 -28 到大约 7.9 x 10 28 且有效位数为 28 到 29 位的值。 
                    Double	浮点型，表示从大约 5.0 x 10 -324 到 1.7 x 10 308 且精度为 15 到 16 位的值。 
                    Guid	全局唯一标识符（或 GUID）。 
                    Int16	整型，表示值介于 -32768 到 32767 之间的有符号 16 位整数。 
                    Int32	整型，表示值介于 -2147483648 到 2147483647 之间的有符号 32 位整数。 
                    Int64	整型，表示值介于 -9223372036854775808 到 9223372036854775807 之间的有符号 64 位整数。 
                    Object	常规类型，表示任何没有由其他 DbType 值显式表示的引用或值类型。 
                    SByte	整型，表示值介于 -128 到 127 之间的有符号 8 位整数。 
                    Single	浮点型，表示从大约 1.5 x 10 -45 到 3.4 x 10 38 且精度为 7 位的值。 
                    String	表示 Unicode 字符串的类型。 
                    StringFixedLength	 
                    Time	日期和时间数据，值范围从 1753 年 1 月 1 日到 9999 年 12 月 31 日，精度为 3.33 毫秒。 
                    UInt16	整型，表示值介于 0 到 65535 之间的无符号 16 位整数。 
                    UInt32	整型，表示值介于 0 到 4294967295 之间的无符号 32 位整数。 
                    UInt64	整型，表示值介于 0 到 18446744073709551615 之间的无符号 64 位整数。 
                    VarNumeric	变长数值。 
                    Xml	XML 文档或片段的分析表示。 
                 */
                return ((DbType)staticDbType.GetField(type,
                        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Public).GetValue(null));
            } catch {
                /*	成员名称	说明
	                BigInt	确切数值，其精度为 19 （如果有符号）或 20 （如果没有符号），小数位数为 0（有符号：-2[63] <= n <= 2[63] - 1，没有符号：0 <= n <= 2[64] - 1）(SQL_BIGINT)。它映射到 Int64。 
	                Binary	二进制数据流 (SQL_BINARY)。它映射到 Byte 类型的 Array。 
	                Bit	只有一位的二进制数据 (SQL_BIT)。它映射到 Boolean。 
	                Char	固定长度字符串 (SQL_CHAR)。它映射到 String。 
	                Date	格式为 yyyymmdd 的日期数据 (SQL_TYPE_DATE)。它映射到 DateTime。 
	                DateTime	格式为 yyyymmddhhmmss 的日期数据 (SQL_TYPE_TIMESTAMP)。它映射到 DateTime。 
	                Decimal	有符号的确切数值，其精度至少为 p，小数位数为 s，其中 1 <= p <= 15 并且 s <= p。最大精度因驱动程序而定 (SQL_DECIMAL)。它映射到 Decimal。 
	                Double	有符号的近似数值，其二进制精度为 53 （零或绝对值为 10[-308] 到 10[308]） (SQL_DOUBLE)。它映射到 Double。 
	                Image	变长二进制数据。最大长度因数据源而定 (SQL_LONGVARBINARY)。它映射到 Byte 类型的 Array。 
	                Int	确切数值，其精度为 10 和小数位数 0（有符号：-2[31] <= n <= 2[31] - 1，没有符号：0 <= n <= 2[32] - 1）(SQL_INTEGER)。它映射到 Int32。 
	                NChar	固定长度的 Unicode 字符串 (SQL_WCHAR)。它映射到 String。 
	                NText	Unicode 变长字符数据。最大长度因数据源而定。(SQL_WLONGVARCHAR)。它映射到 String。 
	                Numeric	有符号的确切数值，其精度为 p，小数位数为 s，其中 1 <= p <= 15 并且 s <= p (SQL_NUMERIC)。它映射到 Decimal。 
	                NVarChar	Unicode 字符的变长流 (SQL_WVARCHAR)。它映射到 String。 
	                Real	有符号的近似数值，其二进制精度为 24 （零或绝对值为 10[-38] 到 10[38]）。(SQL_REAL)。它映射到 Single。 
	                SmallDateTime	格式为 yyyymmddhhmmss 的数据和时间数据 (SQL_TYPE_TIMESTAMP)。它映射到 DateTime。 
	                SmallInt	确切数值，其精度为 5，小数位数为 0 （有符号：-32,768 <= n <= 32,767，没有符号：0 <= n <= 65,535）(SQL_SMALLINT)。它映射到 Int16。 
	                Text	变长字符数据。最大长度因数据源而定 (SQL_LONGVARCHAR)。它映射到 String。 
	                Time	格式为 hhmmss 的日期数据 (SQL_TYPE_TIMES)。它映射到 DateTime。 
	                Timestamp	二进制数据流 (SQL_BINARY)。它映射到 Byte 类型的 Array。 
	                TinyInt	确切数值，其精度为 3，小数位数为 0 （有符号：-128 <= n <= 127，没有符号：0 <= n <= 255）(SQL_TINYINT)。它映射到 Byte。 
	                UniqueIdentifier	固定长度的 GUID (SQL_GUID)。它映射到 Guid。 
	                VarBinary	变长二进制。由用户设置该最大值 (SQL_VARBINARY)。它映射到 Byte 类型的 Array。 
	                VarChar	变长流字符串 (SQL_CHAR)。它映射到 String。 
                */
                switch ((OdbcType)staticOdbcDbType.GetField(type,
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Public).GetValue(null)) {
                    case OdbcType.BigInt:
                        return DbType.Int64;
                    case OdbcType.Binary:
                        return DbType.Binary;
                    case OdbcType.Bit:
                        return DbType.Boolean;
                    case OdbcType.Char:
                        return DbType.String;
                    case OdbcType.Date:
                        return DbType.Date;
                    case OdbcType.DateTime:
                        return DbType.DateTime;
                    case OdbcType.Decimal:
                        return DbType.Decimal;
                    case OdbcType.Double:
                        return DbType.Double;
                    case OdbcType.Image:
                        return DbType.Binary;
                    case OdbcType.Int:
                        return DbType.Int32;
                    case OdbcType.NChar:
                        return DbType.String;
                    case OdbcType.NText:
                        return DbType.String;
                    case OdbcType.Numeric:
                        return DbType.Decimal;
                    case OdbcType.NVarChar:
                        return DbType.String;
                    case OdbcType.Real:
                        return DbType.Single;
                    case OdbcType.SmallDateTime:
                        return DbType.DateTime;
                    case OdbcType.SmallInt:
                        return DbType.Int16;
                    case OdbcType.Text:
                        return DbType.String;
                    case OdbcType.Time:
                        return DbType.DateTime;
                    case OdbcType.Timestamp:
                        return DbType.Binary;
                    case OdbcType.TinyInt:
                        return DbType.Byte;
                    case OdbcType.UniqueIdentifier:
                        return DbType.Guid;
                    case OdbcType.VarBinary:
                        return DbType.Binary;
                    case OdbcType.VarChar:
                        return DbType.String;
                    default:
                        throw new InvalidOperationException(string.Format("DbType与ODBCType中没有找到这个类型!{0}", type));
                }
            }

        }

        #endregion

        public string ParamSign {
            get { return ""; }
        }
    }
}
