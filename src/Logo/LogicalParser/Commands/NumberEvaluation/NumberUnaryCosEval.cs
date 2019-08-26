using System;

namespace LogicalParser.Commands.Evaluation
{
    public class NumberUnaryCosEval : NumberEval
    {
        public NumberUnaryCosEval(NumberEval numberEval)
        {
            this.NumberEval1 = numberEval;
        }

        public override float Value
        {
            get
            {
                return (float)Math.Cos(this.NumberEval1.Value * (Math.PI / 180));
            }
        }

        public NumberEval NumberEval1 { get; private set; }

        public override string ToString()
        {
            return string.Format("Cos {0}", this.NumberEval1);
        }
    }
}