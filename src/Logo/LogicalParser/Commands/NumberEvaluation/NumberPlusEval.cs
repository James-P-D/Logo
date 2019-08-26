namespace LogicalParser.Commands.Evaluation
{
    public class NumberPlusEval : NumberEval
    {
        public NumberPlusEval(NumberEval numberEval1, NumberEval numberEval2)
        {
            this.NumberEval1 = numberEval1;
            this.NumberEval2 = numberEval2;
        }

        public override float Value
        {
            get
            {
                return this.NumberEval1.Value + this.NumberEval2.Value;
            }
        }

        public NumberEval NumberEval1 { get; private set; }
        public NumberEval NumberEval2 { get; private set; }

        public override string ToString()
        {
            return string.Format("({0} + {1})", NumberEval1.ToString(), NumberEval2.ToString());
        }
    }
}