using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class SetColorB : Command
    {
        public SetColorB(NumberEval numberEval)
        {
            this.numberEval = numberEval;
        }

        private NumberEval numberEval { get; }

        public int B
        {
            get
            {
                return (int)this.numberEval.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.COLOR_B, this.numberEval.ToString());
        }

    }
}