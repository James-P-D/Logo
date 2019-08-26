namespace LogicalParser.Objects.Number
{
    public class NumberLiteral : NumberObject
    {
        public NumberLiteral(float val)
          : base(val)
        {

        }

        public override string ToString()
        {
            return $"{base.Value}";
        }
    }
}