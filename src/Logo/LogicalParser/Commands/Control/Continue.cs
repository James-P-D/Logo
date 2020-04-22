namespace LogicalParser.Commands.Control
{
  public class Continue : Command
  {
    public override string ToString()
    {
      return $"{Parser.Continue}";
    }
  }
}