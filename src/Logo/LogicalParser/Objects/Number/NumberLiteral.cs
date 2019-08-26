namespace LogicalParser.Objects
{
    public class NumberLiteral : NumberObject
    {
        public NumberLiteral(float val)
          : base(val)
        {

        }

        public override string ToString()
        {
            return string.Format("{0}", base.Value);
        }
    }
}