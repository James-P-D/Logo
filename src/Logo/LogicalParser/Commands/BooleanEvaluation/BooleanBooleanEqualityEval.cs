using System;
using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands.BooleanEvaluation
{
    public class BooleanNumberEqualityEval : BooleanEval
    {
        public BooleanNumberEqualityEval(NumberEval numberEval1, NumberEval numberEval2)
        {
            NumberEval1 = numberEval1;
            NumberEval2 = numberEval2;
        }

        public override bool Value
        {
            get
            {
                var TOLERANCE = 0.001;
                return Math.Abs(NumberEval1.Value - NumberEval2.Value) < TOLERANCE;
            }
        }

        public NumberEval NumberEval1 { get; }
        public NumberEval NumberEval2 { get; }

        public override string ToString()
        {
            return $"({NumberEval1} == {NumberEval2})";
        }
    }
}