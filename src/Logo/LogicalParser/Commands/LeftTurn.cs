using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class LeftTurn : Command
    {
        public LeftTurn(NumberEval numberEval)
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
            return $"{Parser.LeftTurn} {this.NumberEval.ToString()}";
        }
    }
}