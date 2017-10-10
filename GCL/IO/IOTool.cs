using System;
using System.Collections;
using System.Text;
using GCL.Common;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

namespace GCL.IO {

    /// <summary>
    /// ���ڴ���һ���IO����
    /// </summary>
    public class IOTool : Tool {

        /// <summary>
        /// ����������Stream֮����ɽ������� �����ر���
        /// </summary>
        /// <param name="source">������</param>
        /// <param name="result">�����</param>
        /// <param name="bufferedSize">������С</param>
        public static void Transport(Stream source, Stream result,
            int bufferedSize) {
            byte[] data = new byte[bufferedSize];
            int len = 0;
            while ((len = source.Read(data, 0, data.Length)) >= 0)
                result.Write(data, 0, len);
            result.Flush();
        }

        /// <summary>
        /// ����������Stream֮����ɽ������� �����ر��� Ĭ�ϻ����1024�ֽ�
        /// </summary>
        /// <param name="source">������</param>
        /// <param name="result">�����</param>
        public static void Transport(Stream source, Stream result) {
            Transport(source, result, 1024);
        }

        /// <summary>
        /// ������TextReader��TextWriter֮����ɽ������� �����ر�
        /// </summary>
        /// <param name="source">TextReader����</param>
        /// <param name="result">TextWriter����</param>
        public static void Transport(TextReader source, TextWriter result, int bufferedSize) {
            char[] data = new char[bufferedSize];
            int len = 0;
            while ((len = source.Read(data, 0, data.Length)) >= 0)
                result.Write(data, 0, len);
            result.Flush();
        }

        /// <summary>
        /// ������TextReader��TextWriter֮����ɽ������� �����ر� 1024���ַ�
        /// </summary>
        /// <param name="source">TextReader����</param>
        /// <param name="result">TextWriter����</param>
        public static void Transport(TextReader source, TextWriter result) {
            Transport(source, result, 1024);
        }

        /// <summary>
        /// ��IDictionary����֮�以��ת��
        /// </summary>
        /// <param name="source"></param>
        /// <param name="arm"></param>
        public static void Transport(IDictionary source, IDictionary arm) {
            for (IDictionaryEnumerator ienu = source.GetEnumerator(); ienu.MoveNext(); )
                arm.Add(ienu.Key, ienu.Value);
        }

        public static string GetFileName(string filter) {
            filter = filter.Trim();
            string[] name = filter.Split('\\', '/');
            string path = filter.Replace(name[name.Length - 1], "");
            return name[name.Length - 1].Length == 0 ? "*.*" : name[name.Length - 1];
        }

        public static string GetPath(string filter) {
            filter = filter.Trim();
            string[] name = filter.Split('\\', '/');
            string path = filter.Replace(name[name.Length - 1], "");
            if (path.Trim().Length == 0)
                path = ".";
            return path;
        }

        static double ToBytes(int size, int dimension) {
            return Math.Pow(1024, dimension) * size;
        }

        public static double KToBytes(int Ksize) {
            return ToBytes(Ksize, 1);
        }

        public static double MToBytes(int Msize) {
            return ToBytes(Msize, 2);
        }

        static double FromBytes(int size, int dimension) {
            return size / Math.Pow(1024, dimension);
        }

        public static double BytesToK(int size) {
            return FromBytes(size, 1);
        }

        public static double BytesToM(int size) {
            return FromBytes(size, 2);
        }

        public static Encoding GetEncoding(string path) {
            using (FileStream stream = new FileStream(path, FileMode.Open)) {
                return GetEncoding(stream);
            }
        }

