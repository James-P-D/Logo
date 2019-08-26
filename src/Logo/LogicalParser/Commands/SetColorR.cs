using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class SetColorR : Command
    {
        public SetColorR(NumberEval numberEval)
        {
            this.NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public int R
        {
            get
            {
                return (int)this.NumberEval.Value;
            }
        }

        public override string ToString()
        {
            return $"{Parser.ColorR} {this.NumberEval.ToString()}";
        }
    }
}