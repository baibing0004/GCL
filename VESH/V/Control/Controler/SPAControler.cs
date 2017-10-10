using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using GCL.Project.VESH.V.Control;
using GCL.Project.VESH.V.Control.Session;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;

namespace GCL.Project.VESH.V.Control.Controler {

    /// <summary>
    /// SPA记录文件专用
    /// </summary>
    public class SPAFilter : Stream {
        public string path;
        private Stream stream;
        private Event.CommandEventHandle handle;

        public SPAFilter(Stream stream, string path, GCL.Event.CommandEventHandle handle) {
            this.stream = stream;
            this.path = path;
            this.handle = handle;
        }
        public override bool CanRead {
            get { return true; }
        }

        public override bool CanSeek {
            get { return true; }
        }

        public override bool CanWrite {
            get { return true; }
        }

        public override void Flush() {
            stream.Flush();
        }

        public override long Length {
            get { return stream.Length; }
        }

        public override long Position {
            get {
                return stream.Position;
            }
            set {
                stream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count) {
            return stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin) {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            stream.SetLength(value);
        }
        protected StringBuilder sb = new StringBuilder();
        public override void Write(byte[] buffer, int offset, int count) {
            stream.Write(buffer, offset, count);
            sb.Append(Encoding.UTF8.GetString(buffer, offset, count));
        }
        public override void Close() {
            stream.Close();
            File.WriteAllText(this.path, sb.ToString(), Encoding.UTF8);
            sb.Clear(); sb = null;
            if (this.handle != null) {
                Event.EventArg.CallCommandEventSafely(this.handle);
            }
            base.Close();
        }
    }
    /// <summary>
    /// WebInterface后续页面处理控制器
    /// </summary>
    public class SPAControler : IControler {

        private string paraName;
        protected readonly string extend;
        //记录下spa基础目录
        private string path;
        //记录下spa压缩命令
        private string command;

        /// <summary>
        /// 默认参数名为_page
        /// </summary>
        /// <param name="paraName"></param>
        /// <param name="extend"></param>
        public SPAControler(string paraName, string extend, string path, string command) {
            this.paraName = string.IsNullOrEmpty(paraName) ? "page" : paraName;
            this.extend = extend;
            this.path = path.TrimStart('\\', '/').TrimEnd('\\', '/');
            this.command = command;
        }

        #region IControler Members
        protected string GetResourcePath(HttpRequest request, HttpContext context, SessionDataManager session, string extend) {
            string path = "";
            if (session.Status.StartsWith(".")) {
                path = GCL.IO.Config.ConfigManagement.AppSettings(session.Module.ConfigManager, session.Module.GetRequestClassType(context) + session.Status);
                if (string.IsNullOrEmpty(path)) {
                    //计算路径 放置在其class相同位置 名称为 状态.aspx等
                    string[] s = request.Path.Split('\\', '/');
                    string[] _path = s[s.Length - 1].Split('.');
                    _path[0] = "";
                    if (_path.Length > 2) {
                        _path[_path.Length - 2] = string.Format("{0}.{1}", session.Status.TrimStart('.'), extend);
                        _path[_path.Length - 1] = "";
                    } else {
                        _path[_path.Length - 1] = string.Format("{0}.{1}", session.Status.TrimStart('.'), extend);
                    }
                    s[s.Length - 1] = string.Join(".", _path).Trim('.');
                    path = string.Join("/", s).TrimEnd('/');
                }
            } else if (!string.IsNullOrEmpty(session.Status)) {
                //如果Status为空值，那么不作任何处理
                path = string.Format("{0}/{1}.{2}", request.ApplicationPath, session.Status, extend);
            } else {
                //如果Status为空值 直接替换后缀名
                string[] s = request.Path.Split('\\', '/');
                string[] _path = s[s.Length - 1].Split('.');
                _path[_path.Length - 1] = extend;
                s[s.Length - 1] = string.Join(".", _path);
                path = string.Join("/", s);
            }

            //context.Response.Write(session.Module.GetRequestClassType(context)+"<br/>");
            //context.Response.Write(path);
            //context.Server.Execute(path, true);
            //context.Server.Transfer(new V.View.ASPermissionHandler(), true);
            return path;
        }

        /// <summary>
        /// 专门处理/GCLVESHTest/这种
        /// </summary>
        /// <param name="path"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        public string ReplaceModulePath(string path, string application) {
            application = application.TrimStart('/', '\\').TrimEnd('/', '\\');
            var s = path.Split('\\', '/');
            var w = 0;
            var has = false;
            s.Where(p => {
                if (p.Equals(application, StringComparison.InvariantCultureIgnoreCase) && !has) {
                    s[w + 1] = this.path;
                    has = true;
                }
                w++;
                return false;
            }).Count();
            s[s.Length - 1] = s[s.Length - 1].Replace("." + this.extend, ".htm");
            return string.Join("/", s);
        }


