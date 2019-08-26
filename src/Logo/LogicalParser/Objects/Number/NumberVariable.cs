namespace LogicalParser.Objects.Number
{
    public class NumberVariable : NumberObject
    {
        public NumberVariable(string name) :
          base(0)
        {
            this.Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"{this.Name}";
        }
    }
}