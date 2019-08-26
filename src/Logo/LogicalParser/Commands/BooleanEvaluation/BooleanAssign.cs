using LogicalParser.Objects.Boolean;

namespace LogicalParser.Commands.BooleanEvaluation
{
    public class BooleanAssign : Command
    {
        public BooleanAssign(BooleanVariable variable, BooleanEval booleanEval)
        {
            this.BooleanVar = variable;
            this.BooleanEval = booleanEval;
        }

        public BooleanVariable BooleanVar { get; set; }
        public BooleanEval BooleanEval { get; set; }

        public override string ToString()
        {
            return $"{Parser.Boolean} {this.BooleanVar.Name} = {this.BooleanEval.ToString()}";
        }
    }
}