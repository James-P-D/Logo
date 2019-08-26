using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class Left : Command
    {
        public Left(NumberEval numberEval)
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
            return $"{Parser.Left} {this.NumberEval.ToString()}";
        }
    }
}