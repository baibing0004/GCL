using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace GCL.Db.Ni {
    /// <summary>
    /// 使用SqlLite数据库进行通讯和连接 
    /// 注意 只能在x86或者x64位下使用 不能在AnyCPU模式下使用
    /// select * from [FurnitureCategory] where [categoryID]= $categoryID
    /// 
    /// Basic（基本的）
    ///     Data Source=filename;Version=3;
    ///Using UTF16（使用UTF16编码）
    ///    Data Source=filename;Version=3;UseUTF16Encoding=True;
    ///With password（带密码的）
    ///    Data Source=filename;Version=3;Password=myPassword;
    ///Using the pre 3.3x database format（使用3.3x前数据库格式）
    ///     Data Source=filename;Version=3;Legacy Format=True;
    ///Read only connection（只读连接）
    ///    Data Source=filename;Version=3;Read Only=True;
    ///With connection pooling（设置连接池）
    ///    Data Source=filename;Version=3;Pooling=False;Max Pool Size=100;
    ///Using DateTime.Ticks as datetime format（）
    ///     Data Source=filename;Version=3;DateTimeFormat=Ticks;    
    ///Store GUID as text（把Guid作为文本存储，默认是Binary）
    ///    Data Source=filename;Version=3;BinaryGUID=False;
    ///      如果把Guid作为文本存储需要更多的存储空间
    ///Specify cache size（指定Cache大小）
    ///     Data Source=filename;Version=3;Cache Size=2000;
    ///      Cache Size 单位是字节
    ///Specify page size（指定页大小）
    ///     Data Source=filename;Version=3;Page Size=1024;
    ///      Page Size 单位是字节
    /// </summary>
    public class SQLiteFactory : IDataAbstractFactory {
        public IDbConnection CreateConnection() {
            return System.Data.SQLite.SQLiteFactory.Instance.CreateConnection();
        }

        public IDbCommand CreateCommand() {
            return System.Data.SQLite.SQLiteFactory.Instance.CreateCommand();
        }

        public IDbDataAdapter CreateAdapter() {
            return System.Data.SQLite.SQLiteFactory.Instance.CreateDataAdapter();
        }

        public IDbDataParameter CreateParameter() {
            return System.Data.SQLite.SQLiteFactory.Instance.CreateParameter();
        }

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
                throw new InvalidOperationException(string.Format("DbType中没有找到这个类型!{0}", type));
            }
        }


        public string ParamSign {
            get { return "$"; }
        }
    }
}
