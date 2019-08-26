namespace LogicalParser.Commands.Control
{
    public class Continue : Command
    {
        public override string ToString()
        {
            return string.Format("{0}", Parser.CONTINUE);
        }
    }
}