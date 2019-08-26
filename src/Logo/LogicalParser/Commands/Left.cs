using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class Left : Command
    {
        public Left(NumberEval numberEval)
        {
            this.numberEval = numberEval;
        }

        private NumberEval numberEval { get; set; }

        public float Distance
        {
            get
            {
                return this.numberEval.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.LEFT, this.numberEval.ToString());
        }
    }
}