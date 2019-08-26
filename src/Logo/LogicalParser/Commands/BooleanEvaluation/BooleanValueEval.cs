using LogicalParser.Objects;

namespace LogicalParser.Commands.Evaluation
{
    public class BooleanValueEval : BooleanEval
    {
        public BooleanValueEval(BooleanObject booleanObject)
        {
            this.booleanObject = booleanObject;
        }

        private BooleanObject booleanObject { get; }

        public override bool Value
        {
            get { return booleanObject.Value; }
        }

        public override string ToString()
        {
            return string.Format("{0}", this.booleanObject.ToString());
        }
    }
}