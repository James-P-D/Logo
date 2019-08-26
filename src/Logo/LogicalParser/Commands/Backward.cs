using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class Backward : Command
    {
        public Backward(NumberEval numberEval)
        {
            NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public float Distance => NumberEval.Value;

        public override string ToString()
        {
            return $"{Parser.Backward} {NumberEval}";
        }
    }
}