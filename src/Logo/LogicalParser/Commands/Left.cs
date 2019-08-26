using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class Left : Command
    {
        public Left(NumberEval numberEval)
        {
            NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public float Distance
        {
            get
            {
                return NumberEval.Value;
            }
        }

        public override string ToString()
        {
            return $"{Parser.Left} {NumberEval}";
        }
    }
}