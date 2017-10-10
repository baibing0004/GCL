using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GCL.Project.VESH.V.Control.Session;
using System.Text;
using System.IO;

namespace GCL.Project.VESH.V.Control.Moduler {

    internal class ResponseFilter : Stream {
        private Stream m_sink;
        private long m_position;
        private MemoryStream mo;
        private Encoding encoding;

        public ResponseFilter(Stream sink, Encoding encoding) {
            m_sink = sink;
            mo = new MemoryStream();
            this.encoding = encoding;
        }

        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return false; } }

        public override long Length { get { return 0; } }

        public override long Position {
            get { return m_position; }
            set { m_position = value; }
        }

        public override long Seek(long offset, System.IO.SeekOrigin direction) {
            return 0;
        }

        public override void SetLength(long length) {
            if (m_sink != null)
                m_sink.SetLength(length);
        }

        public string Result {
            get {
                return this.encoding.GetString(mo.ToArray());
            }
        }
        public override void Close() {
            if (m_sink != null) {
                m_sink.Close();
                m_sink.Dispose();
            }
            mo.Close();
            mo.Dispose();
        }

        public override void Flush() {
            if (m_sink != null)
                m_sink.Flush();
            mo.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count) {

            if (m_sink != null)
                return m_sink.Read(buffer, offset, count);
            else return 0;
        }

        public override void Write(byte[] buffer, int offset, int count) {
            if (m_sink != null)
                m_sink.Write(buffer, 0, count);
            //
            mo.Write(buffer, 0, count);
        }
    }

    public class CacheModuler : IModuler {
        private string cacheKey;

        public CacheModuler(string cacheKey) {
            this.cacheKey = cacheKey;
            if (this.cacheKey == null) throw new InvalidOperationException("必须定义所使用的默认存储键值");
        }

        public bool IsSecurity {
            get { return false; }
        }

        public static string REQUESTCACHEKEY = "GCL.Project.VESH.V.Control.Moduler.CacheModuler.OnlyCache";
        public SessionDataManager SessionDatas {
            get { return SessionDataManager.GetCurrentSessionDataManager(); }
        }
        public SessionData GetCacheSession() {
            return SessionDatas[REQUESTCACHEKEY];
        }

        public string GetURLHash(HttpContext context, int val) {
            switch (val) {
                case 1:
                    return string.Format("{0}_{1}", this.cacheKey, context.Request.RawUrl.ToLower());
                case 2:
                    StringBuilder sb = new StringBuilder();
                    try {
                        foreach (string key in context.Request.Form.AllKeys) {
                            sb.AppendFormat("{0}={1}&", key, context.Request.Form[key]);
                        }
                        return string.Format("{0}_{1}{2}", this.cacheKey, context.Request.RawUrl.ToLower(), sb.ToString());
                    } finally {
                        sb.Clear();
                        sb = null;
                    }
                    break;
                case 3:
                    StringBuilder sb2 = new StringBuilder();
                    try {
                        foreach (string key in context.Request.Form.AllKeys) {
                            sb2.AppendFormat("{0}={1}&", key, context.Request.Form[key]);
                        }
                        return string.Format("{0}_{1}{2}{3}", this.cacheKey, SessionDatas.IsLogin ? SessionDatas.UserInfo.UserID : SessionDatas.UserInfo.SessionID, context.Request.RawUrl.ToLower(), sb2.ToString());
                    } finally {
                        sb2.Clear();
                        sb2 = null;
                    }
                    break;
                default:
                    throw new InvalidOperationException("未处理的" + cacheKey + "类型:" + val);

            }
        }
        public void Init(HttpApplication context) {
            context.ReleaseRequestState += new EventHandler(context_ReleaseRequestState);
        }

        void context_ReleaseRequestState(object sender, EventArgs e) {
            string key = Convert.ToString(GetCacheSession()["key"]);
            if (!string.IsNullOrEmpty(key)) {
                var app = sender as HttpApplication;
                if (app.Context.Response.StatusCode == 200) {
                    app.Context.Response.Filter = new ResponseFilter(app.Context.Response.Filter, app.Context.Response.ContentEncoding);
                }
            }
        }

        public void BeginRequest(HttpContext context) {
            int val = 0;
            string key = "";
            if (string.IsNullOrEmpty(context.Request[cacheKey])) return;
            if (int.TryParse(context.Request[cacheKey], out val)) {
                key = GetURLHash(context, val);
            } else {
                key = context.Request[cacheKey];
            }
            GetCacheSession()["key"] = key;
            if (SessionDatas[key].Count > 0) {
                SessionData data = SessionDatas[key];
                context.Response.Clear();
                context.Response.ContentEncoding = Encoding.GetEncoding(Convert.ToString(data["ContentEncoding"]));
                context.Response.ContentType = Convert.ToString(data["ContentType"]);
                context.Response.Output.Write(Convert.ToString(data["Content"]));
                context.Response.End();
            }
        }

        public void EndRequest(HttpContext context) {
            string key = Convert.ToString(GetCacheSession()["key"]);
            if (!string.IsNullOrEmpty(key)) {
                if (SessionDatas[key].Count == 0) {
                    SessionDatas[key]["ContentEncoding"] = context.Response.ContentEncoding.EncodingName;
                    SessionDatas[key]["ContentType"] = context.Response.ContentType;
                    context.Response.Flush();
                    if (context.Response.Filter is ResponseFilter) {
                        SessionDatas[key]["Content"] = ((ResponseFilter)context.Response.Filter).Result;
                    }
                }
            }
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}