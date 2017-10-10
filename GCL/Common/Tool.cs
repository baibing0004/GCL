using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GCL.Common {


    /// <summary>
    /// ���ڴ���һЩ�����Ĳ����������ж϶����Ƿ���Ч�ȵ�
    /// </summary>
    public class Tool {
        /// <summary>
        /// ���ַ�����һ���ַ���ת������һ���ַ���
        /// </summary>
        /// <param name="data">Ҫת�����ַ���</param>
        /// <param name="source">Դ���ַ���</param>
        /// <param name="aim">Ŀ�����ַ���</param>
        /// <returns>����Դ���Ŀ���뽫������ַ�������ת��������ֵ�����ϵͳ��֧��Ŀ����ͷ���ԭֵ</returns>
        public static string TurnCharSet(string data, Encoding source, Encoding aim) {
            if (source.Equals(aim))
                return data;
            else
                return aim.GetString(source.GetBytes(data));
        }

        /// <summary>
        /// ���ַ�����һ���ַ���ת������һ���ַ���
        /// </summary>
        /// <param name="data">Ҫת�����ַ���</param>
        /// <param name="source">Դ���ַ���</param>
        /// <param name="aim">Ŀ�����ַ���</param>
        /// <returns>����Դ���Ŀ���뽫������ַ�������ת��������ֵ�����ϵͳ��֧��Ŀ����ͷ���ԭֵ</returns>
        public static string TurnCharSetSafe(string data, string source, string aim) {
            try {
                return TurnCharSet(data, source, aim);
            } catch {
                return data;
            }
        }

        /// <summary>
        /// ���ַ�����һ���ַ���ת������һ���ַ���
        /// </summary>
        /// <param name="data">Ҫת�����ַ���</param>
        /// <param name="source">Դ���ַ���</param>
        /// <param name="aim">Ŀ�����ַ���</param>
        /// <returns>����Դ���Ŀ���뽫������ַ�������ת��������ֵ�����ϵͳ��֧��Ŀ����ͷ���ԭֵ</returns>
        public static string TurnCharSet(string data, string source, string aim) {
            if (source.Trim().Equals(aim.Trim()))
                return data;
            else
                return TurnCharSet(data, Encoding.GetEncoding(source), Encoding.GetEncoding(aim));
        }

        /// <summary>
        /// �ر����������������ķ��ŵ�ת�壬�Դ����Ĳ�ת���Ա�֤���ݵļ�̺���Ч��  todo δ���ת���ַ�
        /// </summary>
        static readonly System.Text.RegularExpressions.Regex regEncode = new System.Text.RegularExpressions.Regex("[~|!|\\@|#|\\$|%|\\^|&|;|\\*|\\(|\\)|_|\\+|\\{|\\}|\\||:|\"|\\?|`|\\-|=|\\[|\\]|\\\\|;|\'|,|\\.|/|��|��]");
        static string replaceMatch(System.Text.RegularExpressions.Match match) {
            return System.Web.HttpUtility.UrlEncode(match.Value);
        }

        /// <summary>
        /// ����webʹ�õ�����ת�屣��
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string WebEncode(string input) { return regEncode.Replace(input, new System.Text.RegularExpressions.MatchEvaluator(Tool.replaceMatch)); }

        /// <summary>
        /// ����webʹ�õ����ݽ���ת����ʾ!
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string WebDecode(string input) { return System.Web.HttpUtility.UrlDecode(input); }
        /// <summary>
        /// �ж϶����Ƿ���Ч
        /// </summary>
        /// <param name="data">��������</param>
        /// <returns>���ΪNull����false�����򷵻�true</returns>
        public static bool IsEnable(string data) {
            if (data != null && !data.Trim().Equals(""))
                return true;
            else
                return false;
        }

        /// <summary>
        /// �ж϶����Ƿ���Ч
        /// </summary>
        /// <param name="data">��������</param>
        /// <returns>���ΪNull����false�����򷵻�true</returns>
        public static bool IsEnable(object data) {
            if (data != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// ��ȡֵ��stringֵ ���������ʹ��Ĭ��ֵ""
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static string GetValue(string data) {
            return GetValue(data, "");
        }

        /// <summary>
        /// ��ȡֵ��stringֵ ���������ʹ��Ĭ��ֵ
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static string GetValue(string data, string defaultValue) {
            if (IsEnable(data))
                return data;
            else
                return defaultValue;
        }

        /// <summary>
        /// ��ȡֵ��objectֵ ���������ʹ��Ĭ��ֵ����
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <returns>ת����Ķ���ֵ</returns>
        public static object GetValue(object data) {
            return GetValue(data, "");

        }

        /// <summary>
        /// ��ȡֵ��objectֵ ���������ʹ��Ĭ��ֵ
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>ת����Ķ���ֵ</returns>
        public static object GetValue(object data, object defaultValue) {
            if (IsEnable(data))
                return data;
            else
                return defaultValue;

        }

        /// <summary>
        /// ��ȡֵ��stringֵ ���������ʹ��Ĭ��ֵ
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static string ToStringValue(object data) {
            return Convert.ToString(GetValue(data));
        }

        /// <summary>
        /// ��ȡֵ��stringֵ ���������ʹ��Ĭ��ֵ
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static string ToStringValue(object data, object defaultValue) {
            return Convert.ToString(GetValue(data, defaultValue));
        }

        /// <summary>
        /// ��ȡֵ��intֵ ���������ʹ��Ĭ��ֵ0
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static int ToIntValue(object data) {
            return ToIntValue(data, 0);
        }

        /// <summary>
        /// ��ȡֵ��intֵ ���������ʹ��Ĭ��ֵ
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static int ToIntValue(object data, int defaultValue) {
            try {
                return Convert.ToInt32(ToStringValue(data));
            } catch {
                return defaultValue;
            }
        }

        /// <summary>
        /// ��ȡֵ��longֵ ���������ʹ��Ĭ��ֵ0
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static long ToLongValue(object data) {
            return ToLongValue(data, 0);
        }


        /// <summary>
        /// ��ȡֵ��longֵ ���������ʹ��Ĭ��ֵ
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static long ToLongValue(object data, long defaultValue) {
            try {
                return Convert.ToInt64(ToStringValue(data));
            } catch {
                return defaultValue;
            }
        }

        /// <summary>
        /// ��ȡֵ��floatֵ ���������ʹ��Ĭ��ֵ0
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static float ToFloatValue(object data) {
            return ToFloatValue(data, 0);
        }

        /// <summary>
        /// ��ȡֵ��floatֵ ���������ʹ��Ĭ��ֵ
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static float ToFloatValue(object data, float defaultValue) {

            try {
                return Convert.ToSingle(ToStringValue(data));
            } catch {
                return defaultValue;
            }
        }

        /// <summary>
        /// ��ȡֵ��doubleֵ ���������ʹ��Ĭ��ֵ0
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static double ToDoubleValue(object data) {
            return ToDoubleValue(data, 0);
        }

        /// <summary>
        /// ��ȡֵ��doubleֵ ���������ʹ��Ĭ��ֵ
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static double ToDoubleValue(object data, double defaultValue) {

            try {
                return Convert.ToDouble(ToStringValue(data));
            } catch {
                return defaultValue;
            }
        }

        /// <summary>
        /// ��ȡֵ��boolֵ ���������ʹ��Ĭ��ֵfalse
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static bool ToBoolValue(object data) {
            return ToBoolValue(data, false);
        }

        /// <summary>
        /// ��ȡֵ��boolֵ ���������ʹ��Ĭ��ֵ
        /// </summary>
        /// <param name="data">ת��ֵ</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>ת���������ֵ</returns>
        public static bool ToBoolValue(object data, bool defaultValue) {

            try {
                return Convert.ToBoolean(ToStringValue(data));
            } catch {
                return defaultValue;
            }
        }

        /// <summary>
        /// ִ�ж�ʱ�����߲���
        /// </summary>
        /// <param name="time"></param>
        public static void ObjectSleep(int time) {
            try {
                Thread.Sleep(time);
            } catch {
            }
        }



        /// <summary>
        /// ִ�ж�ʱ�ĵȴ�����
        /// </summary>       
        /// <param name="obj">�ȴ��Ķ���</param>
        /// <param name="time">�ȴ��ĺ���ֵ</param>
        public static void ObjectWait(object obj, int time) {
            try {
                lock (obj) {
                    Monitor.Wait(obj, time);
                }
            } catch {
            }
        }

        /// <summary>
        /// ִ�ж�ʱ�ĵȴ�����
        /// </summary>
        /// <param name="obj">�ȴ��Ķ���</param>
        public static void ObjectWait(object obj) {
            try {
                lock (obj) {
                    Monitor.Wait(obj);
                }
            } catch {
            }
        }

        /// <summary>
        /// ִ�ж�ʱ�Ļ��Ѳ���
        /// </summary>
        /// <param name="obj">�ȴ��Ķ���</param>
        public static void ObjectPulse(object obj) {
            try {
                lock (obj) {
                    Monitor.Pulse(obj);
                }
            } catch {
            }
        }

        /// <summary>
        /// ִ�ж�ʱ�Ļ��Ѳ���
        /// </summary>
        /// <param name="obj">�ȴ��Ķ���</param>
        public static void ObjectPulseAll(object obj) {
            try {
                lock (obj) {
                    Monitor.PulseAll(obj);
                }
            } catch {
            }
        }


        /// <summary>
        /// ��¼��־��Ϣ��һ���ļ� ��¼ʱ��
        /// </summary>
        /// <param name="file">�ļ���</param>
        /// <param name="data">�ı�����</param>
        public static void RecordFile(string file, string data) {
            RecordFile(file, data, true);
        }

        /// <summary>
        /// ��¼��־��Ϣ��һ���ļ� ��¼ʱ��
        /// </summary>
        /// <param name="file">�ļ���</param>
        /// <param name="data">�ı�����</param>
        /// <param name="charSet">�ַ���</param>
        /// <param name="recordTime">�Ƿ��¼ʱ��</param>
        public static void RecordFile(string file, string data, bool recordTime) {
            RecordFile(file, data, DefaultEncoding, recordTime);
        }

        /// <summary>
        /// ��¼��־��Ϣ��һ���ļ� ��¼ʱ��
        /// </summary>
        /// <param name="file">�ļ���</param>
        /// <param name="data">�ı�����</param>
        /// <param name="charSet">�ַ���</param>
        public static void RecordFile(string file, string data, string charSet) {
            try {
                RecordFile(file, data, Encoding.GetEncoding(charSet), true);
            } catch {
                RecordFile(file, data, true);
            }
        }

        /// <summary>
        /// ��¼��־��Ϣ��һ���ļ� ��¼ʱ��
        /// </summary>
        /// <param name="file">�ļ���</param>
        /// <param name="data">�ı�����</param>
        /// <param name="charSet">�ַ���</param>
        public static void RecordFile(string file, string data, Encoding charSet) {
            RecordFile(file, data, charSet, true);
        }

        /// <summary>
        /// ��¼��־��Ϣ��һ���ļ�
        /// </summary>
        /// <param name="file">�ļ���</param>
        /// <param name="data">�ı�����</param>
        /// <param name="charSet">�ַ���</param>
        /// <param name="recordTime">�Ƿ��¼ʱ��</param>
        public static void RecordFile(string file, string data, string charSet,
                bool recordTime) {
            try {
                RecordFile(file, data, Encoding.GetEncoding(charSet), recordTime);
            } catch {
                RecordFile(file, data, true);
            }
        }

        /// <summary>
        /// ��¼��־��Ϣ��һ���ļ�
        /// </summary>
        /// <param name="file">�ļ���</param>
        /// <param name="data">�ı�����</param>
        /// <param name="charSet">�ַ���</param>
        /// <param name="recordTime">�Ƿ��¼ʱ��</param>
        public static void RecordFile(string file, string data, Encoding charSet,
                bool recordTime) {
            TextWriter pw = null;

            if (IsEnable(charSet))
                pw = new StreamWriter(file, true, charSet);
            else
                pw = new StreamWriter(file, true);

            try {
                RecordFile(pw, data, recordTime);
            } finally {
                try {
                    pw.Close();
                } catch {
                }
            }
        }

        /*
         * ��¼��־��Ϣ��һ���ļ�
         * 
         * @param file
         *            �ļ���
         * @param data
         *            �ı�����
         * @param recordTime
         *            �Ƿ��¼ʱ��
         * @
         */

        public static void RecordFile(System.IO.TextWriter writer, string data, bool recordTime) {
            lock (writer) {
                if (recordTime)
                    writer.WriteLine(FormatNow() + " " + data);
                else
                    writer.WriteLine(data);
            }
        }


        /// <summary>
        /// ��ȡ������Ĭ���ַ���
        /// </summary>
        public static readonly System.Text.Encoding DefaultEncoding = System.Text.Encoding.Default;

        /// <summary>
        /// ��ϵͳ�Ĳ�����ֹ����
        /// </summary>
        public static readonly string LineSeparator = "\r\n";

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss:S ��Ӧ.netΪyyyy-MM-dd HH:mm:ss:f
        /// �򵥸�ʽ���ַ���
        /// </summary>
        public static readonly string yyyy_MM_dd_HH_mm_ss_S = "yyyy-MM-dd HH:mm:ss:f";

        /// <summary>
        /// ʹ�ø�ʽ���ַ�����ʽ��ʱ��
        /// </summary>
        /// <param name="sdf">��ʽ���ַ���</param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string FormatDate(string sdf, DateTime data) {
            //return string.Format("{0:"+sdf+"}", data);
            //���ֶ�����ʵ�ֹ���
            return data.ToString(sdf);
        }


        /// <summary>
        /// ʹ��Ĭ�ϸ�ʽ���ַ�����ʽ��ʱ��
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string FormatDate(DateTime data) {
            return FormatDate(yyyy_MM_dd_HH_mm_ss_S, data);
        }

        /// <summary>
        /// ʹ��Ĭ�ϸ�ʽ���ַ�����ʽ��ʱ�� �����ȶ��ַ�������ת����DateTime Ȼ����б�׼��ʽ����ʾ
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string FormatDate(string data) {
            //�����Զ�ת��Ϊ 00:00:00 ��ʽ���� 1999/10/5 ���� 1999-10-5������
            return FormatDate(DateTime.Parse(data));
        }


        /// <summary>
        /// ��ʽ�����е�ʱ��
        /// </summary>
        /// <param name="sdfFormat">��ʽ�ַ���</param>
        /// <returns></returns>
        public static string FormatNow(string sdfFormat) {
            return FormatDate(sdfFormat, DateTime.Now);
        }


        /// <summary>
        /// ��ʽ�����е�ʱ��
        /// </summary>
        /// <returns></returns>
        public static string FormatNow() {
            return FormatDate(DateTime.Now);
        }

        /// <summary>
        /// �ر��߳�
        /// </summary>
        /// <param name="thread"></param>
        public static void StopThread(Thread thread) {
            try {
                thread.Interrupt();
            } catch {
            }
            try {
                thread.Abort();
            } catch {
            }
        }

        /// <summary>
        /// ���㹫ʽ
        /// ����X86��64λ��ϵ��HashCode�㷨�����һ�� �������
        /// GetHashCode("http://cwc.a.com/CWCCenterControl/modulev2/GetGPlayVer.sjson?t=130039194499455000f10dbc8d9f9245d7ac43c212ec894ee299C637")
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetHashCode(string str) {
            int h = 0;
            for (int i = 0; i < str.Length; i++) {
                h = (int)(31 * h + str[i]);
            }
            return h;
        }

        /// <summary>
        /// �����ҵ��� ���ǰ�����dll ����Ϊ unsafe
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /*
        public static unsafe int CompatibleHash(string str) {
            fixed (char* charPtr = new String(str.ToCharArray())) {
                int hashCode = (5381 << 16) + 5381;
                int numeric = hashCode;
                int* intPtr = (int*)charPtr;
                for (int i = str.Length; i > 0; i -= 4) {
                    hashCode = ((hashCode << 5) + hashCode + (hashCode >> 27)) ^ intPtr[0];

                    if (i <= 2) break;
                    numeric = ((numeric << 5) + numeric + (numeric >> 27)) ^ intPtr[1];
                    intPtr += 2;
                }
                return hashCode + numeric * 1566083941;
            }
        }*/

        /// <summary>
        /// ���ڽ��� CRC ���Hash�㷨
        /// ����X86��64λ��ϵ��HashCode�㷨�����һ��
        /// </summary>
        private static uint[] CRCTAB = {
                                           0x00000000, 0x77073096, 0xee0e612c, 0x990951ba, 0x076dc419, 0x706af48f, 0xe963a535, 0x9e6495a3, 0x0edb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988, 0x09b64c2b, 0x7eb17cbd,
                                           0xe7b82d07, 0x90bf1d91, 0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7, 0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec,
                                           0x14015c4f, 0x63066cd9, 0xfa0f3d63, 0x8d080df5, 0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b, 0x35b5a8fa, 0x42b2986c,
                                           0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59, 0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599, 0xb8bda50f,
                                           0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924, 0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d, 0x76dc4190, 0x01db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x06b6b51f,
                                           0x9fbfe4a5, 0xe8b8d433, 0x7807c9a2, 0x0f00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x086d3d2d, 0x91646c97, 0xe6635c01, 0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e,
                                           0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457, 0x65b0d9c6, 0x12b7e950, 0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65, 0x4db26158, 0x3ab551ce,
                                           0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb, 0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9, 
                                           0x5005713c, 0x270241aa, 0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f, 0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81, 
                                           0xb7bd5c3b, 0xc0ba6cad, 0xedb88320, 0x9abfb3b6, 0x03b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x04db2615, 0x73dc1683, 0xe3630b12, 0x94643b84, 0x0d6d6a3e, 0x7a6a5aa8,
                                           0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1, 0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb, 0x196c3671, 0x6e6b06e7, 0xfed41b76, 0x89d32be0,
                                           0x10da7a5a, 0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5, 0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,
                                           0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef, 0x4669be79, 0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236, 0xcc0c7795, 0xbb0b4703,
                                           0x220216b9, 0x5505262f, 0xc5ba3bbe, 0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d, 0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x026d930a, 
                                           0x9c0906a9, 0xeb0e363f, 0x72076785, 0x05005713, 0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38, 0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0x0bdbdf21, 0x86d3d2d4, 0xf1d4e242, 
                                           0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777, 0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45,
                                           0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db, 0xaed16a4a, 0xd9d65adc, 0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 
                                           0x47b2cf7f, 0x30b5ffe9, 0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693, 0x54de5729, 0x23d967bf, 0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94, 
                                           0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d
                                       };

        /// <summary>
        /// ͨ��CRC��������2�ߵ�HashCodeֵ �ٶ�Ӧ�����
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetCRCHashCode(string key) {
            long ret = key.Length;
            foreach (char _s in key) {
                ret = (int)(ret >> 8) ^ CRCTAB[(ret & 0xff) ^ _s % 256];
            }
            return (int)ret;
        }

        private static System.Security.Cryptography.MD5CryptoServiceProvider md5sp;
        private static object md5key = DateTime.Now;

        /// <summary>
        /// ͨ��΢��MD5�㷨����ɢ�м��� �㷨��ٷ�
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetMD5HashCode(string key) {
            if (md5sp == null) lock (md5key) {
                    if (md5sp == null) md5sp = new System.Security.Cryptography.MD5CryptoServiceProvider();
                }
            return BitConverter.ToInt32(md5sp.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key)), 0);
        }

        /// <summary>
        /// ����ʹ��WebClient����ʵ��Post���� ֱ�ӱ���������Form����
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string PostString(string url, string data) {
            System.Net.WebClient cl = new Net.WebClient();
            cl.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            cl.Encoding = System.Text.Encoding.UTF8;
            cl.Headers["accept-encoding"] = "text/plain";
            cl.Headers["vesh-encoding"] = "text/plain";
            cl.Headers["accept-encoding-vesh"] = "text/plain";
            return cl.UploadString(url, data);
        }

        /// <summary>
        /// ����ʹ��WebClient����ʵ��Get���� ֱ�ӱ���������QueryString����
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DownloadString(string url, string data) {
            System.Net.WebClient cl = new System.Net.WebClient();
            cl.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            cl.Encoding = System.Text.Encoding.UTF8;
            return cl.DownloadString(string.Format("{0}?{1}", url.Trim('?'), data));
        }

        /// <summary>
        /// ��ǰʱ��ת��������־ʱ��
        /// </summary>
        /// <param name="lacalTime"></param>
        /// <returns></returns>
        public static DateTime LocalTime2GreenwishTime(DateTime localTime) {
            TimeZone localTimeZone = System.TimeZone.CurrentTimeZone;
            TimeSpan timeSpan = localTimeZone.GetUtcOffset(localTime);
            DateTime greenwishTime = localTime - timeSpan;
            return greenwishTime;
        }

        /// <summary>
        /// ��������ʱ��ת����ǰʱ��
        /// </summary>
        /// <param name="greenwishTime"></param>
        /// <returns></returns>
        public static DateTime GreenwishTime2LocalTime(DateTime greenwishTime) {
            TimeZone localTimeZone = System.TimeZone.CurrentTimeZone;
            TimeSpan timeSpan = localTimeZone.GetUtcOffset(greenwishTime);
            DateTime lacalTime = greenwishTime + timeSpan;
            return lacalTime;
        }
        public static readonly string GreenwishTimeStringFormat = @"ddd MMM dd yyyy HH:mm:ss GMT+ffff";
        /// <summary>
        /// ����ʱ��ĸ�������ʱ�䴮
        /// </summary>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public static string LocalTime2GreenwishTimeString(DateTime localTime) {
            return LocalTime2GreenwishTime(localTime).ToString(GreenwishTimeStringFormat);
        }

        /// <summary>
        /// ��������ʱ�䴮ת������ʱ��
        /// </summary>
        /// <param name="greenwishTime"></param>
        /// <returns></returns>
        public static DateTime GreenwishTimeString2LocalTime(string time) {
            try {
                return GreenwishTime2LocalTime(DateTime.ParseExact(time, GreenwishTimeStringFormat, null));
            } catch (FormatException) {
                return DateTime.Now.Date;
            }
        }

        public static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();
        /// <summary>
        /// ����Json ���ڶ��̴߳�����ܻ�������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ParseJson<T>(string json) {
            return (T)Serializer.Deserialize(json, typeof(T));
        }

        /// <summary>
        /// ����Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="se"></param>
        /// <returns></returns>
        public static T ParseJson<T>(string json, JavaScriptSerializer se) {
            return (T)se.Deserialize(json, typeof(T));
        }

        /// <summary>
        /// �������л�
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeToString(object obj) {
            return JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// �����л�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T DeserializeToObject<T>(string str) {
            return JsonConvert.DeserializeObject<T>(str);
        }


        /// <summary>
        /// The json serializer
        /// </summary>
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();
        /// <summary>
        /// ��һ���������л�JSON�ַ���
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="obj">�����л��Ķ���</param>
        /// <returns>JSON�ַ���</returns>
        public static string Serialize(object obj) {
            var sw = new StringWriter();
            JsonSerializer.Serialize(new JsonTextWriter(sw), obj);
            return sw.GetStringBuilder().ToString();
        }
        /// <summary>
        /// ��JSON�ַ��������л�Ϊһ��Object����
        /// </summary>
        /// <remarks>         
        /// </remarks>
        /// <param name="json">JSON�ַ���</param>
        /// <returns>Object����</returns>
        public static object Deserialize(string json) {
            var sr = new StringReader(json);
            return JsonSerializer.Deserialize(new JsonTextReader(sr));
        }
        /// <summary>
        /// ��JSON�ַ��������л�Ϊһ��ָ�����Ͷ���
        /// </summary>
        /// <remarks>         
        /// </remarks>
        /// <typeparam name="TObj">��������</typeparam>
        /// <param name="json">JSON�ַ���</param>
        /// <returns>ָ�����Ͷ���</returns>
        public static TObj Deserialize<TObj>(string json) where TObj : class {
            var sr = new StringReader(json);
            return JsonSerializer.Deserialize(new JsonTextReader(sr), typeof(TObj)) as TObj;
        }
        /// <summary>
        /// ��JSON�ַ��������л�Ϊһ��JObject����
        /// </summary>
        /// <remarks>        
        /// </remarks>
        /// <param name="json">JSON�ַ���</param>
        /// <returns>JObject����</returns>
        public static JObject DeserializeObject(string json) {
            return JsonConvert.DeserializeObject(json) as JObject;
        }
        /// <summary>
        /// ��JSON�ַ��������л�Ϊһ��JArray����
        /// </summary>
        /// <remarks>         
        /// </remarks>
        /// <param name="json">JSON�ַ���</param>
        /// <returns>JArray����</returns>
        public static JArray DeserializeArray(string json) {
            return JsonConvert.DeserializeObject(json) as JArray;
        }

        #region win32

        #region Constants
        public static readonly int WM_DESTROY = 2;
        public static readonly int WS_CHILD = 0x40000000;
        public static readonly int WS_VISIBLE = 0x10000000;

        public static readonly int WS_EX_LAYERED = 0x00080000;
        public static readonly int WS_EX_TRANSPARENT = 0x20;
        public static readonly int WS_EX_NOACTIVATE = 0x08000000;

        public static readonly int GWL_STYLE = -16;
        public static readonly int GWL_EXSTYLE = -20;

        public static readonly uint SWP_NOSIZE = 0x0001;
        public static readonly uint SWP_NOMOVE = 0x0002;
        public static readonly uint SWP_NOACTIVATE = 0x0010;

        public static readonly uint GW_HWNDNEXT = 2;

        public const uint SPI_GETSCREENSAVERRUNNING = 0x0072;

        static public readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        static public readonly IntPtr HWND_TOP = new IntPtr(0);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        #endregion

        #region Delegates

        /// <summary>
        /// The enum windows proc.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public delegate bool EnumWindowsProc(IntPtr hWnd, ref IntPtr data);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The enum windows.
        /// </summary>
        /// <param name="lpEnumFunc">
        /// The lp enum func.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32")]
        public static extern int EnumWindows(EnumWindowsProc lpEnumFunc, ref IntPtr data);

        /// <summary>
        /// The find window.
        /// </summary>
        /// <param name="lpClassName">
        /// The lp class name.
        /// </param>
        /// <param name="lpWindowName">
        /// The lp window name.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("user32")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// The find window ex.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("user32")]
        public static extern IntPtr FindWindowEx(IntPtr a, IntPtr b, string c, string d);

        /// <summary>
        /// The set parent.
        /// </summary>
        /// <param name="hWndChild">
        /// The h wnd child.
        /// </param>
        /// <param name="hWndNewParent">
        /// The h wnd new parent.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("user32")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// The set window pos.
        /// </summary>
        /// <param name="hWnd">
        /// The h wnd.
        /// </param>
        /// <param name="hWndInsertAfter">
        /// The h wnd insert after.
        /// </param>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="Y">
        /// The y.
        /// </param>
        /// <param name="cx">
        /// The cx.
        /// </param>
        /// <param name="cy">
        /// The cy.
        /// </param>
        /// <param name="wFlags">
        /// The w flags.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(
            IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        /// <summary>
        /// The show window.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="nCmdShow">
        /// The n cmd show.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        /// <summary>
        /// The show window desktop.
        /// </summary>
        /// <param name="win_IntPtr">
        /// The win_ int ptr.
        /// </param>
        public static void ShowWindowDesktop(IntPtr win_IntPtr) {
            IntPtr desktop = findDesktopIconWnd();
            SetParent(win_IntPtr, desktop);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get window long.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="nIndex">
        /// The n index.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

        /// <summary>
        /// The set window long.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="nIndex">
        /// The n index.
        /// </param>
        /// <param name="dwNewLong">
        /// The dw new long.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        /// <summary>
        /// The enum user windows cb.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="lParam">
        /// The l param.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool enumUserWindowsCB(IntPtr hwnd, ref IntPtr lParam) {
            long wflags = GetWindowLong(hwnd, GWL_STYLE);
            if ((wflags & WS_VISIBLE) != WS_VISIBLE) {
                return true;
            }

            IntPtr sndWnd = FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null);
            if ((int)sndWnd == 0) {
                return true;
            }

            IntPtr targetWnd = FindWindowEx(sndWnd, IntPtr.Zero, "SysListView32", "FolderView");
            if ((int)targetWnd == 0) {
                return true;
            }

            lParam = targetWnd;

            return false;
        }

        /// <summary>
        /// The find desktop icon wnd.
        /// </summary>
        /// <returns>
        /// The <see cref="IntPtr"/>.
        /// </returns>
        private static IntPtr findDesktopIconWnd() {
            IntPtr resultHwnd = IntPtr.Zero;
            EnumWindows(enumUserWindowsCB, ref resultHwnd);
            return resultHwnd;
        }

        #endregion

        /// <summary>
        /// ����Ӧ�����
        /// </summary>
        /// <param name="pro"></param>
        public static void SetAppMax(Process pro) {
            ShowWindow(pro.MainWindowHandle, 3);
        }

        /// <summary>
        /// ����Ӧ����С��
        /// </summary>
        /// <param name="pro"></param>
        public static void SetAppMin(Process pro) {
            ShowWindow(pro.MainWindowHandle, 6);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="pro"></param>
        public static void SetAppTop(Process pro) {
            //0 top -1 topmost -2 notopmost 1 bottom
            SetWindowPos(pro.MainWindowHandle, 0, 0, 0, 0, 0, 0x0040);
        }

        /// <summary>
        /// ����Ƕ������
        /// </summary>
        /// <param name="pro"></param>
        public static void SetAppInDesktop(IntPtr handle) {
            IntPtr ptr = Tool.FindWindow("Progman", null);
            Tool.SetParent(handle, ptr);
        }

        /// <summary>
        /// ����TopMost
        /// </summary>
        /// <param name="pro"></param>
        public static void SetAppTopMost(Process pro) {
            SetWindowPos(pro.MainWindowHandle, (int)HWND_TOPMOST, 0, 0, 0, 0, 0x0040);
        }

        /// <summary>
        /// ����Ƕ������
        /// </summary>
        /// <param name="pro"></param>
        public static void SetAppBottom(IntPtr handle) {
            IntPtr ptr = Tool.FindWindow("Progman", null);
            Tool.SetParent(handle, ptr);
        }

        /// <summary>
        /// ��ȡ���汳��ͼƬ��λ��
        /// </summary>
        /// <returns></returns>
        public static string GetDesktopImg() {
            return Convert.ToString(Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop").GetValue("Wallpaper").ToString());
        }

        #endregion


    }
}