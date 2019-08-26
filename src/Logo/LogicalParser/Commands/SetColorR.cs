using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class SetColorR : Command
    {
        public SetColorR(NumberEval numberEval)
        {
            this.numberEval = numberEval;
        }

        private NumberEval numberEval { get; }

        public int R
        {
            get
            {
                return (int)this.numberEval.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.COLOR_R, this.numberEval.ToString());
        }
    }
}