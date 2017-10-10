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
    /// 用于处理一般的IO操作
    /// </summary>
    public class IOTool : Tool {

        /// <summary>
        /// 用于在两个Stream之间完成交换工作 但不关闭流
        /// </summary>
        /// <param name="source">输入流</param>
        /// <param name="result">输出流</param>
        /// <param name="bufferedSize">缓存块大小</param>
        public static void Transport(Stream source, Stream result,
            int bufferedSize) {
            byte[] data = new byte[bufferedSize];
            int len = 0;
            while ((len = source.Read(data, 0, data.Length)) >= 0)
                result.Write(data, 0, len);
            result.Flush();
        }

        /// <summary>
        /// 用于在两个Stream之间完成交换工作 但不关闭流 默认缓存块1024字节
        /// </summary>
        /// <param name="source">输入流</param>
        /// <param name="result">输出流</param>
        public static void Transport(Stream source, Stream result) {
            Transport(source, result, 1024);
        }

        /// <summary>
        /// 用于在TextReader与TextWriter之间完成交换工作 但不关闭
        /// </summary>
        /// <param name="source">TextReader对象</param>
        /// <param name="result">TextWriter对象</param>
        public static void Transport(TextReader source, TextWriter result, int bufferedSize) {
            char[] data = new char[bufferedSize];
            int len = 0;
            while ((len = source.Read(data, 0, data.Length)) >= 0)
                result.Write(data, 0, len);
            result.Flush();
        }

        /// <summary>
        /// 用于在TextReader与TextWriter之间完成交换工作 但不关闭 1024个字符
        /// </summary>
        /// <param name="source">TextReader对象</param>
        /// <param name="result">TextWriter对象</param>
        public static void Transport(TextReader source, TextWriter result) {
            Transport(source, result, 1024);
        }

        /// <summary>
        /// 在IDictionary对象之间互相转换
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
                //保存文件流的前4个字节
                byte[] bytes = new byte[3];

                //保存当前Seek位置
                long origPos = stream.Seek(0, SeekOrigin.Begin);
                stream.Read(bytes, Convert.ToInt32(origPos), Convert.ToInt32(stream.Length >= 3 ? 3 : stream.Length));

                //根据文件流的前4个字节判断Encoding
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

                //恢复Seek位置　　　 
                stream.Seek(origPos, SeekOrigin.Begin);
            }
            return null;
        }


        private static Encoding GBKEncoding = System.Text.Encoding.GetEncoding("GBK");
        /// <summary>
        /// 使用不定长数组转换成GBK字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetGBKString(byte[] data) {
            return GBKEncoding.GetString(data).Trim('\0');
        }
        /// <summary>
        /// 使用定长数组转换成GBK字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetGBKString(byte[] data, int start, int length) {
            return GBKEncoding.GetString(data, start, length).Trim('\0');
        }
        /// <summary>
        /// 使用GBK字符串转成不定长Byte数组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] GetGBKBytes(string data) {
            return GBKEncoding.GetBytes(data);
        }
        /// <summary>
        /// 使用GBK字符串转成定长Byte数组（后位补0)
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
        /// 将时间转换成BCD码
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static byte[] DateTime2BCD(DateTime date) {
            return String2BCD((date.ToString("yyMMddHHmmss")));
        }

        /// <summary>
        /// BCD设置成UTC时间
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DateTime BCD2DateTime(byte[] data) {
            return DateTime.ParseExact(BCD2String(data), "yyMMddHHmmss", System.Globalization.DateTimeFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// 字节数组转为16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Bytes2HexString(byte[] bytes) {
            if (bytes == null || bytes.Length == 0)
                throw new ArgumentException("参数不正确");

            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes) {
                sb.Append(string.Format("{0:X2}", b));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 16进制字符串转为二进制 字符串格式如：01A20ED355FD
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        public static byte[] HexString2Bytes(string hexStr, int length) {
            string machStr = "^([0-9A-Fa-f][0-9A-Fa-f])*$";
            Regex regex = new Regex(machStr);
            if (!regex.IsMatch(hexStr))
                throw new ArgumentException("参数格式不正确");

            int count = hexStr.Length / 2;
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++) {
                bytes[i] = byte.Parse(hexStr.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            byte[] ret = new byte[length];
            if (length - bytes.Length < 0) throw new IndexOutOfRangeException("字符串转换长度超过允许的数据长度!");
            Array.Copy(bytes, 0, ret, length - bytes.Length, bytes.Length);
            return ret;
        }

        /// <summary>
        /// 8421BCD码转字符串
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
        /// 字符串转6位定长BCD
        /// </summary>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static byte[] String2BCD(string asc) {
            return String2BCD(asc, 6);
        }

        /// <summary>
        /// 字符串转定长的BCD码
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
        /// 压缩单个文件
        /// </summary>
        /// <param name="fileToZip">要压缩的文件</param>
        /// <param name="zipedFile">压缩后的文件</param>
        /// <param name="compressionLevel">压缩等级</param>
        /// <param name="blockSize">每次写入大小</param>
        public static void ZipFile(string fileToZip, string zipedFile, int compressionLevel, int blockSize) {
            //如果文件没有找到，则报错
            if (!System.IO.File.Exists(fileToZip)) {
                throw new System.IO.FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
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
        /// 压缩单个文件
        /// </summary>
        /// <param name="fileToZip">要进行压缩的文件名</param>
        /// <param name="zipedFile">压缩后生成的压缩文件名</param>
        public static void ZipFile(string fileToZip, string zipedFile) {
            //如果文件没有找到，则报错
            if (!File.Exists(fileToZip)) {
                throw new System.IO.FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
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
        /// 压缩多层目录
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
        /// 递归遍历目录
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

            foreach (string file in filenames)// 遍历所有的文件和目录
	        {

                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
	            {
                    string pPath = parentPath;
                    pPath += file.Substring(file.LastIndexOf("\\") + 1);
                    pPath += "\\";
                    ZipSetp(file, s, pPath);
                } else // 否则直接压缩文件
	            {
                    //打开压缩文件
                    using (FileStream fs = File.OpenRead(file)) {
                        string fileName = parentPath + file.Substring(file.LastIndexOf("\\") + 1);
                        ZipEntry entry = new ZipEntry(fileName);
                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;
                        s.PutNextEntry(entry);
                        byte[] buffer = new byte[4096];//这一行报错，压缩到一定大小就报错，我知道是内存溢出了，但是不知道怎么解决
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
        /// 解压缩一个 zip 文件。
        /// </summary>
        /// <param name="zipedFile">The ziped file.</param>
        /// <param name="strDirectory">The STR directory.</param>
        /// <param name="password">zip 文件的密码。</param>
        /// <param name="overWrite">是否覆盖已存在的文件。</param>
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
