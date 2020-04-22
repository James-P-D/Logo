using System;
using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands.BooleanEvaluation
{
  public class BooleanNumberInequalityEval : BooleanEval
  {
    private double TOLERANCE = 0.0001;

    public BooleanNumberInequalityEval(NumberEval numberEval1, NumberEval numberEval2)
    {
      NumberEval1 = numberEval1;
      NumberEval2 = numberEval2;
    }

    public override bool Value => Math.Abs(NumberEval1.Value - NumberEval2.Value) > TOLERANCE;

    public NumberEval NumberEval1 { get; }
    public NumberEval NumberEval2 { get; }

    public override string ToString()
    {
      return $"({NumberEval1} != {NumberEval2})";
    }
  }
}