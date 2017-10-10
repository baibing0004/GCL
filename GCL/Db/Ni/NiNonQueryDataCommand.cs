using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
namespace GCL.Db.Ni {
    public class NiNonQueryDataCommand : NiQueryDataCommand {
        #region IDataCommand Members

        public override void ExcuteCommand(IDataResource res, IDbCommand command, NiDataResult result) {
            //为ObjectDb特别处理
            if (command is INeedNiDataResult) {
                ((INeedNiDataResult)command).DataResult = result;
            }

            int ret = command.ExecuteNonQuery();
            FillDataTable(result.DataSet, command, "NonQuery", ret);
        }

        #endregion
    }
}
