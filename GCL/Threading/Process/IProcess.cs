using System;
using System.Collections.Generic;
using System.Text;
using GCL.Event;

namespace GCL.Threading.Process {
    
    /// <summary>
    /// 声明进程状态
    /// </summary>
    public enum ProcessState {
        NOSTATE=-1,
        INIT=0,
        READY=1,
        START = 2,
        STOP = 3,
        DISPOSE=4,
        EXCEPTION = 5
    }



    /// <summary>
    /// 作为一种架构设计 完成以下4个方面的要求 基本上可以完成对业务逻辑的初始化，开始，中断，清理等操作。
    /// </summary>
    public interface IProcess:IDisposable {

        /// <summary>
        /// 进程的事件，级别可以通过EventLevel设置 (一般的应该有默认订阅的方法，或者使用安全调用事件方法) 
        /// </summary>
        event ProcessEventHandle ProcessEvent;

        /// <summary>
        /// 处理初始化进程
        /// </summary>
        void Init();

        /// <summary>
        /// 处理开始/继续进程
        /// </summary>
        void Start();

        /// <summary>
        /// 处理结束/中断进程
        /// </summary>
        void Stop();
    }
}
