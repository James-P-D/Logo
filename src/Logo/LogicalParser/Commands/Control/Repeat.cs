using System.Collections.Generic;
using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands.Control
{
    public class Repeat : Command
    {
        public Repeat(NumberEval numberEval, Command[] commands)
        {
            this.NumberEval = numberEval;
            this.Commands = new List<Command>(commands);
        }

        private NumberEval NumberEval { get; }
        public List<Command> Commands { get; }

        public int Counter
        {
            get
            {
                return (int)this.NumberEval.Value;
            }
            private set { }
        }

        public override string ToString()
        {
            return $"{Parser.Repeat} {NumberEval.ToString()}";
        }
    }
}