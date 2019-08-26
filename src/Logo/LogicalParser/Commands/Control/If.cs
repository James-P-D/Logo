using System.Collections.Generic;
using LogicalParser.Commands.BooleanEvaluation;

namespace LogicalParser.Commands.Control
{
    public class If : Command
    {
        // We can only set the Then-Commands when creating the If. We can set the Else later
        // but only if it actually exists!
        public If(BooleanEval booleanEval, Command[] thenCommands)
        {
            BooleanEval = booleanEval;
            ThenCommands = new List<Command>(thenCommands);
            ElseCommands = new List<Command>();
        }

        public void SetElseCommands(Command[] elseCommands)
        {
            ElseCommands.AddRange(elseCommands);
        }

        private BooleanEval BooleanEval { get; }
        public List<Command> ThenCommands { get; }
        public List<Command> ElseCommands { get; }

        public bool Value
        {
            get => BooleanEval.Value;
        }

        public override string ToString()
        {
            return $"{Parser.If} {BooleanEval}";
        }
    }
}