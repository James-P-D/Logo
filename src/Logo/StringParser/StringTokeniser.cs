using System;
using System.Collections.Generic;
using System.Linq;

namespace StringParser
{
    public class StringTokeniser
    {
        #region Operators

        public const string SPACE = " ";
        public const string COMMA = ",";
        public const string ASSIGNMENT = "=";
        public const string START_PARENTHESIS = "(";
        public const string END_PARENTHESIS = ")";

        #endregion

        #region Number Operators

        public const string DIVIDE = "/";
        public const string MULTIPLY = "*";
        public const string MODULUS = "%";
        public const string EXPONENTIAL = "^";
        public const string MINUS = "-";
        public const string PLUS = "+";
        public const string UNARY_MINUS = "#";
        public const string UNARY_PLUS = "@";

        public const string UNARY_SIN = "sin";
        public const string UNARY_COS = "cos";
        public const string UNARY_TAN = "tan";

        public const string MIN = "min";
        public const string MAX = "max";

        #endregion

        #region Boolean Operators

        public const string UNARY_NOT = "!";
        public const string AND = "&&";
        public const string OR = "||";
        public const string XOR = "^^";

        public const string EQUALITY = "==";
        public const string INEQUALITY = "!=";
        public const string GREATER_THAN = ">";
        public const string LESS_THAN = "<";
        public const string GREATER_THAN_OR_EQUAL = ">=";
        public const string LESS_THAN_OR_EQUAL = "<=";

        #endregion

        #region Control, Command Termination and Comments

        public const string END_COMMAND = ";";
        public const string START_BLOCK = "{";
        public const string END_BLOCK = "}";
        public const string COMMENT = "//";

        #endregion

        public StringTokeniser()
        {
        }

        #region Output Debug Information

        public delegate void AddOutputTextDelegate(string text);
        public event AddOutputTextDelegate AddOutputTextEvent;

        private void AddOutputText(string text)
        {
            if (this.AddOutputTextEvent != null)
            {
                this.AddOutputTextEvent.Invoke(text);
            }
        }

        private void Output(string[] lines, int i)
        {
            AddOutputText(string.Format("String Tokeniser Parser {0}", i));
            foreach (string line in lines)
            {
                AddOutputText(line);
            }

            AddOutputText("------");
        }

        #endregion

        #region Tokenising

        public StringToken[] Parse(string[] allLines)
        {
            List<StringToken> tokens = new List<StringToken>();

            Output(allLines, 0);
            string[] crlfRemoved = RemoveCRLF(allLines);
            Output(crlfRemoved, 1);
            string[] commentLess = RemoveComments(crlfRemoved);
            Output(commentLess, 2);
            string[] formattedLines = FormatLines(commentLess);
            Output(formattedLines, 3);

            for (int i = 0; i < formattedLines.Count(); i++)
            {
                foreach (string str in FirstSplit(formattedLines[i]))
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        List<string> strTokens = new List<string>();
                        foreach (string subStr in SecondSplit(str))
                        {
                            if (!string.IsNullOrEmpty(subStr))
                            {
                                strTokens.Add(subStr.ToLower());
                            }
                        }

                        if (strTokens.Count > 0)
                        {
                            tokens.Add(new StringToken(strTokens.ToArray(), i + 1, str));
                        }
                    }
                }
            }

