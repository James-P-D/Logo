namespace LogicalParser.Commands.BooleanEvaluation
{
    public class BooleanUnaryNotEval : BooleanEval
    {
        public BooleanUnaryNotEval(BooleanEval booleanEval)
        {
            BooleanEval = booleanEval;
        }

        public override bool Value => !BooleanEval.Value;

        public BooleanEval BooleanEval { get; }

        public override string ToString()
        {
            return $"!{BooleanEval}";
        }
    }
}