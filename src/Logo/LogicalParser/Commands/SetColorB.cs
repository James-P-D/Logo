using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
    public class SetColorB : Command
    {
        public SetColorB(NumberEval numberEval)
        {
            NumberEval = numberEval;
        }

        private NumberEval NumberEval { get; }

        public int B => (int)NumberEval.Value;

        public override string ToString()
        {
            return $"{Parser.SetColorB} {NumberEval}";
        }

    }
}