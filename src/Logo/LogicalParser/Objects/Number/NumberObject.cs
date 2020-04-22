namespace LogicalParser.Objects.Number
{
  public class NumberObject : LogoObject
  {
    public NumberObject(float val)
    {
      Value = val;
    }

    public NumberObject(NumberLiteral numberConstValue)
    {
      Value = numberConstValue.Value;
    }

    public float Value { get; set; }
  }
}