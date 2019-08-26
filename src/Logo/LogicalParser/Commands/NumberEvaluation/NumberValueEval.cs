using LogicalParser.Objects.Number;

namespace LogicalParser.Commands.NumberEvaluation
{
    public class NumberValueEval : NumberEval
    {
        public NumberValueEval(NumberObject numberObject)
        {
            NumberObject = numberObject;
        }

        private NumberObject NumberObject { get; }

        public override float Value => NumberObject.Value;

        public override string ToString()
        {
            return $"{NumberObject}";
        }
    }
}