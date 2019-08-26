using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class SetY : Command
    {
        public SetY(NumberEval numberEval)
        {
            this.numberEval = numberEval;
        }

        private NumberEval numberEval { get; set; }

        public int Y
        {
            get
            {
                return (int)this.numberEval.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.SET_Y, this.numberEval.ToString());
        }

    }
}