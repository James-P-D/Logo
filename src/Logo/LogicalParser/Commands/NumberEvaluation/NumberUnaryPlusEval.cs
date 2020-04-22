namespace LogicalParser.Commands.NumberEvaluation
{
  public class NumberUnaryPlusEval : NumberEval
  {
    public NumberUnaryPlusEval(NumberEval numberEval)
    {
      NumberEval1 = numberEval;
    }

    public override float Value => NumberEval1.Value;

    public NumberEval NumberEval1 { get; }

    public override string ToString()
    {
      return $"{NumberEval1}";
    }
  }
}