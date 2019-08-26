using System.Collections.Generic;
using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands.Control
{
    public class While : Command
    {
        public While(BooleanEval numberEval, Command[] commands)
        {
            this.booleanEval = numberEval;
            this.commands = new List<Command>(commands);
        }

        private BooleanEval booleanEval { get; set; }
        public List<Command> commands { get; private set; }

        public bool Value
        {
            get
            {
                return booleanEval.Value;
            }
            private set { }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.WHILE, booleanEval.ToString());
        }
    }

}