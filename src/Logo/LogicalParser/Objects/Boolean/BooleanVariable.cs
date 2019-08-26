namespace LogicalParser.Objects
{
    public class BooleanVariable : BooleanObject
    {
        public BooleanVariable(string name) :
          base(false)
        {
            this.Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return string.Format("{0}", this.Name);
        }
    }
}