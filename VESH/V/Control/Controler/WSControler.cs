using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GCL.Project.VESH.V.Control.Controler {
    /// <summary>
    /// 会话交互对象
    /// 针对一次交互
    /// </summary>
    public class WSInteractive {
        public string _id { get; set; }
        public IDictionary<string, IDictionary<string, string>> request { get; set; }
        public string GetValue() {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try {
                request.Values.FirstOrDefault().Where(p => { sb.AppendFormat("&{0}={1}", p.Key, p.Value); return false; }).Count();
                return sb.ToString().TrimStart('&');
            } finally {
                sb.Clear();
                sb = null;
            }
        }
    }
    /// <summary>
    /// 针对一个链接 一个 ServerSession
    /// </summary>
    public class WSSession {
        public string Cookie { get; set; }
        public GCL.Net.WebClient Client { get; set; }
        public IWebSocketConnection Conn { get; set; }
        public Queue<WSInteractive> Queue { get; set; }
        public Task Task { get; set; }
        //记录链接所监听的所有会话信息
        public IDictionary<string, IList<string>> Methods { get; set; }
    }

    /// <summary>
    /// 针对一个监听 一个 ServerSession
    /// </summary>
    public class WSServerSession {
        public WebSocketServer Server { get; set; }
        public IDictionary<IWebSocketConnection, WSSession> Sessions { get; set; }
        public IDictionary<string, IDictionary<string, WSSession>> MethodSession { get; set; }
    }
    /// <summary>
    /// 用于处理Action返回session.Status {port:'8181',ope:'add/restart/close'}等启动信息
    /// </summary>
    public class WSControler : IControler {
        private int port;
        internal static IDictionary<int, WSServerSession> idic = new Dictionary<int, WSServerSession>();
        public static string host;
        public static int socketmax = 0;
        /// <summary>
        /// 后缀名 支持 ws
        /// </summary>
        /// <param name="param"></param>
        /// <param name="port">默认端口号</param>
        /// <param name="socketmax">单个SocketServer的最大等待链接数</param>
        public WSControler(string param, int port, int socketmax) {
            this.ParaName = string.IsNullOrEmpty(param) ? "ws" : param;
            this.port = port <= 0 ? 8181 : port;
            WSControler.socketmax = WSControler.socketmax == 0 ? socketmax : WSControler.socketmax;
        }

        public void Execute(HttpRequest request, HttpResponse response, HttpContext context, Session.SessionDataManager session) {
            if (string.IsNullOrEmpty(host)) {
                host = string.Format("http://{0}/", request.Url.Host);
            }
            if (Logger == null) { Logger = session.Logger; }
            int port = this.port;
            var ret = GCL.Common.Tool.Deserialize<IDictionary<string, string>>(session.Status);
            if (!Int32.TryParse(ret["port"], out port)) {
                port = this.port;
            }
            switch (Convert.ToString(ret["ope"]).Trim().ToLower()) {
                case "add":
                default:
                    if (!idic.ContainsKey(port)) {
                        StartServer(port);
                    }
                    break;
                case "close":
                    if (idic.ContainsKey(port)) {
                        StopServer(port);
                    }
                    break;
                case "restart":
                    if (idic.ContainsKey(port)) {
                        StopServer(port);
                    }
                    if (!idic.ContainsKey(port)) {
                        StartServer(port);
                    }
                    break;
            }
            try {
                //一般很少有超出1个DataSet的情况
                response.Clear();
                response.Write(session.Status);
                response.ContentType = GCL.Net.MIME.js;
                response.Expires = 0;
            } catch (Exception ex) {
                response.Clear();
                response.Write("[False]");
                session.Logger.Error(ex.ToString());
            }
        }

        public static void StopServer(int port) {
            lock (idic) {
                try {
                    if (idic.ContainsKey(port)) {
                        idic[port].Sessions.Keys.Where(conn => { conn.Close(); return false; }).Count();
                        idic[port].Server.Dispose();
                    }
                } finally {
                    idic.Remove(port);
                }
            }
        }

        public static void StartServer(int port) {
            lock (idic) {
                if (!idic.ContainsKey(port)) {
                    var allSockets = new Dictionary<IWebSocketConnection, WSSession>();

                    var server = new WebSocketServer("ws://0.0.0.0:" + port);
                    idic[port] = new WSServerSession { Server = server, Sessions = allSockets, MethodSession = new Dictionary<string, IDictionary<string, WSSession>>() };
                    server.Start(socket => {
                        socket.OnOpen = () => {
                            if (socketmax > 0 && allSockets.Count() > socketmax) {
                                socket.Send("{error:\"连接总数已经超过" + socketmax + "限制\"}");
                                socket.Close();
                            } else allSockets.Add(socket, new WSSession { Conn = socket, Queue = new Queue<WSInteractive>(), Methods = new Dictionary<string, IList<string>>() });
                        };
                        socket.OnClose = () => {
                            Console.WriteLine("socket Close! session count:" + (allSockets.Count()));
                            if (allSockets.ContainsKey(socket)) {
                                WSSession session = allSockets[socket];
                                if (session.Methods != null) {
                                    WSServerSession ssession = idic[port];
                                    session.Methods.Where(p => {
                                        //删除Socket的所有会话记录
                                        if (ssession.MethodSession.ContainsKey(p.Key))
                                            p.Value.Where(q => {
                                                if (ssession.MethodSession[p.Key].ContainsKey(q)) ssession.MethodSession[p.Key].Remove(q);
                                                return false;
                                            }).Count();
                                        return false;
                                    }).Count();
                                }
                                allSockets.Remove(socket);
                            }
                        };
                        socket.OnMessage = message => {
                            if (message.StartsWith("{") && message.EndsWith("}")) {
                                try {
                                    var idc = GCL.Common.Tool.DeserializeObject(message);
                                    if (idc.Property("cookies") != null && string.IsNullOrEmpty(allSockets[socket].Cookie)) {
                                        var session = allSockets[socket];
                                        session.Cookie = session.Cookie ?? idc.Property("cookies").Value.ToString();
                                        session.Client = new Net.WebClient() { Encoding = System.Text.Encoding.UTF8 };
                                        idc.Property("cookies").Value.ToString().Split(';').Where(p => {
                                            p = p.Trim();
                                            if (!(p.StartsWith("expire=", StringComparison.InvariantCultureIgnoreCase) || p.StartsWith("domain=", StringComparison.InvariantCultureIgnoreCase) || p.StartsWith("path=", StringComparison.InvariantCultureIgnoreCase))) {
                                                var ps = p.Split('=');
                                                if (ps.Length > 1) {
                                                    session.Client.Cookies.Add(ps[0].Trim(), new System.Net.Cookie(ps[0].Trim(), p.Substring(ps[0].Length + 1)));
                                                }
                                            }
                                            return false;
                                        }).Count();
                                    } else {
                                        var msg = GCL.Common.Tool.DeserializeToObject<WSInteractive>(message);
                                        var session = allSockets[socket];
                                        if (!msg.request.Values.FirstOrDefault().ContainsKey("_id")) msg.request.Values.FirstOrDefault().Add("_id", msg._id);
                                        session.Queue.Enqueue(msg);
                                        if (session.Task == null) {
                                            session.Task = new System.Threading.Tasks.Task(() => {
                                                while (session.Queue.Count > 0)
                                                    try {
                                                        var _msg = session.Queue.Dequeue();
                                                        var ret = DealMessage(_msg, session, idic[port]);
                                                        if (!string.IsNullOrEmpty(ret))
                                                            session.Conn.Send(ret);
                                                    } catch (Exception ex) {
                                                        if (Logger != null) Logger.Error("WSControler StartServer Task 错误:" + ex.ToString());
                                                        session.Conn.Send("{\"error\":\"OnMessage " + ex.Message + "\"}");
                                                    }
                                                session.Task = null;
                                            });
                                            session.Task.Start();
                                        }
                                    }

                                } catch (Exception ex) {
                                    socket.Send("{\"error\":\"" + ex.ToString() + "\"");
                                    if (Logger != null) Logger.Error("WSControler StartServer 错误:" + ex.ToString());
                                }
                            } else {
                                socket.Send("{\"error\":\"请输入{'_id':'***',request:{method:{param1:'',param2:''}}}格式的JSON数据,并准备处理接受{'_id':'****',response:''}格式的返回数据!\"}");
                            }
                        };
                    });
                }
            }
        }

        public static string DealMessage(WSInteractive msg, WSSession session, WSServerSession ssession) {
            string path = msg.request.Keys.FirstOrDefault();
            if (path.IndexOf(".") >= 0) {
                path = path.StartsWith("http://") ? path : string.Format("{0}{1}", host, path);
                var ret = session.Client.PostString(path, msg.GetValue());
                return GCL.Common.Tool.SerializeToString(new { _id = msg._id, response = ret });
            } else {
                string name = path.ToLower();
                if (path.IndexOf(".") >= 0) {
                    //去除访问的后缀
                    var names = path.Split('.');
                    names[names.Length - 1] = "";
                    name = string.Join(".", names).Trim('.');
                }
                //记录访问的会话，以便WSConnection进行Push操作
                if (!ssession.MethodSession.ContainsKey(name)) ssession.MethodSession[name] = new Dictionary<string, WSSession>();
                if (!ssession.MethodSession[name].ContainsKey(msg._id)) {
                    ssession.MethodSession[name][msg._id] = session;
                    //Socket的Session记录其被记录的方法和请求调用ID 已被Socket关闭时删除所有的session记录
                    if (!session.Methods.ContainsKey(path))
                        session.Methods.Add(path, new List<string>());
                    session.Methods[path].Add(msg._id);
                }
                return GCL.Common.Tool.SerializeToString(new { _id = msg._id, response = GCL.Common.Tool.SerializeToString(new { _regist = name }) });
            }
        }

        public string ParaName {
            get;
            private set;
        }

        public static IO.Log.Logger Logger;

        public void Dispose() {
            idic.Keys.Where(k => {
                try {
                    StopServer(k);
                } catch (Exception ex) {
                    if (Logger != null) Logger.Error("WSControler Dispose错误:" + ex.ToString());
                } return false;
            }).Count();
        }
    }
}