            return tokens.ToArray();
        }

        private string[] RemoveCRLF(string[] lines)
        {
            List<string> crlfLessLines = new List<string>();

            foreach (string line in lines)
            {
                crlfLessLines.Add(line.Replace("\r", "").Replace("\n", ""));
            }

            return crlfLessLines.ToArray();
        }

        private string[] RemoveComments(string[] lines)
        {
            List<string> commentLessLines = new List<string>();

            int i = 0;
            while (i < lines.Count())
            {
                string currentString = lines[i];
                if (currentString.Contains(COMMENT))
                {
                    commentLessLines.Add(currentString.Substring(0, currentString.IndexOf(COMMENT)));
                }
                else
                {
                    commentLessLines.Add(currentString);
                }

                i++;
            }

            return commentLessLines.ToArray();
        }

        private string[] FormatLines(string[] lines)
        {
            List<string> formattedLines = new List<string>();

            string currentString = string.Empty;
            int startLine = 0;

            int i = 0;
            while (i < lines.Count())
            {
                currentString += lines[i];

                if (lines[i].Contains(END_COMMAND) || lines[i].Contains(START_BLOCK) || lines[i].Contains(END_BLOCK))
                {
                    formattedLines.Add(currentString);
                    currentString = string.Empty;
                    while (startLine < i)
                    {
                        formattedLines.Add(String.Empty);
                        startLine++;
                    }

                    startLine = i + 1;
                }
                else if (string.IsNullOrEmpty(currentString.Trim()))
                {
                    formattedLines.Add(String.Empty);
                }
                else
                {
                    throw new Exception(string.Format("Unable to parser '{0}'", currentString));
                }
                i++;
            }

            return formattedLines.ToArray();
        }

        private bool StringMatch(string str, string[] subStrs, int i, ref string match)
        {
            foreach (string subStr in subStrs)
            {
                if (StringMatch(str, subStr, i))
                {
                    match = subStr;
                    return true;
                }
            }

            return false;
        }

        private bool StringMatch(string str, string subStr, int i)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(subStr))
            {
                return false;
            }

            int j = 0;
            while ((j + i < str.Length) && (j < subStr.Length))
            {
                if (!str[j + i].Equals(subStr[j]))
                {
                    return false;
                }

                if (j == subStr.Length - 1)
                {
                    return true;
                }
                else
                {
                    j++;
                }
            }

            return false;
        }

        /// <summary>
        /// Split on ';', '{', and '}', but KEEP the brace characters
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string[] FirstSplit(string str)
        {
            List<string> subStrs = new List<string>();

            string subStr = string.Empty;
            int i = 0;
            while (i < str.Length)
            {
                if (StringMatch(str, END_COMMAND, i))
                {
                    subStrs.Add(subStr);
                    subStr = string.Empty;
                    i += END_COMMAND.Length;
                }
                else if (StringMatch(str, START_BLOCK, i))
                {
                    subStr += str[i];
                    subStrs.Add(subStr);
                    subStr = string.Empty;
                    i += START_BLOCK.Length;
                }
                else if (StringMatch(str, END_BLOCK, i))
                {
                    subStr += str[i];
                    subStrs.Add(subStr);
                    subStr = string.Empty;
                    i += END_BLOCK.Length;
                }
                else
                {
                    subStr += str[i];
                    i++;
                }
            }

            subStr = subStr.Trim();
            if (string.IsNullOrEmpty(subStr))
            {
                subStrs.Add(subStr);
            }

            return subStrs.ToArray();
        }

        string[] separators = new string[] {
      SPACE,
      COMMA,
      ASSIGNMENT,
      START_PARENTHESIS,
      END_PARENTHESIS,
      PLUS,
      MINUS,
      DIVIDE,
      MULTIPLY,
      MODULUS,
      EXPONENTIAL,
      UNARY_NOT,
      AND,
      OR,
      XOR,
      EQUALITY,
      INEQUALITY,
      GREATER_THAN,
      LESS_THAN,
      GREATER_THAN_OR_EQUAL,
      LESS_THAN_OR_EQUAL
    };

        private string[] SecondSplit(string str)
        {
            string[] orderedSeparators = separators.OrderByDescending(s => s.Length).ToArray();
            List<string> subStrs = new List<string>();

            string currentSubStr = string.Empty;
            int i = 0;
            while (i < str.Length)
            {
                char currentChar = str[i];
                string match = string.Empty;
                if (StringMatch(str, orderedSeparators, i, ref match))
                {
                    AddSegment(subStrs, currentSubStr);
                    AddSegment(subStrs, match);
                    currentSubStr = string.Empty;
                    i += match.Length;
                }
                else
                {
                    currentSubStr += currentChar;
                    i++;
                }
            }

            AddSegment(subStrs, currentSubStr);



            return subStrs.ToArray();
        }

        private void AddSegment(List<string> subStrs, string subStr)
        {
            subStr = subStr.Trim();
            if (!string.IsNullOrEmpty(subStr))
            {
                subStrs.Add(subStr);
            }
        }

        #endregion
    }
}