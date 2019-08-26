namespace LogicalParser.Commands
{
    public class HideTurtle : Command
    {
        public HideTurtle()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}", Parser.HIDE_TURTLE);
        }

    }
}