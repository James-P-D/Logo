using System;

namespace LogicalParser.Commands.Evaluation
{
    public class NumberDivideEval : NumberEval
    {
        public NumberDivideEval(NumberEval numberEval1, NumberEval numberEval2)
        {
            this.numberEval1 = numberEval1;
            this.numberEval2 = numberEval2;
        }

        public override float Value
        {
            get
            {
                if (this.numberEval2.Value == 0)
                {
                    throw new DivideByZeroException();
                }
                return this.numberEval1.Value / this.numberEval2.Value;
            }
        }

        public NumberEval numberEval1 { get; private set; }
        public NumberEval numberEval2 { get; private set; }

        public override string ToString()
        {
            return string.Format("({0} / {1})", numberEval1.ToString(), numberEval2.ToString());
        }
    }
}