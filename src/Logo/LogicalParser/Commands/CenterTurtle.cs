namespace LogicalParser.Commands
{
    public class CenterTurtle : Command
    {
        public override string ToString()
        {
            return string.Format("{0}", Parser.CENTER_TURTLE);
        }
    }
}