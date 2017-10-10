using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GCL.Common;
using System.Windows.Threading;
using System.Windows;
using GCL.Event;

namespace GCL.Event {
    /// <summary>
    /// 特别定义用于动态事件注册和动态事件处理，动态命令注册和处理。
    /// 命令是指唯一接收多个触发，事件是指多个接收多个触发
    /// </summary>
    public class EventTool : Tool {
        /// <summary>
        /// 异步命令存储对象
        /// </summary>
        private static IDictionary<string, DynamicEventFunc> IdicCommand = new Dictionary<string, DynamicEventFunc>();
        /// <summary>
        /// 异步命令执行参数 这里暂时不考虑缓存多次请求参数的情况防止出现内存占用过大的错误
        /// </summary>
        private static IDictionary<string, object[]> IdicCommandParam = new Dictionary<string, object[]>();
        /// <summary>
        /// 异步事件存储对象
        /// </summary>
        private static IDictionary<string, IList<DynamicEventFunc>> IdicEvents = new Dictionary<string, IList<DynamicEventFunc>>();
        /// <summary>
        /// 异步事件执行参数 这里暂时不考虑缓存多次请求参数的情况防止出现内存占用过大的错误
        /// </summary>
        private static IDictionary<string, object[]> IdicEventParam = new Dictionary<string, object[]>();
        /// <summary>
        /// 异步事件存储对象
        /// </summary>
        private static IDictionary<string, IDictionary<string, DynamicEventFunc>> IdicEvents2 = new Dictionary<string, IDictionary<string, DynamicEventFunc>>();

        /// <summary>
        /// 异步事件执行参数 这里暂时不考虑缓存多次请求参数的情况防止出现内存占用过大的错误
        /// </summary>
        private static IDictionary<string, object[]> IdicPopEventParam = new Dictionary<string, object[]>();
        /// <summary>
        /// 异步事件存储对象
        /// </summary>
        private static IDictionary<string, IList<PopDynamicEventFunc>> IdicPopEvents = new Dictionary<string, IList<PopDynamicEventFunc>>();

