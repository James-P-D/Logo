﻿using LogicalParser.Commands.NumberEvaluation;

namespace LogicalParser.Commands
{
  public class SetColorA : Command
  {
    public SetColorA(NumberEval numberEval)
    {
      NumberEval = numberEval;
    }

    private NumberEval NumberEval { get; }

    public int A => (int) NumberEval.Value;

    public override string ToString()
    {
      return $"{Parser.SetColorA} {NumberEval}";
    }
  }
}