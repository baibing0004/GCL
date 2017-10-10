using System;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Threading;

namespace Test {

    /// <summary> 

    /// Class1 的摘要说明。 

    /// </summary> 

    public class dllEncrypt {

        //mSCP. GenerateKey（）来生成，矢量的生成也可以用mCSP.GenerateIV()来生成。
        //密钥 
        //private const string sKey = "qJzGEh6hESZDVJeCnFPGuxzaiB7NLQM3";
        private string sKey = "dOybSxxAFaTG/X87amUy8EnKJZVXTRzQ";

        //矢量，矢量可以为空 
        //private const string sIV = "qcDY6X+aPLw="; 
        private string sIV = "aQ5FMgJYVTg=";

        //构造一个对称算法 

        public static SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();


        public dllEncrypt() { }
        public dllEncrypt(string key, string iv) {
            sKey = key;
            sIV = iv;
        }


        #region public string EncryptString(string Value)

        /// <summary> 

        /// 加密字符串 

        /// </summary> 

        /// <param name="Value">输入的字符串</param> 

        /// <returns>加密后的字符串</returns> 

        public string EncryptString(string Value) {

            ICryptoTransform ct;

            MemoryStream ms;

            CryptoStream cs;

            byte[] byt;

            mCSP.Key = Convert.FromBase64String(sKey);

            mCSP.IV = Convert.FromBase64String(sIV);

            //指定加密的运算模式 

            mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;

            //获取或设置加密算法的填充模式 

            mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

            ct = mCSP.CreateEncryptor(mCSP.Key, mCSP.IV);

            byt = Encoding.UTF8.GetBytes(Value);

            ms = new MemoryStream();

            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);

            cs.Write(byt, 0, byt.Length);

            cs.FlushFinalBlock();

            cs.Close();

            return Convert.ToBase64String(ms.ToArray());

        }

        #       endregion


        #      region public string DecryptString(string Value)

        /// <summary> 

        /// 解密字符串 

        /// </summary> 

        /// <param name="Value">加过密的字符串</param> 

        /// <returns>解密后的字符串</returns> 

        public string DecryptString(string Value) {

            ICryptoTransform ct;

            MemoryStream ms;

            CryptoStream cs;

            byte[] byt;

            mCSP.Key = Convert.FromBase64String(sKey);

            mCSP.IV = Convert.FromBase64String(sIV);

            mCSP.Mode = System.Security.Cryptography.CipherMode.ECB;

            mCSP.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

            ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);

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