using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
  public class SetX : Command
  {
    public SetX(NumberEval numberEval)
    {
      NumberEval = numberEval;
    }

    private NumberEval NumberEval { get; }

    public int X => (int) NumberEval.Value;

    public override string ToString()
    {
      return $"{Parser.SetX} {NumberEval}";
    }
  }
}