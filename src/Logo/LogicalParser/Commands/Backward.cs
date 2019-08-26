using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class Backward : Command
    {
        public Backward(NumberEval numberEval)
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
            return $"{Parser.Backward} {this.NumberEval.ToString()}";
        }
    }
}