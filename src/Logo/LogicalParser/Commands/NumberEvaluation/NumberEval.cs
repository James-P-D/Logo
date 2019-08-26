namespace LogicalParser.Commands.Evaluation
{
    public abstract class NumberEval : Eval
    {
        public NumberEval()
        {
        }

        public abstract float Value
        {
            get;
        }
    }
}