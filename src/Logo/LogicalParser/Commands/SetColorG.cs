using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class SetColorG : Command
    {
        public SetColorG(NumberEval numberEval)
        {
            this.numberEval = numberEval;
        }

        private NumberEval numberEval { get; }

        public int G
        {
            get
            {
                return (int)this.numberEval.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.COLOR_G, this.numberEval.ToString());
        }
    }
}