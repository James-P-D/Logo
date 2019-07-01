using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalParser.Commands
{
    public class HideTurtle : Command
    {
        public HideTurtle()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}", Parser.HIDE_TURTLE);
        }

    }
}