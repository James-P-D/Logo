namespace LogicalParser.Commands.Evaluation
{
    public abstract class BooleanEval : Eval
    {
        public abstract bool Value
        {
            get;
        }
    }
}