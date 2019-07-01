using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalParser.Objects
{
    public class NumberObject : LogoObject
    {
        public NumberObject(float val)
        {
            this.Value = val;
        }

        public NumberObject(NumberLiteral numberConstValue)
        {
            this.Value = numberConstValue.Value;
        }

        public float Value { get; set; }
    }
}