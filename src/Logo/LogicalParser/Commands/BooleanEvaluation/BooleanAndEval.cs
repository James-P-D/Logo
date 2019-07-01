using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class BooleanAndEval : BooleanEval
    {
        public BooleanAndEval(BooleanEval booleanEval1, BooleanEval booleanEval2)
        {
            this.BooleanEval1 = booleanEval1;
            this.BooleanEval2 = booleanEval2;
        }

        public override bool Value
        {
            get
            {
                return this.BooleanEval1.Value && this.BooleanEval2.Value;
            }
        }

        public BooleanEval BooleanEval1 { get; private set; }
        public BooleanEval BooleanEval2 { get; private set; }

        public override string ToString()
        {
            return string.Format("({0} && {1})", BooleanEval1.ToString(), BooleanEval2.ToString());
        }
    }
}