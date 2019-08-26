using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class Forward : Command
    {
        public Forward(NumberEval numberEval)
        {
            this.NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public float Distance
        {
            get
            {
                return this.NumberEval.Value;
            }
        }

        public override string ToString()
        {
            return $"{Parser.Forward} {this.NumberEval.ToString()}";
        }
    }
}