        /// <summary>
        /// 专门处理../../这种
        /// </summary>
        /// <param name="p"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        protected string ReplaceModulePath2(string path, string application) {
            application = application.TrimStart('/', '\\').TrimEnd('/', '\\');
            var s = path.Split('\\', '/');
            var w = 0;
            var has = false; ;
            s.Where(p => {
                if (p.Equals(application, StringComparison.InvariantCultureIgnoreCase)) {
                    has = true;
                } else if (has) {
                    has = false;
                    s[w] = this.path;
                }
                w++;
                return false;
            }).Count();
            s[s.Length - 1] = s[s.Length - 1].Replace("." + this.extend, ".htm");
            return string.Join("/", s);
        }

        public virtual void Execute(HttpRequest request, HttpResponse response, HttpContext context, SessionDataManager session) {
            string path = GetResourcePath(request, context, session, this.extend);
            try {
                //context.Response.Write(session.Module.GetRequestClassType(context)+"<br/>");
                //context.Response.Write(path);

                var filePath = ReplaceModulePath(path, request.ApplicationPath);
                filePath = context.Server.MapPath(filePath);
                FileInfo file = new FileInfo(filePath);
                if (!Directory.Exists(file.DirectoryName)) {
                    Directory.CreateDirectory(file.DirectoryName);
                }
                response.Filter = new SPAFilter(response.Filter, filePath, new GCL.Event.CommandEventHandle(new Action(() => {
                    SPA(filePath, request.Url, request.ApplicationPath);
                })));
                context.Server.Execute(path, true);
            } catch (System.Threading.ThreadAbortException) {
            } catch (HttpException ex) {
                if (ex.GetHttpCode() == 404) throw ex;
                throw new Exception(string.Format("GCL.Project.VESH.V.Control.Controler.PageControler\r\nNext URL:{0}\r\nInnerException:{1}", path, ex.InnerException == null ? ex.ToString() : ex.InnerException.ToString()));
            }
        }

