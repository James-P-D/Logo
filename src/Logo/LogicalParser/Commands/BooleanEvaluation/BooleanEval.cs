using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Objects;

namespace LogicalParser.Commands.Evaluation
{
    public abstract class BooleanEval : Eval
    {
        public BooleanEval()
        {
        }

        public abstract bool Value
        {
            get;
        }
    }
}