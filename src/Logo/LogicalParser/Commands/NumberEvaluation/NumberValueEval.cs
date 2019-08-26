using LogicalParser.Objects.Number;

namespace LogicalParser.Commands.NumberEvaluation
{
    public class NumberValueEval : NumberEval
    {
        public NumberValueEval(NumberObject numberObject)
        {
            this.NumberObject = numberObject;
        }

        private NumberObject NumberObject { get; }

        public override float Value
        {
            get { return NumberObject.Value; }
        }

        public override string ToString()
        {
            return $"{this.NumberObject.ToString()}";
        }
    }
}