namespace LogicalParser.Commands
{
    public class HideTurtle : Command
    {
        public override string ToString()
        {
            return string.Format("{0}", Parser.HIDE_TURTLE);
        }

    }
}