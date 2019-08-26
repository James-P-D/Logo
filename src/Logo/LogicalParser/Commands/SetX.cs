﻿using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands
{
    public class SetX : Command
    {
        public SetX(NumberEval numberEval)
        {
            this.numberEval = numberEval;
        }

        private NumberEval numberEval { get; }

        public int X
        {
            get
            {
                return (int)this.numberEval.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.SET_X, this.numberEval.ToString());
        }

    }
}