namespace LogicalParser.Objects.Boolean
{
  public class BooleanObject : LogoObject
  {
    public BooleanObject(bool val)
    {
      Value = val;
    }

    public BooleanObject(BooleanLiteral booleanLiteralValue)
    {
      Value = booleanLiteralValue.Value;
    }

    public bool Value { get; set; }
  }
}