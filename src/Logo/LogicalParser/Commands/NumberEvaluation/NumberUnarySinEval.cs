using System;

namespace LogicalParser.Commands.NumberEvaluation
{
    public class NumberUnarySinEval : NumberEval
    {
        public NumberUnarySinEval(NumberEval numberEval)
        {
            this.NumberEval1 = numberEval;
        }

        public override float Value
        {
            get
            {
                return (float)Math.Sin(this.NumberEval1.Value * ((float)Math.PI / 180));
            }
        }

        public NumberEval NumberEval1 { get; }

        public override string ToString()
        {
            return $"Sin {this.NumberEval1}";
        }
    }
}