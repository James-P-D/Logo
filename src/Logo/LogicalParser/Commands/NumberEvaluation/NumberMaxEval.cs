using System;

namespace LogicalParser.Commands.NumberEvaluation
{
    public class NumberMaxEval : NumberEval
    {
        public NumberMaxEval(NumberEval numberEval1, NumberEval numberEval2)
        {
            this.NumberEval1 = numberEval1;
            this.NumberEval2 = numberEval2;
        }

        public override float Value
        {
            get
            {
                return Math.Max(this.NumberEval1.Value, this.NumberEval2.Value);
            }
        }

        public NumberEval NumberEval1 { get; }
        public NumberEval NumberEval2 { get; }

        public override string ToString()
        {
            return $"max ({NumberEval1.ToString()} , {NumberEval2.ToString()})";
        }
    }
}