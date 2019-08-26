using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class RightTurn : Command
    {
        public RightTurn(NumberEval numberEval)
        {
            NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public float Angle => NumberEval.Value;

        public override string ToString()
        {
            return $"{Parser.RightTurn} {NumberEval}";
        }
    }
}