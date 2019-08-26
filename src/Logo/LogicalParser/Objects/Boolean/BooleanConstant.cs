namespace LogicalParser.Objects
{
    public class BooleanConstant : BooleanObject
    {
        public BooleanConstant(string name, bool val) :
          base(val)
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