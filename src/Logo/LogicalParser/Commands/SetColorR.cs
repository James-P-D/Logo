using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class SetColorR : Command
    {
        public SetColorR(NumberEval numberEval)
        {
            this.numberEval = numberEval;
        }

        private NumberEval numberEval { get; set; }

        public int R
        {
            get
            {
                return (int)this.numberEval.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.COLOR_R, this.numberEval.ToString());
        }
    }
}