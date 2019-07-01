using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalParser.Commands.Evaluation
{
    public class NumberUnaryPlusEval : NumberEval
    {
        public NumberUnaryPlusEval(NumberEval numberEval)
        {
            this.NumberEval1 = numberEval;
        }

        public override float Value
        {
            get
            {
                return this.NumberEval1.Value;
            }
        }

        public NumberEval NumberEval1 { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}", this.NumberEval1);
        }
    }
}