using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class SetColorG : Command
    {
        public SetColorG(NumberEval numberEval)
        {
            this.NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public int G
        {
            get
            {
                return (int)this.NumberEval.Value;
            }
        }

        public override string ToString()
        {
            return $"{Parser.ColorG} {this.NumberEval.ToString()}";
        }
    }
}