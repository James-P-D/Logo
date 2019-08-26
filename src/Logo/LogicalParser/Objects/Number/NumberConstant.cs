namespace LogicalParser.Objects.Number
{
    public class NumberConstant : NumberObject
    {
        public NumberConstant(string name, float val) :
          base(val)
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