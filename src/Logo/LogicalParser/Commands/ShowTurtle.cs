namespace LogicalParser.Commands
{
    public class ShowTurtle : Command
    {
        public ShowTurtle()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}", Parser.SHOW_TURTLE);
        }

    }
}