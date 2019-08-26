namespace LogicalParser.Commands.BooleanEvaluation
{
    public class BooleanAndEval : BooleanEval
    {
        public BooleanAndEval(BooleanEval booleanEval1, BooleanEval booleanEval2)
        {
            BooleanEval1 = booleanEval1;
            BooleanEval2 = booleanEval2;
        }

        public override bool Value
        {
            get
            {
                return BooleanEval1.Value && BooleanEval2.Value;
            }
        }

        public BooleanEval BooleanEval1 { get; }
        public BooleanEval BooleanEval2 { get; }

        public override string ToString()
        {
            return $"({BooleanEval1} && {BooleanEval2})";
        }
    }
}