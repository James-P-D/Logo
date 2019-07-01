using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalParser.Objects
{
    public class BooleanObject : LogoObject
    {
        public BooleanObject(bool val)
        {
            this.Value = val;
        }

        public BooleanObject(BooleanLiteral booleanLiteralValue)
        {
            this.Value = booleanLiteralValue.Value;
        }

        public bool Value { get; set; }
    }
}