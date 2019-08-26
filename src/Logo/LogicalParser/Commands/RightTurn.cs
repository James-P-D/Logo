using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class RightTurn : Command
    {
        public RightTurn(NumberEval numberEval)
        {
            this.NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public float Angle
        {
            get
            {
                return this.NumberEval.Value;
            }
        }

        public override string ToString()
        {
            return $"{Parser.RightTurn} {this.NumberEval.ToString()}";
        }
    }
}