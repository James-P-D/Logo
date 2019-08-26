namespace LogicalParser.Commands.BooleanEvaluation
{
    public class BooleanUnaryNotEval : BooleanEval
    {
        public BooleanUnaryNotEval(BooleanEval booleanEval)
        {
            BooleanEval = booleanEval;
        }

        public override bool Value
        {
            get
            {
                return !BooleanEval.Value;
            }
        }

        public BooleanEval BooleanEval { get; }

        public override string ToString()
        {
            return $"!{BooleanEval}";
        }
    }
}