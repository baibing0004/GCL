using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using GCL.Common;

namespace GCL.Net {
    /// <summary>
    /// 自定义WebClient共享多次请求中的HttpCookie
    /// 且只重载了UploadString未重载其余内容
    /// </summary>
    public class WebClient : System.Net.WebClient {
        public WebClient() {
            this.Cookies = new Dictionary<string, Cookie>();
            this.Timeout = 10000;
        }
        public IDictionary<string, Cookie> Cookies { get; private set; }
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address) {
            WebRequest req = base.GetWebRequest(address);
            req.Timeout = this.Timeout;
            if (req is HttpWebRequest) {
                ((HttpWebRequest)req).ReadWriteTimeout = this.Timeout;
            }
            return req;
        }
        /// <summary>
        /// 准备cookie
        /// </summary>
        protected void PrepareCookie() {
            if (this.Cookies.Count > 0) {
                StringBuilder sb = new StringBuilder();
                try {
                    foreach (Cookie cookie in Cookies.Values) {
                        sb.AppendFormat("{0}={1};", cookie.Name, cookie.Value);
                    }
                    this.Headers.Set("Cookie", sb.ToString().TrimEnd(','));
                } finally {
                    sb.Clear();
                    sb = null;
                }
                //this.Cookies.Clear();
            }
        }

        /// <summary>
        /// 设置cookie
        /// </summary>
        protected void ResetCookie() {
            if (!string.IsNullOrEmpty(this.ResponseHeaders.Get("Set-Cookie"))) {
                string[] strCookies = this.ResponseHeaders.Get("Set-Cookie").Split(',');
                int w = 0;
                strCookies = strCookies.Select(p => {
                    try {
                        if (p.Split(';')[0].IndexOf("=") < 0) return null;
                        if (p.EndsWith("=")) return p;
                        var ret = p.Split('=');
                        switch (ret[ret.Length - 1].Trim().ToLower()) {
                            //英语
                            case "mon":
                            case "tue":
                            case "wed":
                            case "thu":
                            case "fri":
                            case "sat":
                            case "sun":
                            //西班牙
                            case "lun":
                            case "mar":
                            case "mier":
                            case "juev":
                            case "vier":
                            case "sab":
                            case "dom":
                                return p.Replace("=" + ret[ret.Length - 1].Trim(), "=" + strCookies[w + 1]);
                            default:
                                return p;
                        }
                    } finally {
                        w++;
                    }
                }).Where(p => !string.IsNullOrEmpty(p)).ToArray();

                IDictionary<string, string> idic = new Dictionary<string, string>();
                foreach (string strCookie in strCookies) {
                    string[] _strCookies = strCookie.Split(';');
                    Cookie cookie = new Cookie();
                    foreach (string _strCookie in _strCookies) {
                        string[] _strCookieValues = _strCookie.Split('=');
                        switch (_strCookieValues[0].Trim().ToLower()) {
                            case "domain":
                                cookie.Domain = _strCookieValues[1].Trim();
                                break;
                            case "expires":
                                cookie.Expires = Tool.GreenwishTimeString2LocalTime(_strCookieValues[1].Trim());
                                break;
                            case "path":
                                cookie.Path = _strCookieValues[1].Trim();
                                break;
                            case "httponly":
                                cookie.HttpOnly = true;
                                break;
                            case "secure":
                                cookie.Secure = true;
                                break;
                            default:
                                cookie.Name = _strCookieValues[0].Trim();
                                cookie.Value = _strCookie.Substring(cookie.Name.Length).TrimStart('=');
                                break;
                        }
                    }
                    this.Cookies[cookie.Name] = cookie;
                }
            }
        }

        /// <summary>
        /// 自带cookie的将DownloadData下载数据下载回来
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public new byte[] DownloadData(string url) {
            this.PrepareCookie();
            if ("application/x-www-form-urlencoded".Equals(this.Headers["Content-Type"], StringComparison.CurrentCultureIgnoreCase))//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可
                this.Headers.Remove("Content-Type");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可
            var ret = base.DownloadData(url);
            this.ResetCookie();
            return ret;
        }



        /// <summary>
        /// 重载后的共享cookie的方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public new byte[] UploadData(Uri url, byte[] data) {
            this.PrepareCookie();
            byte[] ret = base.UploadData(url, data);
            this.ResetCookie();
            return ret;
        }

        /// <summary>
        /// 重载后的get请求获取数据页面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public new string DownloadString(string url) {
            this.PrepareCookie();
            if ("application/x-www-form-urlencoded".Equals(this.Headers["Content-Type"], StringComparison.CurrentCultureIgnoreCase))//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可
                this.Headers.Remove("Content-Type");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可            
            string ret = base.DownloadString(url);
            this.ResetCookie();
            return ret;
        }

        /// <summary>
        /// 重载后的get请求获取数据页面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public new string DownloadString(Uri url) {
            this.PrepareCookie();
            if ("application/x-www-form-urlencoded".Equals(this.Headers["Content-Type"], StringComparison.CurrentCultureIgnoreCase))//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可
                this.Headers.Remove("Content-Type");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可
            string ret = base.DownloadString(url);
            this.ResetCookie();
            return ret;
        }


        /// <summary>
        /// Post表单专用,默认的UploadString不是干这个用的
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual string PostString(string url, string data) {
            this.PrepareCookie();
            this.Headers.Add("Content-Type", "application/x-www-form-urlencoded");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可             
            string ret = this.UploadString(url, data);
            this.ResetCookie();
            return ret;
        }

        /// <summary>
        /// 当前时间转换格林威志时间
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
        /// 格林威治时间转换当前时间
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
        /// 当地时间改格林威治时间串
        /// </summary>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public static string LocalTime2GreenwishTimeString(DateTime localTime) {
            return LocalTime2GreenwishTime(localTime).ToString(GreenwishTimeStringFormat);
        }

        /// <summary>
        /// 格林威治时间串转换当地时间
        /// </summary>
        /// <param name="greenwishTime"></param>
        /// <returns></returns>
        public static DateTime GreenwishTimeString2LocalTime(string time) {
            return GreenwishTime2LocalTime(DateTime.ParseExact(time, GreenwishTimeStringFormat, null));
        }
    }
}
