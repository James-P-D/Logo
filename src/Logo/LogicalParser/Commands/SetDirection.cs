using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
  public class SetDirection : Command
  {
    public SetDirection(NumberEval numberEval)
    {
      NumberEval = numberEval;
    }

    private NumberEval NumberEval { get; }

    public float Direction => NumberEval.Value;

    public override string ToString()
    {
      return $"{Parser.SetDirection} {NumberEval}";
    }
  }
}