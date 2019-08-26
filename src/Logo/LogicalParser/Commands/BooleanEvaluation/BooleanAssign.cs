using LogicalParser.Objects;

namespace LogicalParser.Commands.Evaluation
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
            return string.Format("{0} {1} = {2}", Parser.BOOLEAN, this.BooleanVar.Name, this.BooleanEval.ToString());
        }
    }
}