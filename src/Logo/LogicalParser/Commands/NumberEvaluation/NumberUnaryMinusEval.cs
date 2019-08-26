namespace LogicalParser.Commands.Evaluation
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

        public NumberEval NumberEval1 { get; private set; }

        public override string ToString()
        {
            return string.Format("-{0}", this.NumberEval1);
        }
    }
}