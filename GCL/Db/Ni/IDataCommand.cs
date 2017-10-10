using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
namespace GCL.Db.Ni {
    public interface IDataCommand {
        /// <summary>
        /// 用于执行各种类型的命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="result"></param>
        void ExcuteCommand(IDataResource res, IDbCommand command,NiDataResult result);
    }
}
