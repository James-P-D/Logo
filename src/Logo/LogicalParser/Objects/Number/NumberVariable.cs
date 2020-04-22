namespace LogicalParser.Objects.Number
{
  public class NumberVariable : NumberObject
  {
    public NumberVariable(string name) :
      base(0)
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