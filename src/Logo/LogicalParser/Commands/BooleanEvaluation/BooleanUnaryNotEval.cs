using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class BooleanUnaryNotEval : BooleanEval
    {
        public BooleanUnaryNotEval(BooleanEval booleanEval)
        {
            this.BooleanEval = booleanEval;
        }

        public override bool Value
        {
            get
            {
                return !this.BooleanEval.Value;
            }
        }

        public BooleanEval BooleanEval { get; private set; }

        public override string ToString()
        {
            return string.Format("!{0}", this.BooleanEval);
        }
    }
}