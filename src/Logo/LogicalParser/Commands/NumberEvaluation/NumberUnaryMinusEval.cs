namespace LogicalParser.Commands.NumberEvaluation
{
    public class NumberUnaryMinusEval : NumberEval
    {
        public NumberUnaryMinusEval(NumberEval numberEval)
        {
            this.NumberEval1 = numberEval;
        }

        public override float Value
        {
            get
            {
                return -this.NumberEval1.Value;
            }
        }

        public NumberEval NumberEval1 { get; }

        public override string ToString()
        {
            return $"-{this.NumberEval1}";
        }
    }
}