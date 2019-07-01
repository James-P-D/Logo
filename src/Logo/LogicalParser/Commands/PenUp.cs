using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicalParser.Commands
{
    public class PenUp : Command
    {
        public PenUp()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}", Parser.PEN_UP);
        }

    }
}