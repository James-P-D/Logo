using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class SetColorB : Command
    {
        public SetColorB(NumberEval numberEval)
        {
            this.NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public int B
        {
            get
            {
                return (int)this.NumberEval.Value;
            }
        }

        public override string ToString()
        {
            return $"{Parser.ColorB} {this.NumberEval.ToString()}";
        }

    }
}