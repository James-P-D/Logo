using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Objects;
using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class RightTurn : Command
    {
        public RightTurn(NumberEval numberEval)
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
            return string.Format("{0} {1}", Parser.RIGHT_TURN, this.numberEval.ToString());
        }
    }
}