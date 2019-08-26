using System;

namespace LogicalParser.Commands.NumberEvaluation
{
    public class NumberUnarySinEval : NumberEval
    {
        public NumberUnarySinEval(NumberEval numberEval)
        {
            NumberEval1 = numberEval;
        }

        public override float Value
        {
            get
            {
                return (float)Math.Sin(NumberEval1.Value * ((float)Math.PI / 180));
            }
        }

        public NumberEval NumberEval1 { get; }

        public override string ToString()
        {
            return $"Sin {NumberEval1}";
        }
    }
}