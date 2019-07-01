using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalParser.Commands
{
    public class PenDown : Command
    {
        public PenDown()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}", Parser.PEN_DOWN);
        }

    }
}