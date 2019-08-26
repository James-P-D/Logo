using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class SetX : Command
    {
        public SetX(NumberEval numberEval)
        {
            this.NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public int X
        {
            get
            {
                return (int)this.NumberEval.Value;
            }
        }

        public override string ToString()
        {
            return $"{Parser.SetX} {this.NumberEval.ToString()}";
        }

    }
}