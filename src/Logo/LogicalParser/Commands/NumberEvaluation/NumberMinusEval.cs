namespace LogicalParser.Commands.NumberEvaluation
{
    public class NumberMinusEval : NumberEval
    {
        public NumberMinusEval(NumberEval numberEval1, NumberEval numberEval2)
        {
            NumberEval1 = numberEval1;
            NumberEval2 = numberEval2;
        }

        public override float Value => NumberEval1.Value - NumberEval2.Value;

        public NumberEval NumberEval1 { get; }
        public NumberEval NumberEval2 { get; }

        public override string ToString()
        {
            return $"({NumberEval1} - {NumberEval2})";
        }
    }
}