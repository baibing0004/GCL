using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Data.SqlClient;
namespace GCL.Db.Ni {
    public class SqlFactory : IDataAbstractFactory {
        #region IDataAbstractFactory Members

        public IDbConnection CreateConnection() {
            return new SqlConnection();
        }

        #endregion

        #region IDataAbstractBase Members
        public IDbCommand CreateCommand() {
            return new SqlCommand();
        }

        public IDbDataAdapter CreateAdapter() {
            return new SqlDataAdapter();
        }

        public IDbDataParameter CreateParameter() {
            return new SqlParameter();
        }

        private static Type staticDbType = typeof(System.Data.DbType);
        private static Type staticSqlDbType = typeof(System.Data.SqlDbType);
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
                    BigInt	Int64。64 位的有符号整数。 
	                Binary	Byte 类型的 Array。二进制数据的固定长度流，范围在 1 到 8,000 个字节之间。 
	                Bit	Boolean。无符号数值，可以是 0、1 或 空引用（在 Visual Basic 中为 Nothing）。 
	                Char	String。非 Unicode 字符的固定长度流，范围在 1 到 8,000 个字符之间。 
	                DateTime	DateTime。日期和时间数据，值范围从 1753 年 1 月 1 日到 9999 年 12 月 31 日，精度为 3.33 毫秒。 
	                Decimal	Decimal。固定精度和小数位数数值，在 -10 38 -1 和 10 38 -1 之间。 
	                Float	Double。-1.79E +308 到 1.79E +308 范围内的浮点数。 
	                Image	Byte 类型的 Array。二进制数据的可变长度流，范围在 0 到 2 31 -1（即 2,147,483,647）字节之间。 
	                Int	Int32。32 位的有符号整数。 
	                Money	Decimal。货币值，范围在 -2 63（即 -922,337,203,685,477.5808）到 2 63 -1（即 +922,337,203,685,477.5807）之间，精度为千分之十个货币单位。 
	                NChar	String。Unicode 字符的固定长度流，范围在 1 到 4,000 个字符之间。 
	                NText	String。Unicode 数据的可变长度流，最大长度为 2 30 - 1（即 1,073,741,823）个字符。 
	                NVarChar	String。Unicode 字符的可变长度流，范围在 1 到 4,000 个字符之间。如果字符串大于 4,000 个字符，隐式转换会失败。在使用比 4,000 个字符更长的字符串时，请显式设置对象。 
	                Real	Single。-3.40E +38 到 3.40E +38 范围内的浮点数。 
	                SmallDateTime	DateTime。日期和时间数据，值范围从 1900 年 1 月 1 日到 2079 年 6 月 6 日，精度为 1 分钟。 
	                SmallInt	Int16。16 位的有符号整数。 
	                SmallMoney	Decimal。货币值，范围在 -214,748.3648 到 +214,748.3647 之间，精度为千分之十个货币单位。 
	                Text	String。非 Unicode 数据的可变长度流，最大长度为 2 31 -1（即 2,147,483,647）个字符。 
	                Timestamp	Byte 类型的 Array。自动生成的二进制数，并保证其在数据库中唯一。timestamp 通常用作对表中各行的版本进行标记的机制。存储大小为 8 字节。 
	                TinyInt	Byte。8 位的无符号整数。 
	                Udt	SQL Server 2005 用户定义的类型 (UDT)。 
	                UniqueIdentifier	Guid。全局唯一标识符（或 GUID）。 
	                VarBinary	Byte 类型的 Array。二进制数据的可变长度流，范围在 1 到 8,000 个字节之间。如果字节数组大于 8,000 个字节，隐式转换会失败。在使用比 8,000 个字节大的字节数组时，请显式设置对象。 
	                VarChar	String。非 Unicode 字符的可变长度流，范围在 1 到 8,000 个字符之间。 
	                Variant	Object。特殊数据类型，可以包含数值、字符串、二进制或日期数据，以及 SQL Server 值 Empty 和 Null，后两个值在未声明其他类型的情况下采用。 
	                Xml	XML 值。使用 GetValue 方法或 Value 属性获取字符串形式的 XML，或通过调用 CreateReader 方法获取 XmlReader 形式的 XML。 
                 */
                switch ((SqlDbType)staticSqlDbType.GetField(type,
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Public).GetValue(null)) {
                    case SqlDbType.BigInt:
                        return DbType.Int64;
                    case SqlDbType.Binary:
                        return DbType.Binary;
                    case SqlDbType.Bit:
                        return DbType.Boolean;
                    case SqlDbType.Char:
                        return DbType.String;
                    case SqlDbType.DateTime:
                        return DbType.DateTime;
                    case SqlDbType.Decimal:
                        return DbType.Decimal;
                    case SqlDbType.Float:
                        return DbType.Double;
                    case SqlDbType.Image:
                        return DbType.Binary;
                    case SqlDbType.Money:
                        return DbType.Currency;
                    case SqlDbType.NChar:
                        return DbType.String;
                    case SqlDbType.NText:
                        return DbType.String;
                    case SqlDbType.NVarChar:
                        return DbType.String;
                    case SqlDbType.Real:
                        return DbType.Single;
                    case SqlDbType.SmallDateTime:
                        return DbType.DateTime;
                    case SqlDbType.SmallInt:
                        return DbType.Int16;
                    case SqlDbType.SmallMoney:
                        return DbType.Currency;
                    case SqlDbType.Text:
                        return DbType.String;
                    case SqlDbType.Timestamp:
                        return DbType.Binary;
                    case SqlDbType.TinyInt:
                        return DbType.Byte;
                    case SqlDbType.UniqueIdentifier:
                        return DbType.Guid;
                    case SqlDbType.VarBinary:
                        return DbType.Binary;
                    case SqlDbType.VarChar:
                        return DbType.String;
                    case SqlDbType.Variant:
                        return DbType.Object;
                    case SqlDbType.Xml:
                        return DbType.Xml;
                    default:
                        throw new InvalidOperationException(string.Format("DbType与SqlDbType中没有找到这个类型!{0}", type));
                }
            }
        }

        #endregion


        public string ParamSign {
            get { return "@"; }
        }
    }
}
