namespace LogicalParser.Commands.NumberEvaluation
{
    public class NumberMultiplyEval : NumberEval
    {
        public NumberMultiplyEval(NumberEval numberEval1, NumberEval numberEval2)
        {
            this.NumberEval1 = numberEval1;
            this.NumberEval2 = numberEval2;
        }

        public override float Value
        {
            get
            {
                return this.NumberEval1.Value * this.NumberEval2.Value;
            }
        }

        public NumberEval NumberEval1 { get; }
        public NumberEval NumberEval2 { get; }

        public override string ToString()
        {
            return $"({NumberEval1.ToString()} * {NumberEval2.ToString()})";
        }
    }
}