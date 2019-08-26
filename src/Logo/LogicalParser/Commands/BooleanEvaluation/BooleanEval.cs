namespace LogicalParser.Commands.Evaluation
{
    public abstract class BooleanEval : Eval
    {
        public BooleanEval()
        {
        }

        public abstract bool Value
        {
            get;
        }
    }
}