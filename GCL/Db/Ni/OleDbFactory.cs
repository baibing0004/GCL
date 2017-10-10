using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
namespace GCL.Db.Ni {
    public class OleDbFactory : IDataAbstractFactory {

        #region IDataAbstractFactory Members

        public IDbConnection CreateConnection() {
            return new OleDbConnection();
        }

        #endregion

        #region IDataAbstractBase Members

        public IDbCommand CreateCommand() {
            return new OleDbCommand();
        }

        public IDbDataAdapter CreateAdapter() {
            return new OleDbDataAdapter();
        }

        public IDbDataParameter CreateParameter() {
            return new OleDbParameter();
        }

        private static Type staticOleDbType = typeof(System.Data.OleDb.OleDbType);
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
                /*
                    BigInt	64 位带符号的整数 (DBTYPE_I8)。它映射到 Int64。 
	                Binary	二进制数据流 (DBTYPE_BYTES)。它映射到 Byte 类型的 Array。 
	                Boolean	布尔值 (DBTYPE_BOOL)。它映射到 Boolean。 
	                BSTR	Unicode 字符的空终止字符串 (DBTYPE_BSTR)。它映射到 String。 
	                Char	字符串 (DBTYPE_STR)。它映射到 String。 
	                Currency	一个货币值，范围在 -2 63（或 -922,337,203,685,477.5808）到 2 63 -1（或 +922,337,203,685,477.5807）之间，精度为千分之十个货币单位 (DBTYPE_CY)。它映射到 Decimal。 
	                Date	日期数据，存储为双精度型 (DBTYPE_DATE)。整数部分是自 1899 年 12 月 30 日以来的天数，而小数部分是不足一天的部分。它映射到 DateTime。 
	                DBDate	格式为 yyyymmdd 的日期数据 (DBTYPE_DBDATE)。它映射到 DateTime。 
	                DBTime	格式为 hhmmss 的时间数据 (DBTYPE_DBTIME)。它映射到 TimeSpan。 
	                DBTimeStamp	格式为 yyyymmddhhmmss 的日期和时间数据 (DBTYPE_DBTIMESTAMP)。它映射到 DateTime。 
	                Decimal	定点精度和小数位数数值，范围在 -10 38 -1 和 10 38 -1 之间 (DBTYPE_DECIMAL)。它映射到 Decimal。 
	                Double	浮点数字，范围在 -1.79E +308 到 1.79E +308 之间 (DBTYPE_R8)。它映射到 Double。 
	                Empty	无任何值 (DBTYPE_EMPTY)。 
	                Error	32 位错误代码 (DBTYPE_ERROR)。它映射到 Exception。 
	                Filetime	64 位无符号整数，表示自 1601 年 1 月 1 日以来 100 个纳秒间隔的数字 (DBTYPE_FILETIME)。它映射到 DateTime。 
	                Guid	全局唯一标识符（或 GUID） (DBTYPE_GUID)。它映射到 Guid。 
	                IDispatch	指向 IDispatch 接口的指针 (DBTYPE_IDISPATCH)。它映射到 Object。                ADO.NET 当前不支持该数据类型。使用它可能导致不可预知的结果。
	                Integer	32 位带符号的整数 (DBTYPE_I4)。它映射到 Int32。 
	                IUnknown	指向 IUnknown 接口的指针 (DBTYPE_UNKNOWN)。它映射到 Object。                ADO.NET 当前不支持该数据类型。使用它可能导致不可预知的结果。
	                LongVarBinary	长的二进制值（只限 OleDbParameter）。它映射到 Byte 类型的 Array。 
	                LongVarChar	长的字符串值（只限 OleDbParameter）。它映射到 String。 
	                LongVarWChar	长的空终止 Unicode 字符串值（只限 OleDbParameter）。它映射到 String。 
	                Numeric	具有定点精度和小数位数的精确数值 (DBTYPE_NUMERIC)。它映射到 Decimal。 
	                PropVariant	自动化 PROPVARIANT (DBTYPE_PROP_VARIANT)。它映射到 Object。 
	                Single	浮点数字，范围在 -3.40E +38 到 3.40E +38 之间 (DBTYPE_R4)。它映射到 Single。 
	                SmallInt	16 位带符号的整数 (DBTYPE_I2)。它映射到 Int16。 
	                TinyInt	8 位带符号的整数 (DBTYPE_I1)。它映射到 SByte。 
	                UnsignedBigInt	64 位无符号整数 (DBTYPE_UI8)。它映射到 UInt64。 
	                UnsignedInt	32 位无符号整数 (DBTYPE_UI4)。它映射到 UInt32。 
	                UnsignedSmallInt	16 位无符号整数 (DBTYPE_UI2)。它映射到 UInt16。 
	                UnsignedTinyInt	8 位无符号整数 (DBTYPE_UI1)。它映射到 Byte。 
	                VarBinary	二进制数据的变长流（只限 OleDbParameter）。它映射到 Byte 类型的 Array。 
	                VarChar	非 Unicode 字符的变长流（只限 OleDbParameter）。它映射到 String。 
	                Variant	可包含数字、字符串、二进制或日期数据以及特殊值 Empty 和 Null 的特殊数据类型 (DBTYPE_VARIANT)。如果未指定任何其他类型，则假定为该类型。它映射到 Object。 
	                VarNumeric	变长数值（只限 OleDbParameter）。它映射到 Decimal。 
	                VarWChar	Unicode 字符的变长、空终止流（只限 OleDbParameter）。它映射到 String。 
	                WChar	Unicode 字符的空终止流 (DBTYPE_WSTR)。它映射到 String。  
                 */
                switch ((OleDbType)staticOleDbType.GetField(type,
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Public).GetValue(null)) {
                    case OleDbType.BigInt:
                        return DbType.Int64;
                    case OleDbType.Binary:
                        return DbType.Binary;
                    case OleDbType.Boolean:
                        return DbType.Boolean;
                    case OleDbType.BSTR:
                        return DbType.String;
                    case OleDbType.Char:
                        return DbType.String;
                    case OleDbType.Currency:
                        return DbType.Currency;
                    case OleDbType.Date:
                    case OleDbType.DBDate:
                    case OleDbType.DBTimeStamp:
                        return DbType.Date;
                    case OleDbType.DBTime:
                        return DbType.Time;
                    case OleDbType.Decimal:
                        return DbType.Decimal;
                    case OleDbType.Double:
                        return DbType.Double;
                    case OleDbType.Empty:
                        return DbType.Object;
                    case OleDbType.Error:
                        return DbType.Object;
                    case OleDbType.Filetime:
                        return DbType.DateTime;
                    case OleDbType.Guid:
                        return DbType.Guid;
                    case OleDbType.IDispatch:
                        return DbType.Object;
                    case OleDbType.Integer:
                        return DbType.Int32;
                    case OleDbType.IUnknown:
                        return DbType.Object;
                    case OleDbType.LongVarBinary:
                        return DbType.Binary;
                    case OleDbType.LongVarChar:
                        return DbType.String;
                    case OleDbType.LongVarWChar:
                        return DbType.String;
                    case OleDbType.Numeric:
                        return DbType.Decimal;
                    case OleDbType.PropVariant:
                        return DbType.Object;
                    case OleDbType.Single:
                        return DbType.Single;
                    case OleDbType.SmallInt:
                        return DbType.Int16;
                    case OleDbType.TinyInt:
                        return DbType.SByte;
                    case OleDbType.UnsignedBigInt:
                        return DbType.UInt64;
                    case OleDbType.UnsignedInt:
                        return DbType.UInt32;
                    case OleDbType.UnsignedSmallInt:
                        return DbType.UInt16;
                    case OleDbType.UnsignedTinyInt:
                        return DbType.Byte;
                    case OleDbType.VarBinary:
                        return DbType.Binary;
                    case OleDbType.VarChar:
                        return DbType.String;
                    case OleDbType.Variant:
                        return DbType.Object;
                    case OleDbType.VarNumeric:
                        return DbType.Decimal;
                    case OleDbType.VarWChar:
                        return DbType.String;
                    case OleDbType.WChar:
                        return DbType.String;
                    default:
                        throw new InvalidOperationException(string.Format("DbType与OleDbType中没有找到这个类型!{0}", type));
                }
            }
        }

        #endregion


        public string ParamSign {
            get { return ""; }
        }
    }
}
