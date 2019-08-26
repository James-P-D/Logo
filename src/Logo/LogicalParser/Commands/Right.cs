using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class Right : Command
    {
        public Right(NumberEval numberEval)
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
            return $"{Parser.Right} {this.NumberEval.ToString()}";
        }
    }
}