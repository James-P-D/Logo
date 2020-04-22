namespace LogicalParser.Commands
{
  public class Output : Command
  {
    public Output(Eval eval)
    {
      Eval = eval;
    }

    private Eval Eval { get; }

    public string Name => Eval.ToString();

    public Eval Value => Eval;

    public override string ToString()
    {
      return $"{Parser.Output} {Name}";
    }
  }
}