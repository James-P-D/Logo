using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class SetColorA : Command
    {
        public SetColorA(NumberEval numberEval)
        {
            this.numberEval = numberEval;
        }

        private NumberEval numberEval { get; }

        public int A
        {
            get
            {
                return (int)this.numberEval.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.COLOR_A, this.numberEval.ToString());
        }
    }
}