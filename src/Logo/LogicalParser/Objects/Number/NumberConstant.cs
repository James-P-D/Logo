using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalParser.Objects
{
    public class NumberConstant : NumberObject
    {
        public NumberConstant(string name, float val) :
          base(val)
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