using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Objects;

namespace LogicalParser.Commands.Evaluation
{
    public class NumberValueEval : NumberEval
    {
        public NumberValueEval(NumberObject numberObject)
        {
            this.numberObject = numberObject;
        }

        private NumberObject numberObject { get; set; }

        public override float Value
        {
            get { return numberObject.Value; }
        }

        public override string ToString()
        {
            return string.Format("{0}", this.numberObject.ToString());
        }
    }
}