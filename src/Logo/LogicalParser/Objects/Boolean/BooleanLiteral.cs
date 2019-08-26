namespace LogicalParser.Objects.Boolean
{
    public class BooleanLiteral : BooleanObject
    {
        public BooleanLiteral(bool val)
          : base(val)
        {

        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}