using System;

namespace LogicalParser.Commands.NumberEvaluation
{
    public class NumberExponentialEval : NumberEval
    {
        public NumberExponentialEval(NumberEval numberEval1, NumberEval numberEval2)
        {
            NumberEval1 = numberEval1;
            NumberEval2 = numberEval2;
        }

        public override float Value
        {
            get
            {
                throw new NotImplementedException();
                //return this.numberEval1.Value ^ this.numberEval2.Value;
            }
        }

        public NumberEval NumberEval1 { get; }
        public NumberEval NumberEval2 { get; }

        public override string ToString()
        {
            return $"({NumberEval1} ^ {NumberEval2})";
        }
    }
}