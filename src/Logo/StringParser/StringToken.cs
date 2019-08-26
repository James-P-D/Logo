namespace StringParser
{
    public class StringToken
    {
        public StringToken(string[] tokens, int lineNumber, string originalString)
        {
            this.Tokens = tokens;
            this.LineNumber = lineNumber;
            this.OriginalString = originalString;
        }

        public string[] Tokens { get; }
        public int LineNumber { get; }
        public string OriginalString { get; }
    }
}