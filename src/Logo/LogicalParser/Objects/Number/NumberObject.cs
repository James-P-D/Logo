namespace LogicalParser.Objects.Number
{
    public class NumberObject : LogoObject
    {
        public NumberObject(float val)
        {
            this.Value = val;
        }

        public NumberObject(NumberLiteral numberConstValue)
        {
            this.Value = numberConstValue.Value;
        }

        public float Value { get; set; }
    }
}