using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalParser.Objects
{
    public class BooleanLiteral : BooleanObject
    {
        public BooleanLiteral(bool val)
          : base(val)
        {

        }

        public override string ToString()
        {
            return string.Format("{0}", base.Value);
        }
    }
}