        /// <summary>
        /// 真实调用事件的处理
        /// 本身未做容错处理
        /// </summary>
        /// <param name="func"></param>
        /// <param name="param"></param>
        /// <param name="needDispatch"></param>
        private static void _CallFunc(DynamicEventFunc func, object[] param, bool needDispatch) {
            if (needDispatch) {
                if (!IsWPFInvoke) {
                    Dispatcher.CurrentDispatcher.Invoke(func, new object[] { param });

                } else
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                        func.Invoke(param);
                    }));
            } else
                func.Invoke(param);
        }

        private static object strIdcCommandLockKey = DateTime.Now;
        private static object strIdcCommandParamLockKey = DateTime.Now;
        /// <summary>
        /// 注册命令，如果已经有调用过的命令，那么会即时调用
        /// </summary>
        /// <param name="key"></param>
        /// <param name="func"></param>
        public static void RegistCommand(string key, DynamicEventFunc func) {
            IdicCommand[key] = func;
            if (IdicCommandParam.ContainsKey(key)) {
                object[] param = null;
                lock (strIdcCommandParamLockKey) {
                    if (IdicCommandParam.ContainsKey(key)) {
                        param = IdicCommandParam[key];
                        IdicCommandParam.Remove(key);
                    }
                }
                if (param != null) {
                    //防止参数多次执行
                    _CallFunc(func, param[1] as object[], (bool)param[0]);
                }
            }
        }

        /// <summary>
        /// 调用注册的命令
        /// </summary>
        /// <param name="key"></param>
        /// <param name="param"></param>
        public static void CallCommand(string key, params object[] param) {
            CallCommand(key, false, param);
        }
        /// <summary>
        /// 调用注册的命令
        /// </summary>
        /// <param name="key"></param>
        /// <param name="param"></param>
        public static void CallCommand(string key, bool needDispatch, params object[] param) {
            if (IdicCommand.ContainsKey(key)) {
                DynamicEventFunc func = null;
                lock (strIdcCommandLockKey) {
                    if (IdicCommand.ContainsKey(key)) {
                        func = IdicCommand[key];
                    }
                }
                if (func != null) {
                    _CallFunc(func, param, needDispatch);
                } else {
                    //说明命令刚刚被删除
                }
            } else {
                //暂时备份命令参数
                IdicCommandParam[key] = new object[] { needDispatch, param };
            }
        }
        public static void CleanCommand(string key) {
            lock (strIdcCommandLockKey) {
                if (IdicCommand.ContainsKey(key))
                    IdicCommand.Remove(key);
            }
            lock (strIdcCommandParamLockKey) {
                if (IdicCommandParam.ContainsKey(key))
                    IdicCommandParam.Remove(key);
            }
        }
        private static object strIdcEventLockKey = DateTime.Now;
        private static object strIdcEventParamLockKey = DateTime.Now;

        /// <summary>
        /// 除事件名外添加方法key值,防止方法多次注册
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ownkey"></param>
        /// <param name="func"></param>
        public static void RegistEvent(string key, string ownkey, DynamicEventFunc func) {
            if (!IdicEvents2.ContainsKey(key)) {
                lock (strIdcEventLockKey) {
                    IdicEvents2[key] = new Dictionary<string, DynamicEventFunc>();
                }
            }
            if (!IdicEvents2[key].ContainsKey(ownkey)) {
                lock (IdicEvents2[key]) {
                    if (!IdicEvents2[key].ContainsKey(ownkey)) {
                        IdicEvents2[key].Add(ownkey, func);
                    }
                }
            };
            if (IdicEventParam.ContainsKey(key)) {
                object[] param = null;
                lock (strIdcEventParamLockKey) {
                    if (IdicEventParam.ContainsKey(key)) {
                        param = IdicEventParam[key];
                        IdicEventParam.Remove(key);
                    }
                }
                if (param != null) {
                    //防止参数多次执行
                    DynamicEventFunc[] funcs = null;
                    if (IdicEvents.ContainsKey(key)) {
                        lock (IdicEvents[key]) {
                            funcs = IdicEvents[key].ToArray();
                        }
                    };
                    if (funcs != null)
                        foreach (DynamicEventFunc _func in funcs)
                            _CallFunc(_func, param[1] as object[], (bool)param[0]);

                    if (IdicEvents2.ContainsKey(key)) {
                        lock (IdicEvents2[key]) {
                            funcs = IdicEvents2[key].Values.ToArray();
                        }
                    };
                    if (funcs != null)
                        foreach (DynamicEventFunc _func in funcs)
                            _CallFunc(_func, param[1] as object[], (bool)param[0]);
                } else {
                    //这里应该是参数刚刚被执行
                }
            }
        }

        public static void RegistEvent(string key, DynamicEventFunc func) {
            if (!IdicEvents.ContainsKey(key)) {
                lock (strIdcEventLockKey) {
                    IdicEvents[key] = new List<DynamicEventFunc>();
                }
            }
            if (!IdicEvents[key].Contains(func)) {
                lock (IdicEvents[key]) {
                    if (!IdicEvents[key].Contains(func)) {
                        IdicEvents[key].Add(func);
                    }
                }
            };
            if (IdicEventParam.ContainsKey(key)) {
                object[] param = null;
                lock (strIdcEventParamLockKey) {
                    if (IdicEventParam.ContainsKey(key)) {
                        param = IdicEventParam[key];
                        IdicEventParam.Remove(key);
                    }
                }
                if (param != null) {
                    //防止参数多次执行
                    DynamicEventFunc[] funcs = null;
                    if (IdicEvents.ContainsKey(key)) {
                        lock (IdicEvents[key]) {
                            funcs = IdicEvents[key].ToArray();
                        }
                    };
                    if (funcs != null)
                        foreach (DynamicEventFunc _func in funcs)
                            _CallFunc(_func, param[1] as object[], (bool)param[0]);

                    if (IdicEvents2.ContainsKey(key)) {
                        lock (IdicEvents2[key]) {
                            funcs = IdicEvents2[key].Values.ToArray();
                        }
                    };
                    if (funcs != null)
                        foreach (DynamicEventFunc _func in funcs)
                            _CallFunc(_func, param[1] as object[], (bool)param[0]);
                } else {
                    //这里应该是参数刚刚被执行
                }
            }
        }
        public static void CallEvent(string key, bool needDispatch, params object[] param) {
            if (IdicEvents.ContainsKey(key)) {
                DynamicEventFunc[] func = null;
                lock (strIdcEventLockKey) {
                    if (IdicEvents.ContainsKey(key)) {
                        func = IdicEvents[key].ToArray();
                    }
                }
                if (func != null) {
                    foreach (DynamicEventFunc _func in func) {
                        _CallFunc(_func, param, needDispatch);
                    }
                } else {
                    //说明参数刚刚被删除
                }
            }
            if (IdicEvents2.ContainsKey(key)) {
                DynamicEventFunc[] func = null;
                lock (strIdcEventLockKey) {
                    if (IdicEvents2.ContainsKey(key)) {
                        func = IdicEvents2[key].Values.ToArray();
                    }
                }
                if (func != null) {
                    foreach (DynamicEventFunc _func in func) {
                        _CallFunc(_func, param, needDispatch);
                    }
                }
            }
            if (!IdicEvents.ContainsKey(key) && !IdicEvents2.ContainsKey(key)) {
                //暂时备份命令参数
                IdicEventParam[key] = new object[] { needDispatch, param };
            }
        }
        public static void CallEvent(string key, params object[] param) {
            CallEvent(key, false, param);
        }
        public static void UnRegistEvent(string key, DynamicEventFunc func) {
            if (IdicEvents.ContainsKey(key)) {
                lock (strIdcEventLockKey) {
                    if (IdicEvents[key].Contains(func)) {
                        IdicEvents[key].Remove(func);
                    }
                }
            }
        }
        public static void CleanEvent(string key) {
            lock (strIdcEventLockKey) {
                if (IdicEvents.ContainsKey(key)) {
                    IList<DynamicEventFunc> lst = IdicEvents[key];
                    IdicEvents.Remove(key);
                    lst.Clear();
                }
            }
            lock (strIdcEventParamLockKey) {
                if (IdicEventParam.ContainsKey(key))
                    IdicCommandParam.Remove(key);
            }
        }

        #region 冒泡事件处理

        private static bool _CallFunc(PopDynamicEventFunc func, object[] param, bool needDispatch) {
            if (needDispatch) {
                if (!IsWPFInvoke) {
                    return Convert.ToBoolean(Dispatcher.CurrentDispatcher.Invoke(func, new object[] { param }));
                } else
                    return Convert.ToBoolean(func.Invoke(param));
                //Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                //    func.Invoke(param);
                //}));
            } else
                return Convert.ToBoolean(func.Invoke(param));
        }
        private static object strPopIdcEventLockKey = DateTime.Now;
        private static object strPopIdcEventParamLockKey = DateTime.Now;

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="top"></param>
        public static void RegistPopEvent(string key, PopDynamicEventFunc func, bool top = false) {
            if (!IdicPopEvents.ContainsKey(key)) {
                lock (strIdcEventLockKey) {
                    IdicPopEvents[key] = new List<PopDynamicEventFunc>();
                }
            }
            if (IdicPopEventParam.ContainsKey(key)) {
                object[] param = null;
                lock (strPopIdcEventParamLockKey) {
                    if (IdicPopEventParam.ContainsKey(key)) {
                        param = IdicPopEventParam[key];
                        IdicPopEventParam.Remove(key);
                    }
                }
                if (param != null) {
                    //防止参数多次执行
                    PopDynamicEventFunc[] funcs = null;
                    if (IdicPopEvents.ContainsKey(key)) {
                        lock (IdicPopEvents[key]) {
                            funcs = IdicPopEvents[key].ToArray();
                        }
                    };
                    if (funcs != null)
                        foreach (PopDynamicEventFunc _func in funcs)
                            if (!_CallFunc(_func, param[1] as object[], (bool)param[0])) break;
                } else {
                    //这里应该是参数刚刚被执行
                }
            }
        }

        public static void CallPopEvent(string key, bool needDispatch, params object[] param) {
            if (IdicPopEvents.ContainsKey(key)) {
                PopDynamicEventFunc[] func = null;
                lock (strPopIdcEventLockKey) {
                    if (IdicPopEvents.ContainsKey(key)) {
                        func = IdicPopEvents[key].ToArray();
                    }
                }
                if (func != null) {
                    foreach (PopDynamicEventFunc _func in func)
                        if (!_CallFunc(_func, param[1] as object[], (bool)param[0])) break;
                } else {
                    //说明参数刚刚被删除
                }
            }

            if (!IdicPopEvents.ContainsKey(key)) {
                //暂时备份命令参数
                IdicPopEventParam[key] = new object[] { needDispatch, param };
            }
        }
        public static void CallPopEvent(string key, params object[] param) {
            CallPopEvent(key, false, param);
        }
        public static void UnRegistPopEvent(string key, PopDynamicEventFunc func) {
            if (IdicPopEvents.ContainsKey(key)) {
                lock (strPopIdcEventLockKey) {
                    if (IdicPopEvents[key].Contains(func)) {
                        IdicPopEvents[key].Remove(func);
                    }
                }
            }
        }
        public static void CleanPopEvent(string key) {
            lock (strPopIdcEventLockKey) {
                if (IdicPopEvents.ContainsKey(key)) {
                    IList<PopDynamicEventFunc> lst = IdicPopEvents[key];
                    IdicPopEvents.Remove(key);
                    lst.Clear();
                }
            }
            lock (strPopIdcEventParamLockKey) {
                if (IdicPopEventParam.ContainsKey(key))
                    IdicPopEventParam.Remove(key);
            }
        }
        #endregion

        static EventTool() {

        }
        public static bool IsWPFInvoke { get; set; }

    }
}