        public static Encoding GetEncoding(Stream stream) {
            if (stream != null) {
                //�����ļ�����ǰ4���ֽ�
                byte[] bytes = new byte[3];

                //���浱ǰSeekλ��
                long origPos = stream.Seek(0, SeekOrigin.Begin);
                stream.Read(bytes, Convert.ToInt32(origPos), Convert.ToInt32(stream.Length >= 3 ? 3 : stream.Length));

                //�����ļ�����ǰ4���ֽ��ж�Encoding
                //Unicode {0xFF, 0xFE};
                //BE-Unicode {0xFE, 0xFF};
                //UTF8 = {0xEF, 0xBB, 0xBF};
                if (bytes[0] == 0xFE && bytes[1] == 0xFF) {//UnicodeBe
                    return Encoding.BigEndianUnicode;
                } else if (bytes[0] == 0xFF && bytes[1] == 0xFE && bytes[2] != 0xFF) {//Unicode
                    return Encoding.Unicode;
                } else if (bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF) {//UTF8
                    return Encoding.UTF8;
                }

                //�ָ�Seekλ�á����� 
                stream.Seek(origPos, SeekOrigin.Begin);
            }
            return null;
        }


        private static Encoding GBKEncoding = System.Text.Encoding.GetEncoding("GBK");
        /// <summary>
        /// ʹ�ò���������ת����GBK�ַ���
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetGBKString(byte[] data) {
            return GBKEncoding.GetString(data).Trim('\0');
        }
        /// <summary>
        /// ʹ�ö�������ת����GBK�ַ���
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetGBKString(byte[] data, int start, int length) {
            return GBKEncoding.GetString(data, start, length).Trim('\0');
        }
        /// <summary>
        /// ʹ��GBK�ַ���ת�ɲ�����Byte����
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] GetGBKBytes(string data) {
            return GBKEncoding.GetBytes(data);
        }
        /// <summary>
        /// ʹ��GBK�ַ���ת�ɶ���Byte���飨��λ��0)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] GetGBKBytes(string data, int length) {
            byte[] datas = new byte[length];
            byte[] _datas = GBKEncoding.GetBytes(data);
            if (_datas.Length > datas.Length) throw new IndexOutOfRangeException();
            Array.Copy(_datas, datas, _datas.Length);
            return datas;
        }

        /// <summary>
        /// ��ʱ��ת����BCD��
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static byte[] DateTime2BCD(DateTime date) {
            return String2BCD((date.ToString("yyMMddHHmmss")));
        }

        /// <summary>
        /// BCD���ó�UTCʱ��
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DateTime BCD2DateTime(byte[] data) {
            return DateTime.ParseExact(BCD2String(data), "yyMMddHHmmss", System.Globalization.DateTimeFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// �ֽ�����תΪ16�����ַ���
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Bytes2HexString(byte[] bytes) {
            if (bytes == null || bytes.Length == 0)
                throw new ArgumentException("��������ȷ");

            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes) {
                sb.Append(string.Format("{0:X2}", b));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 16�����ַ���תΪ������ �ַ�����ʽ�磺01A20ED355FD
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        public static byte[] HexString2Bytes(string hexStr, int length) {
            string machStr = "^([0-9A-Fa-f][0-9A-Fa-f])*$";
            Regex regex = new Regex(machStr);
            if (!regex.IsMatch(hexStr))
                throw new ArgumentException("������ʽ����ȷ");

            int count = hexStr.Length / 2;
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++) {
                bytes[i] = byte.Parse(hexStr.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            byte[] ret = new byte[length];
            if (length - bytes.Length < 0) throw new IndexOutOfRangeException("�ַ���ת�����ȳ�����������ݳ���!");
            Array.Copy(bytes, 0, ret, length - bytes.Length, bytes.Length);
            return ret;
        }

        /// <summary>
        /// 8421BCD��ת�ַ���
        /// </summary>
        /// <param name="bcdbytes"></param>
        /// <returns></returns>
        public static string BCD2String(byte[] bcdbytes) {
            StringBuilder sb = new StringBuilder();
            try {
                foreach (byte by in bcdbytes) {
                    sb.Append((byte)(by >> 4)).Append((byte)(by & 0x0f));
                }
                return sb.ToString();
            } finally {
                sb.Clear();
                sb = null;
            }
        }

        /// <summary>
        /// �ַ���ת6λ����BCD
        /// </summary>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static byte[] String2BCD(string asc) {
            return String2BCD(asc, 6);
        }

        /// <summary>
        /// �ַ���ת������BCD��
        /// </summary>
        /// <param name="asc"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] String2BCD(string asc, int length) {
            asc = string.Format("{0:D" + length * 2 + "}", Convert.ToInt64(asc));
            int len = asc.Length / 2;
            byte[] abt = new byte[len];
            for (int w = 0; w < len; w++) {
                int j, k;
                j = (Convert.ToUInt16(asc[w * 2].ToString()) & 0x0f) << 4;
                k = Convert.ToUInt16(asc[w * 2 + 1].ToString()) & 0x0f;
                abt[w] = (byte)(j | k);
            }
            return abt;
        }

        /// <summary>
        /// ѹ�������ļ�
        /// </summary>
        /// <param name="fileToZip">Ҫѹ�����ļ�</param>
        /// <param name="zipedFile">ѹ������ļ�</param>
        /// <param name="compressionLevel">ѹ���ȼ�</param>
        /// <param name="blockSize">ÿ��д���С</param>
        public static void ZipFile(string fileToZip, string zipedFile, int compressionLevel, int blockSize) {
            //����ļ�û���ҵ����򱨴�
            if (!System.IO.File.Exists(fileToZip)) {
                throw new System.IO.FileNotFoundException("ָ��Ҫѹ�����ļ�: " + fileToZip + " ������!");
            }

            using (System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile)) {
                using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile)) {
                    using (System.IO.FileStream StreamToZip = new System.IO.FileStream(fileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
                        string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);

                        ZipEntry ZipEntry = new ZipEntry(fileName);

                        ZipStream.PutNextEntry(ZipEntry);

                        ZipStream.SetLevel(compressionLevel);

                        byte[] buffer = new byte[blockSize];

                        int sizeRead = 0;

                        try {
                            do {
                                sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                                ZipStream.Write(buffer, 0, sizeRead);
                            }
                            while (sizeRead > 0);
                        } catch (System.Exception ex) {
                            throw ex;
                        }

                        StreamToZip.Close();
                    }

                    ZipStream.Finish();
                    ZipStream.Close();
                }

                ZipFile.Close();
            }
        }

        /// <summary>
        /// ѹ�������ļ�
        /// </summary>
        /// <param name="fileToZip">Ҫ����ѹ�����ļ���</param>
        /// <param name="zipedFile">ѹ�������ɵ�ѹ���ļ���</param>
        public static void ZipFile(string fileToZip, string zipedFile) {
            //����ļ�û���ҵ����򱨴�
            if (!File.Exists(fileToZip)) {
                throw new System.IO.FileNotFoundException("ָ��Ҫѹ�����ļ�: " + fileToZip + " ������!");
            }
            using (FileStream ZipFile = File.Create(zipedFile)) {
                using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile)) {
                    string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);
                    ZipEntry ZipEntry = new ZipEntry(fileName);
                    ZipStream.PutNextEntry(ZipEntry);
                    ZipStream.SetLevel(5);
                    using (FileStream fs = File.OpenRead(fileToZip)) {
                        byte[] buffer = new byte[4096];
                        int count = 0;
                        while ((count = fs.Read(buffer, 0, buffer.Length)) > 0) {
                            ZipStream.Write(buffer, 0, count);
                        }
                    }
                    ZipStream.Finish();
                    ZipStream.Close();
                }
            }
        }

        /// <summary>
        /// ѹ�����Ŀ¼
        /// </summary>
        /// <param name="strDirectory">The directory.</param>
        /// <param name="zipedFile">The ziped file.</param>
        public static void ZipFileDirectory(string strDirectory, string zipedFile) {
            using (System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile)) {
                using (ZipOutputStream s = new ZipOutputStream(ZipFile)) {
                    ZipSetp(strDirectory, s, "");
                }
            }
        }

        /// <summary>
        /// �ݹ����Ŀ¼
        /// </summary>
        /// <param name="strDirectory">The directory.</param>
        /// <param name="s">The ZipOutputStream Object.</param>
        /// <param name="parentPath">The parent path.</param>
        private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath) {
            if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar) {
                strDirectory += Path.DirectorySeparatorChar;
            }
            Crc32 crc = new Crc32();

            string[] filenames = Directory.GetFileSystemEntries(strDirectory);

            foreach (string file in filenames)// �������е��ļ���Ŀ¼
	        {

                if (Directory.Exists(file))// �ȵ���Ŀ¼��������������Ŀ¼�͵ݹ�Copy��Ŀ¼������ļ�
	            {
                    string pPath = parentPath;
                    pPath += file.Substring(file.LastIndexOf("\\") + 1);
                    pPath += "\\";
                    ZipSetp(file, s, pPath);
                } else // ����ֱ��ѹ���ļ�
	            {
                    //��ѹ���ļ�
                    using (FileStream fs = File.OpenRead(file)) {
                        string fileName = parentPath + file.Substring(file.LastIndexOf("\\") + 1);
                        ZipEntry entry = new ZipEntry(fileName);
                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;
                        s.PutNextEntry(entry);
                        byte[] buffer = new byte[4096];//��һ�б���ѹ����һ����С�ͱ�����֪�����ڴ�����ˣ����ǲ�֪����ô���
                        int count = 0;
                        crc.Reset();
                        while ((count = fs.Read(buffer, 0, buffer.Length)) > 0) {
                            crc.Update(buffer, 0, count);
                            s.Write(buffer, 0, count);
                        }
                        entry.Crc = crc.Value;
                    }
                }
            }
        }

        /// <summary>
        /// ��ѹ��һ�� zip �ļ���
        /// </summary>
        /// <param name="zipedFile">The ziped file.</param>
        /// <param name="strDirectory">The STR directory.</param>
        /// <param name="password">zip �ļ������롣</param>
        /// <param name="overWrite">�Ƿ񸲸��Ѵ��ڵ��ļ���</param>
        public void UnZip(string zipedFile, string strDirectory, string password, bool overWrite) {

            if (strDirectory == "")
                strDirectory = Directory.GetCurrentDirectory();
            if (!strDirectory.EndsWith("\\"))
                strDirectory = strDirectory + "\\";

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipedFile))) {
                s.Password = password;
                ZipEntry theEntry;

                while ((theEntry = s.GetNextEntry()) != null) {
                    string directoryName = "";
                    string pathToZip = "";
                    pathToZip = theEntry.Name;

                    if (pathToZip != "")
                        directoryName = Path.GetDirectoryName(pathToZip) + "\\";

                    string fileName = Path.GetFileName(pathToZip);

                    Directory.CreateDirectory(strDirectory + directoryName);

                    if (fileName != "") {
                        if ((File.Exists(strDirectory + directoryName + fileName) && overWrite) || (!File.Exists(strDirectory + directoryName + fileName))) {
                            using (FileStream streamWriter = File.Create(strDirectory + directoryName + fileName)) {
                                int size = 8;
                                byte[] data = new byte[8];
                                while (true) {
                                    size = s.Read(data, 0, data.Length);

                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }

                s.Close();
            }
        }

        public static void Transport<K, V>(System.Collections.Generic.IDictionary<K, V> source, System.Collections.Generic.IDictionary<K, V> idic) {
            for (System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<K, V>> ienu = source.GetEnumerator(); ienu.MoveNext(); )
                if (idic.ContainsKey(ienu.Current.Key))
                    idic[ienu.Current.Key] = ienu.Current.Value;
                else
                    idic.Add(ienu.Current.Key, ienu.Current.Value);
        }

        public static void Transport<K, V>(System.Collections.Generic.Dictionary<K, V> source, System.Collections.Generic.IDictionary<K, object> idic) {
            for (System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<K, V>> ienu = source.GetEnumerator(); ienu.MoveNext(); )
                if (idic.ContainsKey(ienu.Current.Key))
                    idic[ienu.Current.Key] = ienu.Current.Value;
                else
                    idic.Add(ienu.Current.Key, ienu.Current.Value);
        }
    }
}