        public static Regex linkRegex = new Regex("<link[^>]+>", RegexOptions.IgnoreCase);
        public static Regex jsRegex = new Regex("<script[^>]+></script>", RegexOptions.IgnoreCase);
        public static Regex cssRegex = new Regex(@"[^""']+\.css", RegexOptions.IgnoreCase);
        public static Regex jssRegex = new Regex(@"[^""']+\.js", RegexOptions.IgnoreCase);
        public static Regex headRegex = new Regex(@"<html[^\>]*>", RegexOptions.IgnoreCase);
        public static Regex pathRegex = new Regex(@"path[^'^\""]*['\""][^'^\""]+['\""]\s*,?", RegexOptions.IgnoreCase);
        public static Regex hostRegex = new Regex(@"host[^'^\""]*['\""][^'^\""]+['\""]\s*,?", RegexOptions.IgnoreCase);
        public static Regex paramRegex = new Regex(@"params\s*:\s*\[['""][^'^""]*['""]\s*,?", RegexOptions.IgnoreCase);

        private void SPA(string filePath, Uri url, string application) {
            application = application.TrimStart('/', '\\').TrimEnd('/', '\\');
            var cssstack = new System.Collections.ArrayList();
            var jsstack = new System.Collections.ArrayList();
            //开始处理SPA
            #region 获取其中js与css路径，并全部替换掉，只保留顶部css位置与底部js位置 并且写入css与js堆栈 并且替换文件内容与头部appcatch文件内容 并且写入appcache文件
            string fileName = new FileInfo(filePath).Name;
            fileName = fileName.Substring(0, fileName.Length - 4);
            //获取到真实的fileName;
            var content = File.ReadAllText(filePath);
            var firstcsspath = "";
            var lastjspath = "";
            var cssidic = new Dictionary<string, bool>();
            var jsidic = new Dictionary<string, bool>();
            foreach (Match m in linkRegex.Matches(content)) {
                if (string.IsNullOrEmpty(firstcsspath)) firstcsspath = m.Value;
                cssidic[cssRegex.Match(m.Value).Value] = true;
                cssstack.Add(cssRegex.Match(m.Value).Value);
            }
            foreach (Match m in jsRegex.Matches(content)) {
                jsidic[jssRegex.Match(m.Value).Value] = true;
                jsstack.Add(jssRegex.Match(m.Value).Value);
                lastjspath = m.Value;
            }

            content = linkRegex.Replace(content, new MatchEvaluator(p => {
                if (firstcsspath.Equals(p.Value))
                    return ReplaceModulePath2(firstcsspath.Replace(".css", ".min.css"), "..");
                else return "";
            }));
            content = jsRegex.Replace(content, new MatchEvaluator(p => {
                if (lastjspath.Equals(p.Value))
                    return ReplaceModulePath2(lastjspath.Replace(".js", ".min.js"), "..");
                else return "";
            }));
            content = headRegex.Replace(content, headRegex.Match(content).Value.Replace(">", " manifest=\"/" + ((!string.IsNullOrEmpty(application) && url.AbsoluteUri.IndexOf(application) >= 0) ? (application + "/") : "") + this.path.TrimEnd('/', '\\').TrimStart('/', '\\') + "/" + fileName + ".appcache\">"));
            File.WriteAllText(filePath, content);
            firstcsspath = ReplaceModulePath2(cssRegex.Match(firstcsspath).Value.Replace(".css", ".min.css"), "..");
            lastjspath = ReplaceModulePath2(jssRegex.Match(lastjspath).Value.Replace(".js", ".min.js"), "..");
            #endregion

            var sb = new StringBuilder();

            #region 填写minifest.appcache
            try {
                sb.AppendLine("CACHE MANIFEST");
                sb.AppendLine("#version " + GCL.Common.Tool.FormatDate("yyyy.MM.dd.hhss", DateTime.Now));
                sb.AppendLine("CACHE:");
                var s = ReplaceModulePath(url.AbsolutePath, application).Split('/', '\\');
                s[s.Length - 1] = fileName + ".htm";
                string host = string.Format("http://{0}{1}", url.Host, url.Port.Equals(80) ? "" : (":" + url.Port));
                sb.AppendFormat("{0}{1}", host, string.Join("/", s));
                sb.AppendLine();
                if (firstcsspath.StartsWith("http")) sb.AppendLine(firstcsspath);
                else {
                    sb.AppendFormat("{0}/{1}/{2}", host, application, firstcsspath.Replace("../", "").Replace("..\\", ""));
                    sb.AppendLine();
                }
                if (lastjspath.StartsWith("http")) sb.AppendLine(lastjspath);
                else {
                    sb.AppendFormat("{0}/{1}/{2}", host, application, lastjspath.Replace("../", "").Replace("..\\", ""));
                    sb.AppendLine();
                }
                //少资源文件
                sb.AppendLine("NETWORK:");
                sb.AppendLine("*");
                File.WriteAllText(HttpContext.Current.Server.MapPath("/" + application + "/" + this.path.TrimEnd('/', '\\').TrimStart('/', '\\') + "/" + fileName + ".appcache"), sb.ToString(), Encoding.UTF8);

            } catch (Exception ex) {
                throw ex;
            } finally {
                sb.Clear();
            }
            #endregion

            #region 分析config.js 并将path全部替换掉，并且写入css与js堆栈底部
            try {
                var host = "";
                jsstack.ToArray().Where(j => {
                    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(j.ToString()));

                    if (file.Name.EndsWith("config.js", StringComparison.InvariantCultureIgnoreCase) || file.Name.EndsWith(fileName + ".js", StringComparison.InvariantCultureIgnoreCase)) {
                        //开始处理config.js
                        var config = File.ReadAllText(HttpContext.Current.Server.MapPath(j.ToString()), Encoding.UTF8);
                        Match hostMatch = hostRegex.Match(config);
                        host = hostMatch.Success ? hostMatch.Value.Trim(',').Trim().Replace("host", "").Replace(":", "").Replace("\"", "").Replace("'", "") : host;
                        foreach (Match m in pathRegex.Matches(config)) {
                            var paths = m.Value.Trim(',').Trim().Replace("path", "").Replace(":", "").Replace("\"", "").Replace("'", "");
                            paths.Split(';').Where(p => {
                                p = p.Trim();
                                if (p.StartsWith("http") || p.StartsWith("../") || p.StartsWith("..\\")) {

                                } else {
                                    p = host.TrimEnd('/', '\\') + "/" + p;
                                }
                                if (p.EndsWith(".js")) {
                                    if (!jsidic.ContainsKey(p)) {
                                        jsidic[p] = true;
                                        if (p.StartsWith("http")) {
                                            sb.AppendLine(GCL.Common.Tool.DownloadString(p, ""));
                                        } else {
                                            sb.AppendLine(File.ReadAllText(HttpContext.Current.Server.MapPath(p), Encoding.UTF8));
                                        }
                                    }
                                } else if (p.EndsWith(".css")) {
                                    if (!cssidic.ContainsKey(p)) {
                                        cssidic[p] = true;
                                        cssstack.Add(p);
                                    }
                                }
                                return false;
                            }).Count();
                        }
                        config = pathRegex.Replace(config, new MatchEvaluator(p => {
                            return "spapath:true" + (p.Value.IndexOf(",") >= 0 ? "," : "");
                        }));
                        config = paramRegex.Replace(config, new MatchEvaluator(m => {
                            var path = m.Value.Trim(',').Trim().Substring(6).Replace(":", "").Replace("[", "").Replace("'", "").Replace("\"", "");
                            if (path.IndexOf("<") >= 0) return m.Value;
                            else if (path.Length > 0 && (path.EndsWith(".part", StringComparison.InvariantCultureIgnoreCase) || path.EndsWith(".page", StringComparison.InvariantCultureIgnoreCase) || path.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase) || path.EndsWith(".htm", StringComparison.InvariantCultureIgnoreCase) || path.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase) || path.EndsWith(".php", StringComparison.InvariantCultureIgnoreCase) || path.EndsWith(".do", StringComparison.InvariantCultureIgnoreCase))) {
                                if (!path.StartsWith("http") || path.StartsWith("../")) {
                                    path = url.OriginalString.Replace(url.Segments[url.Segments.Length - 1], path.Trim()).Split('?')[0];
                                }
                                try {
                                    return string.Format("params:['{0}'{1}", GCL.Common.Tool.DownloadString(path, "").Replace("'", "\\'").Replace("\r", "").Replace("\n", ""), m.Value.EndsWith(",") ? "," : "");
                                } catch (Exception ex) {
                                    throw ex;
                                }
                            } else return m.Value;
                        }));
                        sb.AppendLine(config);
                    } else
                        sb.AppendLine(File.ReadAllText(HttpContext.Current.Server.MapPath(j.ToString()), Encoding.UTF8)); return false;
                }).Count();
                var jsfile = new FileInfo(HttpContext.Current.Server.MapPath(lastjspath));
                if (!Directory.Exists(jsfile.DirectoryName)) Directory.CreateDirectory(jsfile.DirectoryName);
                File.WriteAllText(jsfile.FullName.Replace(".min.js", ".js"), sb.ToString(), Encoding.UTF8);
            } catch (Exception ex) {
                throw ex;
            } finally { sb.Clear(); }

