using LogicalParser.Objects.Number;

namespace LogicalParser.Commands.NumberEvaluation
{
  public class NumberAssign : Command
  {
    public NumberAssign(NumberVariable variable, NumberEval numberEval)
    {
      NumberVar = variable;
      NumberEval = numberEval;
    }

    public NumberVariable NumberVar { get; set; }
    public NumberEval NumberEval { get; set; }

    public override string ToString()
    {
      return $"{Parser.Number} {NumberVar.Name} = {NumberEval}";
    }
  }
}