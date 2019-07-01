using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalParser.Commands
{
    public class CenterTurtle : Command
    {
        public CenterTurtle()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}", Parser.CENTER_TURTLE);
        }
    }
}