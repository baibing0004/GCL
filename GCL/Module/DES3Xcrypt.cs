using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace GCL.Module {
    /// <summary> 
    /// DES3Xcrypt 提供简单方法实现TripleDESCryptoServiceProvider快速加解密 
    /// 源码从网上下载 出处不明
    /// 可以通过Xcrypt.MSCP.GenerateKey()与GenerateIV()获取其Key与Iv
    /// </summary> 
    public class DES3Xcrypt {

        public DES3Xcrypt(string key) {
            this.key = key;
        }

        private string key;
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="pToEncrypt">源字符串</param>
        /// <param name="sKey">加密KEY</param>
        /// <returns>加密好的字符串</returns>
        public string TripleDESEncrypt(string pToEncrypt) {
            try {
                StringBuilder ret = new StringBuilder();
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.Key = ASCIIEncoding.UTF8.GetBytes(this.key);
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.Zeros;
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

                byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                foreach (byte b in ms.ToArray()) {
                    ret.AppendFormat("{0:X2}", b);
                }
                return ret.ToString();
            } catch (Exception ex) {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="pToDecrypt">待解密串</param>
        /// <param name="sKey">加密KEY</param>
        /// <returns>解密后的字符串</returns>
        public string TripleDESDecrypt(string pToDecrypt) {
            try {
                byte[] buffer = new byte[pToDecrypt.Length / 2];
                for (int i = 0; i < (pToDecrypt.Length / 2); i++) {
                    int num2 = Convert.ToInt32(pToDecrypt.Substring(i * 2, 2), 0x10);
                    buffer[i] = (byte)num2;
                }
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.Key = ASCIIEncoding.UTF8.GetBytes(this.key);
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.Zeros;
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

                cs.Write(buffer, 0, buffer.Length);
                cs.FlushFinalBlock();
                cs.Close();
                ms.Close();
                return Encoding.UTF8.GetString(ms.ToArray()).Replace("\0", "");
            } catch (Exception ex) {
                return ex.ToString();
            }
        }
    }
}
