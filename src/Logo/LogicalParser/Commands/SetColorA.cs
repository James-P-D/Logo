using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class SetColorA : Command
    {
        public SetColorA(NumberEval numberEval)
        {
            this.NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public int A
        {
            get
            {
                return (int)this.NumberEval.Value;
            }
        }

        public override string ToString()
        {
            return $"{Parser.ColorA} {this.NumberEval.ToString()}";
        }
    }
}