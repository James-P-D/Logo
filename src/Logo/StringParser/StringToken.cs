using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public string[] Tokens { get; private set; }
        public int LineNumber { get; private set; }
        public string OriginalString { get; private set; }
    }
}