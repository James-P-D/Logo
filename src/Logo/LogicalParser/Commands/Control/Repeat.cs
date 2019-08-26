using System.Collections.Generic;
using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands.Control
{
    public class Repeat : Command
    {
        public Repeat(NumberEval numberEval, Command[] commands)
        {
            NumberEval = numberEval;
            Commands = new List<Command>(commands);
        }

        private NumberEval NumberEval { get; }
        public List<Command> Commands { get; }

        public int Counter
        {
            get => (int)NumberEval.Value;
        }

        public override string ToString()
        {
            return $"{Parser.Repeat} {NumberEval}";
        }
    }
}