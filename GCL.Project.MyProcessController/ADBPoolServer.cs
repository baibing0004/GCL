using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using GCL;
using GCL.Db;
using GCL.IO.Log;
namespace GCL.Project.MyProcessController {
    /// <summary>
    /// DBPool业务类，其Creater设置应该大于1.
    /// IDictionary idic = value as IDictionary;
    /// </summary>
    public abstract class ADBPoolServer : APoolServer {


        private int minCapacity = 50;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minCapacity">数据队列最小容量，当数据队列小于此容量时，生产者自动添加新目录下文件</param>
        /// <param name="createrNum">生产者最大值</param>
        /// <param name="createrWaittime">生产数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="customerNum">消费数据者线程数</param>
        /// <param name="customerWaittime">消费数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        public ADBPoolServer(int minCapacity, int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime)
            : base(createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime) {
            this.minCapacity = minCapacity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minCapacity">数据队列最小容量，当数据队列小于此容量时，生产者自动添加新目录下文件</param>
        /// <param name="createrNum">生产者最大值</param>
        /// <param name="createrWaittime">生产数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="customerNum">消费数据者线程数</param>
        /// <param name="customerWaittime">消费数据者线程无数据时等待时间（单位 毫秒）</param>
        /// <param name="refreshWaittime">消费数据者线程过期时间（单位 毫秒）</param>
        /// <param name="createWaitTime">生产数据者线程生产数据时等待时间（单位 毫秒）</param>
        public ADBPoolServer(int minCapacity, int createrNum, int createrWaittime, int customerNum, int customerWaittime, int refreshWaittime, int createWaitTime)
            : base(createrNum, createrWaittime, customerNum, customerWaittime, refreshWaittime, createWaitTime)
        {
            this.minCapacity = minCapacity;
        }

        /// <summary>
        /// isOver是否已经没有数据等待结束，是否正在有人读取信息
        /// </summary>
        private bool isSelect = false;
        private DateTime passDate = new DateTime();
        private object key = DateTime.Now;
        /// <summary>
        /// 是否正在有人读取信息
        /// </summary>
        public bool IsSelect {
            get { return isSelect; }
            set {
                lock (key) {
                    if (isSelect && isSelect == value)
                        throw new InvalidOperationException("值已经设定");
                    isSelect = value;
                }
            }
        }

        private DataTable dt = new DataTable();
        protected int GetDataCount() {
            return dt.Rows.Count;
        }

        protected override object Create(object sender, GCL.Event.EventArg e) {

            #region 需要填充DataTable
            if (dt.Rows.Count < minCapacity)
                try {
                    this.IsSelect = true;
                    //大于过期时间
                    if (DateTime.Now.CompareTo(passDate) > 0)
                        try {
                            DataTable _dt = GetData();
                            if (_dt == null || _dt.Rows.Count == 0) {
                                passDate = DateTime.Now.AddMilliseconds(this.poolProcess.PoolRefresher.WaitTime);
                                this.CallProcessEventSimple(LogType.RELEASE, 212, "数据库信息已经取完!");
                            } else
                                lock (dt) {
                                    dt.Merge(_dt);
                                }
                            _dt = null;
                        } catch (System.Data.Common.DbException ex) {
                            this.CallProcessEventSimple(LogType.RELEASE, 211, new object[] { "创建信息时发生错误:" + ex.ToString(), ex });
                        }
                } catch (InvalidOperationException) {
                } finally {
                    this.IsSelect = false;
                }
            #endregion

            lock (dt) {
                if (dt.Rows.Count > 0) {
                    DataRow row = dt.Rows[0];
                    NameValueCollection col = ToCollection(row);
                    row.Delete();
                    row.AcceptChanges();
                    //for (int w = 0; w < col.Count; w++)
                    //    this.CallProcessEventSimple(303, string.Format("{0}:{1}", col.Keys[w].ToString(), col.GetValues(w)[0]));
                    return col;
                } else
                    throw new Exception("没有值");
            }
        }

        protected abstract DataTable GetData();
        private NameValueCollection ToCollection(DataRow row) {
            NameValueCollection value = new NameValueCollection();
            foreach (DataColumn column in row.Table.Columns)
                value[column.ColumnName] = Convert.ToString(row[column]);
            return value;
        }

        protected override void OnClose(object sender, GCL.Event.EventArg e) {
            foreach (DataRow row in dt.Rows) {
                RollBack(sender, e, ToCollection(row));
            }
            dt.Clear();
        }
    }
}
