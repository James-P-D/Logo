using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Objects;
using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class SetDirection : Command
    {
        public SetDirection(NumberEval numberEval)
        {
            this.numberEval = numberEval;
        }

        private NumberEval numberEval { get; set; }

        public float Direction
        {
            get
            {
                return this.numberEval.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.SET_DIRECTION, this.numberEval.ToString());
        }

    }
}