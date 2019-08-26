namespace LogicalParser.Commands.Control
{
    public class Break : Command
    {
        public Break()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}", Parser.BREAK);
        }
    }
}