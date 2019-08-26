using System;

namespace LogicalParser.Commands.NumberEvaluation
{
    public class NumberUnaryCosEval : NumberEval
    {
        public NumberUnaryCosEval(NumberEval numberEval)
        {
            NumberEval1 = numberEval;
        }

        public override float Value => (float)Math.Cos(NumberEval1.Value * (Math.PI / 180));

        public NumberEval NumberEval1 { get; }

        public override string ToString()
        {
            return $"Cos {NumberEval1}";
        }
    }
}