            try {
                cssstack.ToArray().Where(c => {
                    string path2 = HttpContext.Current.Server.MapPath(c.ToString());
                    string path3 = HttpContext.Current.Server.MapPath("/" + application);
                    if (path2.Replace(path3, "").Split('\\', '/').Length == 4) {
                        sb.AppendLine(File.ReadAllText(HttpContext.Current.Server.MapPath(c.ToString())).Replace("../../../", "../../"));
                    } else
                        sb.AppendLine(File.ReadAllText(HttpContext.Current.Server.MapPath(c.ToString()))); return false;
                }).Count();
                var cssfile = new FileInfo(HttpContext.Current.Server.MapPath(firstcsspath));
                if (!Directory.Exists(cssfile.DirectoryName)) Directory.CreateDirectory(cssfile.DirectoryName);
                File.WriteAllText(cssfile.FullName.Replace(".min.css", ".css"), sb.ToString(), Encoding.UTF8);
            } catch (Exception) { throw; } finally { sb.Clear(); }

            #endregion

            #region 压缩css，压缩js文件
            try {
                var cssfile = new FileInfo(HttpContext.Current.Server.MapPath(firstcsspath));
                File.Copy(cssfile.FullName.Replace(".min.css", ".css"), cssfile.FullName, true);
                var jsfile = new FileInfo(HttpContext.Current.Server.MapPath(lastjspath));
                File.Copy(jsfile.FullName.Replace(".min.js", ".js"), jsfile.FullName, true);
                return;
                File.WriteAllText(cssfile.FullName, Regex.Replace(
                    File.ReadAllText(cssfile.FullName.Replace(".min.css", ".css"), Encoding.UTF8)
                    , @"\s+", " "), Encoding.UTF8);
                File.WriteAllText(jsfile.FullName, Regex.Replace(
                    File.ReadAllText(jsfile.FullName.Replace(".min.js", ".js"), Encoding.UTF8)
                    , @"\s+", " "), Encoding.UTF8);

                System.Diagnostics.Process.Start(HttpContext.Current.Server.MapPath("/" + application + "/" + command), HttpContext.Current.Server.MapPath(lastjspath.Replace(".min.js", "")) + " js");
                System.Diagnostics.Process.Start(HttpContext.Current.Server.MapPath("/" + application + "/" + command), HttpContext.Current.Server.MapPath(firstcsspath.Replace(".min.css", "")) + " css");
            } catch (Exception ex) {
                throw ex;
            }
            #endregion
        }

        public string ParaName {
            get { return this.paraName; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
        }

        #endregion
    }
}
