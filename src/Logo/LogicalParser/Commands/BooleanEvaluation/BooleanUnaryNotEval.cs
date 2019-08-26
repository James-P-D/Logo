namespace LogicalParser.Commands.BooleanEvaluation
{
    public class BooleanUnaryNotEval : BooleanEval
    {
        public BooleanUnaryNotEval(BooleanEval booleanEval)
        {
            this.BooleanEval = booleanEval;
        }

        public override bool Value
        {
            get
            {
                return !this.BooleanEval.Value;
            }
        }

        public BooleanEval BooleanEval { get; }

        public override string ToString()
        {
            return $"!{this.BooleanEval}";
        }
    }
}