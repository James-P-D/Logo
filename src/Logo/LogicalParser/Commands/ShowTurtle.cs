﻿namespace LogicalParser.Commands
{
    public class ShowTurtle : Command
    {
        public override string ToString()
        {
            return $"{Parser.SHOW_TURTLE}";
        }

    }
}