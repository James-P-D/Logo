using System.Collections.Generic;
using LogicalParser.Commands.Evaluation;

namespace LogicalParser.Commands.Control
{
    public class If : Command
    {
        // We can only set the Then-Commands when creating the If. We can set the Else later
        // but only if it actually exists!
        public If(BooleanEval booleanEval, Command[] thenCommands)
        {
            this.booleanEval = booleanEval;
            this.thenCommands = new List<Command>(thenCommands);
            this.elseCommands = new List<Command>();
        }

        public void SetElseCommands(Command[] elseCommands)
        {
            this.elseCommands.AddRange(elseCommands);
        }

        private BooleanEval booleanEval { get; set; }
        public List<Command> thenCommands { get; private set; }
        public List<Command> elseCommands { get; private set; }

        public bool Value
        {
            get
            {
                return booleanEval.Value;
            }
            private set { }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Parser.IF, booleanEval.ToString());
        }
    }
}