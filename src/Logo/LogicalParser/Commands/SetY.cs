using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
  public class SetY : Command
  {
    public SetY(NumberEval numberEval)
    {
      NumberEval = numberEval;
    }

    private NumberEval NumberEval { get; }

    public int Y => (int) NumberEval.Value;

    public override string ToString()
    {
      return $"{Parser.SetY} {NumberEval}";
    }
  }
}