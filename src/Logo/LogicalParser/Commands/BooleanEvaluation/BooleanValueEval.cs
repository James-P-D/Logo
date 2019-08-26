using LogicalParser.Objects.Boolean;

namespace LogicalParser.Commands.BooleanEvaluation
{
    public class BooleanValueEval : BooleanEval
    {
        public BooleanValueEval(BooleanObject booleanObject)
        {
            BooleanObject = booleanObject;
        }

        private BooleanObject BooleanObject { get; }

        public override bool Value
        {
            get { return BooleanObject.Value; }
        }

        public override string ToString()
        {
            return $"{BooleanObject}";
        }
    }
}