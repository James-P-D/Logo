using System.Collections.Generic;
using LogicalParser.Commands.BooleanEvaluation;

namespace LogicalParser.Commands.Control
{
    public class While : Command
    {
        public While(BooleanEval numberEval, Command[] commands)
        {
            this.BooleanEval = numberEval;
            this.Commands = new List<Command>(commands);
        }

        private BooleanEval BooleanEval { get; }
        public List<Command> Commands { get; }

        public bool Value
        {
            get
            {
                return BooleanEval.Value;
            }
            private set { }
        }

        public override string ToString()
        {
            return $"{Parser.While} {BooleanEval.ToString()}";
        }
    }

}