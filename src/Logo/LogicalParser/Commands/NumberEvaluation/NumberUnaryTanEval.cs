using System;

namespace LogicalParser.Commands.Evaluation
{
    public class NumberUnaryTanEval : NumberEval
    {
        public NumberUnaryTanEval(NumberEval numberEval)
        {
            this.NumberEval1 = numberEval;
        }

        public override float Value
        {
            get
            {
                return (float)Math.Tan(this.NumberEval1.Value * (Math.PI / 180));
            }
        }

        public NumberEval NumberEval1 { get; }

        public override string ToString()
        {
            return string.Format("Tan {0}", this.NumberEval1);
        }
    }
}