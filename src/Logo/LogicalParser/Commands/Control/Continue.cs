using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalParser.Commands.Control
{
    public class Continue : Command
    {
        public Continue()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}", Parser.CONTINUE);
        }
    }
}