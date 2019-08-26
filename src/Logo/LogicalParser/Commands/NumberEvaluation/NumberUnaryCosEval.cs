using System;

namespace LogicalParser.Commands.NumberEvaluation
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

        public NumberEval NumberEval1 { get; }

        public override string ToString()
        {
            return $"Cos {this.NumberEval1}";
        }
    }
}