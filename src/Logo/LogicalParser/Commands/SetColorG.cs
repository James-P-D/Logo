using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class SetColorG : Command
    {
        public SetColorG(NumberEval numberEval)
        {
            NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public int G => (int)NumberEval.Value;

        public override string ToString()
        {
            return $"{Parser.ColorG} {NumberEval}";
        }
    }
}