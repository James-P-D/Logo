namespace LogicalParser.Commands.BooleanEvaluation
{
  public abstract class BooleanEval : Eval
  {
    public abstract bool Value { get; }
  }
}