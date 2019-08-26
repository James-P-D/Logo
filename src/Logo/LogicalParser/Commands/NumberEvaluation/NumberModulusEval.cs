using System;

namespace LogicalParser.Commands.NumberEvaluation
{
    public class NumberModulusEval : NumberEval
    {
        public NumberModulusEval(NumberEval numberEval1, NumberEval numberEval2)
        {
            this.NumberEval1 = numberEval1;
            this.NumberEval2 = numberEval2;
        }

        public override float Value
        {
            get
            {
                throw new NotImplementedException();
                return -1;
                //return this.numberEval1.Value % this.numberEval2.Value;
            }
        }

        public NumberEval NumberEval1 { get; }
        public NumberEval NumberEval2 { get; }

        public override string ToString()
        {
            return $"({NumberEval1.ToString()} % {NumberEval2.ToString()})";
        }
    }
}