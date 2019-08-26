namespace StringParser
{
    public class StringToken
    {
        public StringToken(string[] tokens, int lineNumber, string originalString)
        {
            Tokens = tokens;
            LineNumber = lineNumber;
            OriginalString = originalString;
        }

        public string[] Tokens { get; }
        public int LineNumber { get; }
        public string OriginalString { get; }
    }
}