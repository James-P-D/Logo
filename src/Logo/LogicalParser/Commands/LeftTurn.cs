using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Objects;
using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class LeftTurn : Command
    {
        public LeftTurn(NumberEval numberEval)
        {
            this.numberEval = numberEval;
        }

        private NumberEval numberEval { get; set; }

        public float Angle
        {
            get
            {
                return this.numberEval.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.LEFT_TURN, this.numberEval.ToString());
        }
    }
}