namespace LogicalParser.Commands.Control
{
    public class Break : Command
    {
        public override string ToString()
        {
            return $"{Parser.Break}";
        }
    }
}