using LogicalParser.Objects;

namespace LogicalParser.Commands.Evaluation
{
    public class NumberAssign : Command
    {
        public NumberAssign(NumberVariable variable, NumberEval numberEval)
        {
            this.NumberVar = variable;
            this.NumberEval = numberEval;
        }

        public NumberVariable NumberVar { get; set; }
        public NumberEval NumberEval { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} = {2}", Parser.NUMBER, this.NumberVar.Name, this.NumberEval.ToString());
        }
    }
}