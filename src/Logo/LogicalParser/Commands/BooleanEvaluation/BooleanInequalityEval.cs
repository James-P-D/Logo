namespace LogicalParser.Commands.BooleanEvaluation
{
    public class BooleanInequalityEval : BooleanEval
    {
        public BooleanInequalityEval(BooleanEval booleanEval1, BooleanEval booleanEval2)
        {
            this.BooleanEval1 = booleanEval1;
            this.BooleanEval2 = booleanEval2;
        }

        public override bool Value
        {
            get
            {
                return this.BooleanEval1.Value == this.BooleanEval2.Value;
            }
        }

        public BooleanEval BooleanEval1 { get; }
        public BooleanEval BooleanEval2 { get; }

        public override string ToString()
        {
            return $"({BooleanEval1.ToString()} != {BooleanEval2.ToString()})";
        }
    }
}