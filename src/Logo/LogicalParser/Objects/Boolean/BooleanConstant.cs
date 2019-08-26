namespace LogicalParser.Objects.Boolean
{
    public class BooleanConstant : BooleanObject
    {
        public BooleanConstant(string name, bool val) :
          base(val)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}