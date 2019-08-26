using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class Backward : Command
    {
        public Backward(NumberEval numberEval)
        {
            this.numberEval = numberEval;
        }

        private NumberEval numberEval { get; }

        public float Distance
        {
            get
            {
                return this.numberEval.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.BACKWARD, this.numberEval.ToString());
        }
    }
}