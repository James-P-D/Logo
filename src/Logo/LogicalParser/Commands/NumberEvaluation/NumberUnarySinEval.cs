using System;

namespace LogicalParser.Commands.Evaluation
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

        public NumberEval NumberEval1 { get; private set; }

        public override string ToString()
        {
            return string.Format("Sin {0}", this.NumberEval1);
        }
    }
}