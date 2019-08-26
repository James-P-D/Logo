using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class SetY : Command
    {
        public SetY(NumberEval numberEval)
        {
            this.NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public int Y
        {
            get
            {
                return (int)this.NumberEval.Value;
            }
        }

        public override string ToString()
        {
            return $"{Parser.SetY} {this.NumberEval.ToString()}";
        }

    }
}