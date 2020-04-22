using System;
using System.Collections.Generic;
using System.Linq;

namespace StringParser
{
  public class StringTokeniser
  {
    #region Operators

    public const string Space = " ";
    public const string Comma = ",";
    public const string Assignment = "=";
    public const string StartParenthesis = "(";
    public const string EndParenthesis = ")";

    #endregion

    #region Number Operators

    public const string Divide = "/";
    public const string Multiply = "*";
    public const string Modulus = "%";
    public const string Exponential = "^";
    public const string Minus = "-";
    public const string Plus = "+";

    // In general notation there is no difference between binary minus and unary minus ('5 - 4' and '-6' respectively), but in
    // reality they are two totally different operators. When we come to parsing evaluations, we'll need to distinguish between
    // the two so the statement 'x = (+5 - (3 * -2));' will become 'x = (@5 - (3 * #2));'
    public const string UnaryMinus = "#";
    public const string UnaryPlus = "@";

    public const string UnarySin = "sin";
    public const string UnaryCos = "cos";
    public const string UnaryTan = "tan";

    public const string Min = "min";
    public const string Max = "max";

    #endregion

    #region Boolean Operators

    public const string UnaryNot = "!";
    public const string And = "&&";
    public const string Or = "||";
    public const string Xor = "^^";

    public const string Equality = "==";
    public const string Inequality = "!=";
    public const string GreaterThan = ">";
    public const string LessThan = "<";
    public const string GreaterThanOrEqual = ">=";
    public const string LessThanOrEqual = "<=";

    #endregion

    #region Control, Command Termination and Comments

    public const string EndCommand = ";";
    public const string StartBlock = "{";
    public const string EndBlock = "}";
    public const string Comment = "//";

    #endregion

    #region Output Debug Information

    public delegate void AddOutputTextDelegate(string text);

    public event AddOutputTextDelegate AddOutputTextEvent;

    private void AddOutputText(string text)
    {
      AddOutputTextEvent?.Invoke(text);
    }

    private void Output(string[] lines, int i)
    {
      AddOutputText($"String Tokeniser Parser {i}");
      foreach (var line in lines)
      {
        AddOutputText(line);
      }

      AddOutputText("------");
    }

    #endregion

    #region Tokenising

    public StringToken[] Parse(string[] allLines)
    {
      var tokens = new List<StringToken>();

      Output(allLines, 0);
      var crlfRemoved = RemoveCrlf(allLines);
      Output(crlfRemoved, 1);
      var commentsRemoved = RemoveComments(crlfRemoved);
      Output(commentsRemoved, 2);
      var formattedLines = FormatLines(commentsRemoved);
      Output(formattedLines, 3);

      for (var i = 0; i < formattedLines.Length; i++)
      {
        foreach (var str in FirstSplit(formattedLines[i]))
        {
          if (!string.IsNullOrEmpty(str))
          {
            var strTokens = new List<string>();
            foreach (var subStr in SecondSplit(str))
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

    private string[] RemoveCrlf(string[] lines)
    {
      var crlfLessLines = new List<string>();

      foreach (var line in lines)
      {
        crlfLessLines.Add(line.Replace("\r", "").Replace("\n", ""));
      }

      return crlfLessLines.ToArray();
    }

    private string[] RemoveComments(string[] lines)
    {
      var commentLessLines = new List<string>();

      var i = 0;
      while (i < lines.Length)
      {
        var currentString = lines[i];
        commentLessLines.Add(currentString.Contains(Comment)
          ? currentString.Substring(0, currentString.IndexOf(Comment, StringComparison.Ordinal))
          : currentString);

        i++;
      }

      return commentLessLines.ToArray();
    }

    private string[] FormatLines(string[] lines)
    {
      var formattedLines = new List<string>();

      var currentString = string.Empty;
      var startLine = 0;

      var i = 0;
      while (i < lines.Length)
      {
        currentString += lines[i];

        if (lines[i].Contains(EndCommand) || lines[i].Contains(StartBlock) || lines[i].Contains(EndBlock))
        {
          formattedLines.Add(currentString);
          currentString = string.Empty;
          while (startLine < i)
          {
            formattedLines.Add(string.Empty);
            startLine++;
          }

          startLine = i + 1;
        }
        else if (string.IsNullOrEmpty(currentString.Trim()))
        {
          formattedLines.Add(string.Empty);
        }
        else
        {
          throw new Exception($"Unable to parser '{currentString}'");
        }

        i++;
      }

      return formattedLines.ToArray();
    }

    private bool StringMatch(string str, string[] subStrings, int i, ref string match)
    {
      foreach (var subStr in subStrings)
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

      var j = 0;
      while (j + i < str.Length && j < subStr.Length)
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
      var subStrings = new List<string>();

      var subStr = string.Empty;
      var i = 0;
      while (i < str.Length)
      {
        if (StringMatch(str, EndCommand, i))
        {
          subStrings.Add(subStr);
          subStr = string.Empty;
          i += EndCommand.Length;
        }
        else if (StringMatch(str, StartBlock, i))
        {
          subStr += str[i];
          subStrings.Add(subStr);
          subStr = string.Empty;
          i += StartBlock.Length;
        }
        else if (StringMatch(str, EndBlock, i))
        {
          subStr += str[i];
          subStrings.Add(subStr);
          subStr = string.Empty;
          i += EndBlock.Length;
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
        subStrings.Add(subStr);
      }

      return subStrings.ToArray();
    }

    private readonly string[] separators = new string[]
    {
      Space,
      Comma,
      Assignment,
      StartParenthesis,
      EndParenthesis,
      Plus,
      Minus,
      Divide,
      Multiply,
      Modulus,
      Exponential,
      UnaryNot,
      And,
      Or,
      Xor,
      Equality,
      Inequality,
      GreaterThan,
      LessThan,
      GreaterThanOrEqual,
      LessThanOrEqual
    };

    private string[] SecondSplit(string str)
    {
      var orderedSeparators = separators.OrderByDescending(s => s.Length).ToArray();
      var subStrings = new List<string>();

      var currentSubStr = string.Empty;
      var i = 0;
      while (i < str.Length)
      {
        var currentChar = str[i];
        var match = string.Empty;
        if (StringMatch(str, orderedSeparators, i, ref match))
        {
          AddSegment(subStrings, currentSubStr);
          AddSegment(subStrings, match);
          currentSubStr = string.Empty;
          i += match.Length;
        }
        else
        {
          currentSubStr += currentChar;
          i++;
        }
      }

      AddSegment(subStrings, currentSubStr);

      return subStrings.ToArray();
    }

    private void AddSegment(List<string> subStrings, string subStr)
    {
      subStr = subStr.Trim();
      if (!string.IsNullOrEmpty(subStr))
      {
        subStrings.Add(subStr);
      }
    }

    #endregion
  }
}