using System;
using System.Collections.Generic;
using System.Text;

namespace GCL.Bean.Transaction {

    /// <summary>
    /// 用于事务处理
    /// </summary>
    public class Transaction : IDisposable {
        private IList<Command> commands = new List<Command>();



        ~Transaction() {
            Dispose();
        }

        /// <summary>
        /// 有关对象
        /// </summary>
        public IList<Command> Commands {
            get { return commands; }
        }

        /// <summary>
        /// 请注意如果需要前面执行的Command结果作为后面Command的参数，请加入invokeParas\rollbackParas中，不会二次调用，而且默认的Rollback方法第一个参数是method方法的结果
        /// </summary>
        /// <param name="o"></param>
        /// <param name="method"></param>
        /// <param name="invokeParas"></param>
        /// <param name="order"></param>
        /// <param name="rollbackMethod"></param>
        public void AddCommand(object o, string method, object[] invokeParas, InvokeOrder order, string rollbackMethod) {
            commands.Add(new Command(o, method, invokeParas, order, rollbackMethod));
        }


        /// <summary>
        /// 请注意如果需要前面执行的Command结果作为后面Command的参数，请加入invokeParas\rollbackParas中，不会二次调用，而且默认的Rollback方法第一个参数是method方法的结果
        /// </summary>
        /// <param name="o"></param>
        /// <param name="method"></param>
        /// <param name="invokeParas"></param>
        /// <param name="order"></param>
        public void AddCommand(object o, string method, object[] invokeParas, InvokeOrder order) {
            commands.Add(new Command(o, method, invokeParas, order));
        }

        /// <summary>
        /// 请注意如果需要前面执行的Command结果作为后面Command的参数，请加入invokeParas\rollbackParas中，不会二次调用，而且默认的Rollback方法第一个参数是method方法的结果
        /// </summary>
        /// <param name="o"></param>
        /// <param name="method"></param>
        /// <param name="order"></param>
        /// <param name="rollbackMethod"></param>
        public void AddCommand(object o, string method, InvokeOrder order, string rollbackMethod) {
            commands.Add(new Command(o, method, order, rollbackMethod));
        }


        public void Submit() {
            foreach (Command com in commands)
                try {
                    com.Action();
                } catch (Exception) {
                    RollBack();
                    throw;
                }
        }

        public void RollBack() {
            for (int w = commands.Count - 1; w >= 0; w--)
                try {
                    commands[w].RollBack();
                } catch {
                }
        }

        #region IDisposable Members

        public void Dispose() {
            if (commands != null)
                commands.Clear();
            commands = null;
        }

        #endregion
    }
}
