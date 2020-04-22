using System;

namespace LogicalParser.Commands.NumberEvaluation
{
  public class NumberUnaryTanEval : NumberEval
  {
    public NumberUnaryTanEval(NumberEval numberEval)
    {
      NumberEval1 = numberEval;
    }

    public override float Value => (float) Math.Tan(NumberEval1.Value * (Math.PI / 180));

    public NumberEval NumberEval1 { get; }

    public override string ToString()
    {
      return $"Tan {NumberEval1}";
    }
  }
}