using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class BooleanNumberGreaterThanEval : BooleanEval
    {
        public BooleanNumberGreaterThanEval(NumberEval numberEval1, NumberEval numberEval2)
        {
            this.NumberEval1 = numberEval1;
            this.NumberEval2 = numberEval2;
        }

        public override bool Value
        {
            get
            {
                return this.NumberEval1.Value > this.NumberEval2.Value;
            }
        }

        public NumberEval NumberEval1 { get; private set; }
        public NumberEval NumberEval2 { get; private set; }

        public override string ToString()
        {
            return string.Format("({0} > {1})", this.NumberEval1.ToString(), this.NumberEval2.ToString());
        }
    }
}