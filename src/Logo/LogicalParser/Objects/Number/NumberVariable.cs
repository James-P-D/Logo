using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Objects
{
    public class NumberVariable : NumberObject
    {
        public NumberVariable(string name) :
          base(0)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}", this.Name);
        }
    }
}