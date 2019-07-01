using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalParser.Commands
{
    public class ShowTurtle : Command
    {
        public ShowTurtle()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}", Parser.SHOW_TURTLE);
        }

    }
}