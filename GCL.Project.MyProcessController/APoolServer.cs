using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GCL.IO.Log;
using GCL.Event;
using GCL.Common;
using GCL.Threading;
using GCL.Threading.Process;

namespace GCL.Project.MyProcessController {
    /// <summary>
    /// 池控制多线程插件类，继承基本插件类
    /// 只需要节点内设置设置属性，即可以使用池控制的多线程管理，实现Create,Custom,RollBack,OnClose方法实现数据产生与处理，回滚与消除等逻辑
    /// 不需要处理init(),start(),stop()等操作，不需要注意CallProcessEvent传出状态
    /// </summary>
    public abstract class APoolServer : ABusinessServer {

        public bool WaitCustomerClose {
            get { return this.poolProcess.WaitCustomerClose; }
            set { this.poolProcess.WaitCustomerClose = value; }
        }

        protected CCPoolProcess poolProcess;

        /// <summary>
        /// 构造函数 请注意这里只对消费者线程进行池管理。
        /// </summary>
        /// <param name="createrNum">生产者最大值</param>
        /// <param name="createrWaittime">生产数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="customerNum">消费数据者线程数</param>
        /// <param name="customerWaittime">消费数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        public APoolServer(int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime) {
            poolProcess = new CCPoolProcess(new CreaterAction(this.Create), new CustomerAction(this.Custom), new CustomerAction(this.RollBack),
                createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime);
            poolProcess.ProcessEvent += new ProcessEventHandle(poolProcess_ProcessEvent);
            this.CanDisposeByReady = poolProcess.CanDisposeByReady;

#if(DEBUG)
            this.WaitCustomerClose = false;
#endif
        }

        /// <summary>
        /// 构造函数 请注意这里只对消费者线程进行池管理。
        /// 这个构造方法允许生产数据时有等待时间，也就是说当待处理数据并不为空时也需要等待的时间
        /// </summary>
        /// <param name="createrNum">生产者最大值</param>
        /// <param name="createrWaittime">生产数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="customerNum">消费数据者线程数</param>
        /// <param name="customerWaittime">消费数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        /// <param name="createWaitTime">生产数据者线程生产数据时等待时间（单位 毫秒）</param>
        public APoolServer(int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime, int createWaitTime)
            : this(createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime) {
            if (createWaitTime > 0)
                this.poolProcess.CreateWaitTime = createWaitTime;
        }

        protected virtual void poolProcess_ProcessEvent(object sender, ProcessEventArg e) {
            if (isDispose && e.GetState() == ProcessState.STOP) {
                try {
                    OnClose(sender, e);
                } catch {
                }
            }
            this.CallProcessEventSimple(e);
        }

        protected virtual void OnClose(object sender, EventArg e) { }
        protected abstract object Create(object sender, EventArg e);
        protected abstract void Custom(object sender, EventArg e, object value);
        protected abstract void RollBack(object sender, EventArg e, object value);

        public override void Init() {
            poolProcess.Init();
        }

        public override void Start() {
            poolProcess.Start();
        }

        public override void Stop() {
            if (!isDispose)
                poolProcess.Stop();
            else
                poolProcess.Dispose();
        }

        protected override bool CheckStop() {
            return poolProcess.GetState() == ProcessState.STOP || poolProcess.GetState() == ProcessState.DISPOSE;
        }
    }
}
