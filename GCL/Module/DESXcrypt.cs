using System;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Threading;
namespace GCL.Module {
    /// <summary> 
    /// XNcrypt 提供简单方法实现TripleDESCryptoServiceProvider快速加解密 
    /// 源码从网上下载 出处不明
    /// 可以通过Xcrypt.MSCP.GenerateKey()与GenerateIV()获取其Key与Iv
    /// </summary> 
    public class DESXcrypt {
        //MSCP.GenerateKey（）来生成，矢量的生成也可以用MSCP.GenerateIV()来生成。
        //密钥 
        //private const string sKey = "qJzGEh6hESZDVJeCnFPGuxzaiB7NLQM3";
        /// <summary>
        /// 默认密钥
        /// </summary>
        private string sKey = "dOybSxxAFaTG/X87amUy8EnKJZVXTRzQ";
        //private const string sIV = "qcDY6X+aPLw="; 
        /// <summary>
        /// 默认矢量
        /// </summary>
        private string sIV = "aQ5FMgJYVTg=";

        private SymmetricAlgorithm mscp;
        /// <summary>
        /// 构造一个对称算法 
        /// </summary>
        public static SymmetricAlgorithm MSCP = new TripleDESCryptoServiceProvider();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        public DESXcrypt(string key, string iv) : this(key, iv, new TripleDESCryptoServiceProvider()) { }

        /// <summary>
        /// 必须提供迷钥与矢量，保证加密成功
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="mscp"></param>
        public DESXcrypt(string key, string iv, SymmetricAlgorithm mscp) {
            sKey = key;
            sIV = iv;
            this.mscp = mscp;
        }

        #region public string Encrypt(string Value)
        /// <summary> 
        /// 加密字符串 
        /// </summary> 
        /// <param name="Value">输入的字符串</param> 
        /// <returns>加密后的字符串</returns> 
        public string Encrypt(string Value) {
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            this.mscp.Key = Convert.FromBase64String(sKey);
            this.mscp.IV = Convert.FromBase64String(sIV);
            //指定加密的运算模式 
            this.mscp.Mode = System.Security.Cryptography.CipherMode.CBC;
            //获取或设置加密算法的填充模式 
            this.mscp.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ct = this.mscp.CreateEncryptor(this.mscp.Key, this.mscp.IV);
            byt = Encoding.UTF8.GetBytes(Value);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Convert.ToBase64String(ms.ToArray());
        }
        #       endregion

        #      region public string Decrypt(string Value)
        /// <summary> 
        /// 解密字符串 
        /// </summary> 
        /// <param name="Value">加过密的字符串</param> 
        /// <returns>解密后的字符串</returns> 
        public string Decrypt(string Value) {
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            this.mscp.Key = Convert.FromBase64String(sKey);
            this.mscp.IV = Convert.FromBase64String(sIV);
            this.mscp.Mode = System.Security.Cryptography.CipherMode.CBC;
            this.mscp.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            ct = this.mscp.CreateDecryptor(this.mscp.Key, this.mscp.IV);
            byt = Convert.FromBase64String(Value);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
        #      endregion
    }
}