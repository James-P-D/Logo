using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class SetDirection : Command
    {
        public SetDirection(NumberEval numberEval)
        {
            this.NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public float Direction
        {
            get
            {
                return this.NumberEval.Value;
            }
        }

        public override string ToString()
        {
            return $"{Parser.SetDirection} {this.NumberEval.ToString()}";
        }

    }
}