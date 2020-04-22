namespace LogicalParser.Objects.Boolean
{
  public class BooleanVariable : BooleanObject
  {
    public BooleanVariable(string name) :
      base(false)
    {
      Name = name;
    }

    public string Name { get; }

    public override string ToString()
    {
      return $"{Name}";
    }
  }
}