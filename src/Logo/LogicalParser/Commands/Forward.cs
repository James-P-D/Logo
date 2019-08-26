using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class Forward : Command
    {
        public Forward(NumberEval numberEval)
        {
            NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public float Distance => NumberEval.Value;

        public override string ToString()
        {
            return $"{Parser.Forward} {NumberEval}";
        }
    }
}