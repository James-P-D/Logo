using System.Collections.Generic;
using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands.Control
{
    public class Repeat : Command
    {
        public Repeat(NumberEval numberEval, Command[] commands)
        {
            this.numberEval = numberEval;
            this.commands = new List<Command>(commands);
        }

        private NumberEval numberEval { get; set; }
        public List<Command> commands { get; private set; }

        public int Counter
        {
            get
            {
                return (int)this.numberEval.Value;
            }
            private set { }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.REPEAT, numberEval.ToString());
        }
    }
}