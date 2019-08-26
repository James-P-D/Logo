namespace LogicalParser.Commands
{
    public class Output : Command
    {
        public Output(Eval eval)
        {
            this.Eval = eval;
        }

        private Eval Eval { get; }

        public string Name
        {
            get
            {
                return this.Eval.ToString();
            }
        }

        public Eval Value
        {
            get
            {
                return this.Eval;
            }
        }

        public override string ToString()
        {
            return $"{Parser.Output} {this.Name}";
        }
    }
}