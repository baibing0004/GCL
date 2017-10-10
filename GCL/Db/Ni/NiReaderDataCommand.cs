using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace GCL.Db.Ni {
    public class NiReaderDataCommand : IDataCommand {

        #region IDataCommand Members

        public void ExcuteCommand(IDataResource res, IDbCommand command, NiDataResult result) {
            //为ObjectDb特别处理
            

            result.DoReader(command.ExecuteReader());
        }

        #endregion
    }
}
