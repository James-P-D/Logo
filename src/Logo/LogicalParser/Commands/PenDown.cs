namespace LogicalParser.Commands
{
    public class PenDown : Command
    {
        public PenDown()
        {
        }

        public override string ToString()
        {
            return string.Format("{0}", Parser.PEN_DOWN);
        }

    }
}