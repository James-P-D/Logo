namespace LogicalParser.Commands.Evaluation
{
    public abstract class NumberEval : Eval
    {
        public abstract float Value
        {
            get;
        }
    }
}