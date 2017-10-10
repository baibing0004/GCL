using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
namespace GCL.Db.Ni {
    public class ParameterCommand {
        private string commandText;

        public string CommandText {
            get { return commandText; }
        }
        private CommandType commandType;

        public CommandType CommandType {
            get { return commandType; }
        }
        private ParameterEntity[] paras;

        public ParameterEntity[] Parameters {
            get { return paras; }
            set { paras = value; }
        }

        public string Template { get; protected set; }

        public ParameterCommand(string commandText, string template, CommandType commandType, ParameterEntity[] paras) {
            this.commandText = commandText;
            this.Template = template;
            this.commandType = commandType;
            this.paras = paras;
        }
    }
}
