using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Objects;

namespace LogicalParser.Commands.Evaluation
{
    public abstract class NumberEval : Eval
    {
        public NumberEval()
        {
        }

        public abstract float Value
        {
            get;
        }
    }
}