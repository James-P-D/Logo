namespace LogicalParser.Commands
{
    public class PenUp : Command
    {
        public override string ToString()
        {
            return string.Format("{0}", Parser.PEN_UP);
        }

    }
}