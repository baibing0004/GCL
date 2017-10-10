using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
namespace GCL.Db.Ni {
    /// <summary>
    /// 用于填充DataSet
    /// </summary>
    public class NiFillDataCommand : IDataCommand {

        #region IDataCommand Members

        public virtual void ExcuteCommand(IDataResource res, IDbCommand command, NiDataResult result) {
            //为ObjectDb特别处理
            if (command is INeedNiDataResult) {
                ((INeedNiDataResult)command).DataResult = result;
            }

            IDbDataAdapter adapter = res.CreateAdapter();
            adapter.SelectCommand = command;
            try {
                adapter.Fill(result.DataSet);
            } finally {
                adapter.SelectCommand.Connection = null;
                Bean.BeanTool.Close(adapter);
            }
        }

        #endregion
    }
}
