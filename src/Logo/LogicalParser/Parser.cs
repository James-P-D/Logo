using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Commands;
using StringParser;
using LogicalParser.Commands.Evaluation;
using LogicalParser.Objects;
using LogicalParser.Commands.Control;

namespace LogicalParser
{
    public class Parser
    {
        #region Commands

        public const string NUMBER = "number";
        public const string BOOLEAN = "boolean";

        public const string TRUE = "true";
        public const string FALSE = "false";

        public const string PEN_UP = "penup";
        public const string PEN_DOWN = "pendown";
        public const string COLOR_A = "colora";
        public const string COLOR_R = "colorr";
        public const string COLOR_G = "colorg";
        public const string COLOR_B = "colorb";
        public const string CENTER_TURTLE = "centerturtle";
        public const string HIDE_TURTLE = "hideturtle";
        public const string SHOW_TURTLE = "showturtle";

        public const string FORWARD = "forward";
        public const string BACKWARD = "backward";
        public const string RIGHT_TURN = "rightturn";
        public const string LEFT_TURN = "leftturn";
        public const string LEFT = "left";
        public const string RIGHT = "right";

        public const string SET_DIRECTION = "setdirection";
        public const string SET_X = "setx";
        public const string SET_Y = "sety";

        public const string SIN = "sin";
        public const string COS = "cos";
        public const string TAN = "tan";

        public const string MIN = "min";
        public const string MAX = "max";

        public const string REPEAT = "repeat";
        public const string WHILE = "while";
        public const string IF = "if";
        public const string ELSE = "else";
        public const string BREAK = "break";
        public const string CONTINUE = "continue";

        public const string START_BLOCK = "{";
        public const string END_BLOCK = "}";

        public const string OUTPUT = "output";

        private string[] RESERVED_WORDS = {
                                        NUMBER,

                                        BOOLEAN,
                                        TRUE, FALSE,

                                        PEN_UP,
                                        PEN_DOWN,
                                        COLOR_A, COLOR_R, COLOR_G, COLOR_B,
                                        CENTER_TURTLE,
                                        HIDE_TURTLE,
                                        SHOW_TURTLE,
                                        FORWARD,
                                        BACKWARD,
                                        LEFT,
                                        RIGHT,
                                        RIGHT_TURN,
                                        LEFT_TURN,
                                        SET_DIRECTION,
                                        SET_X,
                                        SET_Y,

                                        SIN, COS, TAN,
                                        MIN, MAX,

                                        REPEAT,
                                        WHILE,
                                        IF,
                                        ELSE,
                                        BREAK,
                                        CONTINUE,

                                        OUTPUT
                                    };

        #endregion

        #region Operators

        //TODO: Add Sin Cos Tan, Min/Max (two (or more?) parameters? and if so, how to distinguish between them?) What else?
        private enum OperatorType
        {
            Plus,
            Minus,
            Divide,
            Multiply,
            Modulus,
            Exponential,
            UnaryPlus,
            UnaryMinus,

            UnarySin,
            UnaryCos,
            UnaryTan,

            UnaryNot,
            And,
            Or,
            Xor,

            Min,
            Max,

            Equality,
            Inequality,
            GreaterThan,
            LessThan,
            GreaterThanOrEqual,
            LessThanOrEqual,

            StartParentheses,
            EndParentheses,

            Invalid
        };

        #endregion

        #region Parser

        public void Parse(StringToken[] stringTokens, out List<Command> commands, out List<LogoObject> objects)
        {
            commands = new List<Command>();
            objects = new List<LogoObject>();
            int newStringTokenIndex;
            Parse(0, out newStringTokenIndex, stringTokens, commands, objects, true);
        }

        private void Parse(int stringTokenIndex, out int newStringTokenIndex, StringToken[] stringTokens, List<Command> commands, List<LogoObject> objects, bool outerLoop)
        {
            // Makes sure we add any constants before we start parsing
            objects.Add(new NumberConstant("pi", (float)Math.PI));

            while (stringTokenIndex < stringTokens.Count())
            {
                StringToken stringToken = stringTokens[stringTokenIndex];
                string firstToken = stringToken.Tokens.First();
                int parameters = stringToken.Tokens.Count() - 1;

                if (firstToken.Equals(REPEAT))
                {
                    string lastParam = stringToken.Tokens.Last();

                    if (lastParam.Equals(START_BLOCK))
                    {
                        int updatedStringTokenIndex = 0;

                        List<Command> repeatCommands = new List<Command>();
                        Parse(stringTokenIndex + 1, out updatedStringTokenIndex, stringTokens, repeatCommands, objects, false);
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1, parameters - 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new Repeat(eval as NumberEval, repeatCommands.ToArray()));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }

                        stringTokenIndex = updatedStringTokenIndex;
                    }
                    else
                    {
                        ThrowError(string.Format("Expected '{0}' character not '{1}'", START_BLOCK, lastParam), stringToken);
                        newStringTokenIndex = stringTokenIndex;
                        return;
                    }
                }
                else if (firstToken.Equals(WHILE))
                {
                    string lastParam = stringToken.Tokens.Last();

                    if (lastParam.Equals(START_BLOCK))
                    {
                        int updatedStringTokenIndex = 0;

                        List<Command> whileCommands = new List<Command>();
                        Parse(stringTokenIndex + 1, out updatedStringTokenIndex, stringTokens, whileCommands, objects, false);
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1, parameters - 1), objects, stringToken);
                        if (eval is BooleanEval)
                        {
                            commands.Add(new While(eval as BooleanEval, whileCommands.ToArray()));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Boolean", stringToken);
                        }

                        stringTokenIndex = updatedStringTokenIndex;
                    }
                    else
                    {
                        ThrowError(string.Format("Expected '{0}' character not '{1}'", START_BLOCK, lastParam), stringToken);
                        newStringTokenIndex = stringTokenIndex;
                        return;
                    }
                }
                else if (firstToken.Equals(IF))
                {
                    string lastParam = stringToken.Tokens.Last();

                    if (lastParam.Equals(START_BLOCK))
                    {
                        int updatedStringTokenIndex = 0;

                        List<Command> thenCommands = new List<Command>();
                        Parse(stringTokenIndex + 1, out updatedStringTokenIndex, stringTokens, thenCommands, objects, false);
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1, parameters - 1), objects, stringToken);
                        if (eval is BooleanEval)
                        {
                            commands.Add(new If(eval as BooleanEval, thenCommands.ToArray()));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Boolean", stringToken);
                        }

                        stringTokenIndex = updatedStringTokenIndex;
                    }
                    else
                    {
                        ThrowError(string.Format("Expected '{0}' character not '{1}'", START_BLOCK, lastParam), stringToken);
                        newStringTokenIndex = stringTokenIndex;
                        return;
                    }
                }
                else if (firstToken.Equals(ELSE))
                {
                    string lastParam = stringToken.Tokens.Last();

                    if (lastParam.Equals(START_BLOCK))
                    {
                        int updatedStringTokenIndex = 0;

                        List<Command> elseCommands = new List<Command>();
                        Parse(stringTokenIndex + 1, out updatedStringTokenIndex, stringTokens, elseCommands, objects, false);

                        Command lastCommand = commands.Last();
                        if ((lastCommand != null) && (lastCommand is If))
                        {
                            (lastCommand as If).SetElseCommands(elseCommands.ToArray());
                        }
                        else
                        {
                            ThrowError(string.Format("Orphan '{0}' without an '{1}'", ELSE, IF), stringToken);
                        }

                        stringTokenIndex = updatedStringTokenIndex;
                    }
                    else
                    {
                        ThrowError(string.Format("Expected '{0}' character not '{1}'", START_BLOCK, lastParam), stringToken);
                        newStringTokenIndex = stringTokenIndex;
                        return;
                    }
                }
                else if (firstToken.Equals(END_BLOCK))
                {
                    if (outerLoop)
                    {
                        ThrowError(string.Format("Unexpected '{0}'", firstToken), stringToken);
                    }
                    else
                    {
                        newStringTokenIndex = stringTokenIndex + 1;
                        return;
                    }
                }
                else
                {
                    if (firstToken.Equals(NUMBER))
                    {
                        if (parameters == 0)
                        {
                            ThrowError(string.Format("Parameter expected to variable '{0}'", firstToken), stringToken);
                        }
                        else if (parameters == 1)
                        {
                            string param1 = stringToken.Tokens[1];

                            AddNewNumberVariable(param1, objects, stringToken);
                        }
                        else if (parameters >= 3)
                        {
                            string param1 = stringToken.Tokens[1];
                            string param2 = stringToken.Tokens[2];

                            if (param2.Equals(StringTokeniser.ASSIGNMENT))
                            {
                                AddNewNumberVariable(param1, objects, stringToken);

                                commands.Add(ParseNumberAssignment(param1, CopyArray(stringToken.Tokens, 3), objects, stringToken));
                            }
                            else
                            {
                                ThrowError(string.Format("Cannot parse '{0}'", param2), stringToken);
                            }
                        }
                        else
                        {
                            ThrowError(string.Format("Cannot parse '{0}'", stringToken.OriginalString), stringToken);
                        }
                    }
                    else if (firstToken.Equals(BOOLEAN))
                    {
                        if (parameters == 0)
                        {
                            ThrowError(string.Format("Parameter expected to variable '{0}'", firstToken), stringToken);
                        }
                        else if (parameters == 1)
                        {
                            string param1 = stringToken.Tokens[1];

                            AddNewBooleanVariable(param1, objects, stringToken);
                        }
                        else if (parameters >= 3)
                        {
                            string param1 = stringToken.Tokens[1];
                            string param2 = stringToken.Tokens[2];

                            if (param2.Equals(StringTokeniser.ASSIGNMENT))
                            {
                                AddNewBooleanVariable(param1, objects, stringToken);

                                commands.Add(ParseBooleanAssignment(param1, CopyArray(stringToken.Tokens, 3), objects, stringToken));
                            }
                            else
                            {
                                ThrowError(string.Format("Cannot parse '{0}'", param2), stringToken);
                            }
                        }
                        else
                        {
                            ThrowError(string.Format("Cannot parse '{0}'", stringToken.OriginalString), stringToken);
                        }
                    }
                    else if (GetExistingObject(firstToken, objects) != null)
                    {
                        if (parameters >= 2)
                        {
                            string param1 = stringToken.Tokens[1];

                            if (param1.Equals(StringTokeniser.ASSIGNMENT))
                            {
                                LogoObject variable = GetExistingObject(firstToken, objects);

                                if (variable is NumberVariable)
                                {
                                    commands.Add(ParseNumberAssignment(firstToken, CopyArray(stringToken.Tokens, 2), objects, stringToken));
                                }
                                else if (variable is BooleanVariable)
                                {
                                    commands.Add(ParseBooleanAssignment(firstToken, CopyArray(stringToken.Tokens, 2), objects, stringToken));
                                }
                                else if (variable is NumberConstant || variable is BooleanConstant)
                                {
                                    ThrowError(string.Format("Cannot assign value to constant '{0}'", firstToken), stringToken);
                                }
                                else
                                {
                                    ThrowError(string.Format("Cannot parse '{0}'", param1), stringToken);
                                }
                            }
                            else
                            {
                                ThrowError(string.Format("Cannot parse '{0}'", param1), stringToken);
                            }
                        }
                        else
                        {
                            ThrowError(string.Format("Cannot parse '{0}'", stringToken.OriginalString), stringToken);
                        }
                    }
                    else if (firstToken.Equals(FORWARD))
                    {
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new Forward(eval as NumberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(BACKWARD))
                    {
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new Backward(eval as NumberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(LEFT))
                    {
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new Left(eval as NumberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(RIGHT))
                    {
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new Right(eval as NumberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(RIGHT_TURN))
                    {
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new RightTurn(eval as NumberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(LEFT_TURN))
                    {
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new LeftTurn(eval as NumberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(SET_DIRECTION))
                    {
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new SetDirection(eval as NumberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(SET_X))
                    {
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new SetX(eval as NumberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(SET_Y))
                    {
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new SetY(eval as NumberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(PEN_UP))
                    {
                        commands.Add(new PenUp());
                    }
                    else if (firstToken.Equals(PEN_DOWN))
                    {
                        commands.Add(new PenDown());
                    }
                    else if (firstToken.Equals(COLOR_A))
                    {
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new SetColorA(eval as NumberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(COLOR_R))
                    {
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new SetColorR(eval as NumberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(COLOR_G))
                    {
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new SetColorG(eval as NumberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(COLOR_B))
                    {
                        Eval eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval)
                        {
                            commands.Add(new SetColorB(eval as NumberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(CENTER_TURTLE))
                    {
                        commands.Add(new CenterTurtle());
                    }
                    else if (firstToken.Equals(HIDE_TURTLE))
                    {
                        commands.Add(new HideTurtle());
                    }
                    else if (firstToken.Equals(SHOW_TURTLE))
                    {
                        commands.Add(new ShowTurtle());
                    }
                    else if (firstToken.Equals(OUTPUT))
                    {
                        commands.Add(new Output(ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken)));
                    }
                    else if (firstToken.Equals(BREAK))
                    {
                        commands.Add(new Break());
                    }
                    else if (firstToken.Equals(CONTINUE))
                    {
                        commands.Add(new Continue());
                    }
                    else
                    {
                        ThrowError(string.Format("Don't recognise command '{0}'", firstToken), stringToken);
                    }

                    stringTokenIndex++;
                }
            }
            newStringTokenIndex = stringTokenIndex + 1;
        }

        #region Parser Functions

        private string[] CopyArray(string[] items, int startIndex)
        {
            int length = items.Count() - startIndex;
            string[] subItems = new string[length];
            Array.Copy(items, startIndex, subItems, 0, length);
            return subItems;
        }

        private string[] CopyArray(string[] items, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex + 1;
            string[] subItems = new string[length];
            Array.Copy(items, startIndex, subItems, 0, length);
            return subItems;
        }

        #endregion

        #endregion

        #region Variable Parsing and Evaulation

        private bool IsValidVariableName(string variableName)
        {
            if (string.IsNullOrEmpty(variableName)) return false;

            for (int i = 0; i < variableName.Length; i++)
            {
                char currentChar = variableName[i];
                if (i == 0)
                {
                    if ((!char.IsLetter(currentChar)) && (currentChar != '_'))
                    {
                        return false;
                    }
                }
                else
                {
                    if ((!char.IsLetterOrDigit(currentChar)) && (currentChar != '_'))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private LogoObject GetExistingObject(string variableName, List<LogoObject> objects)
        {
            return objects.FirstOrDefault(v =>
              (v is NumberVariable && (v as NumberVariable).Name.Equals(variableName)) ||
              (v is BooleanVariable && (v as BooleanVariable).Name.Equals(variableName)) ||
              (v is NumberConstant && (v as NumberConstant).Name.Equals(variableName)) ||
              (v is BooleanConstant && (v as BooleanConstant).Name.Equals(variableName)));
        }

        private LogoObject ParseObject(string str, List<LogoObject> objects, StringToken stringToken)
        {
            LogoObject variable = GetExistingObject(str, objects);

            if (variable != null)
            {
                return variable;
            }
            else
            {
                if (str.ToLower().Equals(TRUE))
                {
                    return new BooleanLiteral(true);
                }
                else if (str.ToLower().Equals(FALSE))
                {
                    return new BooleanLiteral(false);
                }

                float floatValue;
                if (float.TryParse(str, out floatValue))
                {
                    return new NumberLiteral(floatValue);
                }
            }

            ThrowError(string.Format("Unable to parse parameter '{0}'", str), stringToken);
            return null;
        }

        #region Number Variables

        private void AddNewNumberVariable(string variableName, List<LogoObject> objects, StringToken stringToken)
        {
            if (RESERVED_WORDS.Contains(variableName))
            {
                ThrowError(string.Format("Cannot declare variable with name '{0}'.", variableName), stringToken);
            }
            if (!IsValidVariableName(variableName))
            {
                ThrowError(string.Format("Variable '{0}' is not a valid name.", variableName), stringToken);
            }
            else
            {
                if (GetExistingObject(variableName, objects) == null)
                {
                    objects.Add(new NumberVariable(variableName));
                }
                else
                {
                    ThrowError(string.Format("Cannot redeclare variable '{0}'", variableName), stringToken);
                }
            }
        }

        private Command ParseNumberAssignment(string variableName, string[] subTokens, List<LogoObject> objects, StringToken stringToken)
        {
            LogoObject variable = GetExistingObject(variableName, objects);
            if (variable is NumberVariable)
            {
                object evaluation = ParseEvaluation(subTokens, objects, stringToken);
                if (evaluation is NumberEval)
                {
                    return new NumberAssign(variable as NumberVariable, (evaluation as NumberEval));
                }
                else
                {
                    ThrowError(string.Format("Cannot parse '{0}'", stringToken), stringToken);
                    return null;
                }
            }
            else
            {
                ThrowError(string.Format("Unable to find variable '{0}'", variableName), stringToken);
                return null;
            }
        }

        #endregion

        #region Boolean Variables

        private void AddNewBooleanVariable(string variableName, List<LogoObject> objects, StringToken stringToken)
        {
            if (RESERVED_WORDS.Contains(variableName))
            {
                ThrowError(string.Format("Cannot declare variable with name '{0}'.", variableName), stringToken);
            }
            if (!IsValidVariableName(variableName))
            {
                ThrowError(string.Format("Variable '{0}' is not a valid name.", variableName), stringToken);
            }
            else
            {
                if (GetExistingObject(variableName, objects) == null)
                {
                    objects.Add(new BooleanVariable(variableName));
                }
                else
                {
                    ThrowError(string.Format("Cannot redeclare variable '{0}'", variableName), stringToken);
                }
            }
        }

        private Command ParseBooleanAssignment(string variableName, string[] subTokens, List<LogoObject> objects, StringToken stringToken)
        {
            LogoObject variable = GetExistingObject(variableName, objects);
            if (variable is BooleanVariable)
            {
                object evaluation = ParseEvaluation(subTokens, objects, stringToken);
                if (evaluation is BooleanEval)
                {
                    return new BooleanAssign(variable as BooleanVariable, (evaluation as BooleanEval));
                }
                else
                {
                    ThrowError(string.Format("Cannot parse '{0}'", stringToken), stringToken);
                    return null;
                }
            }
            else
            {
                ThrowError(string.Format("Unable to find variable '{0}'", variableName), stringToken);
                return null;
            }
        }

        #endregion

        #region Evaluation

        private Eval ParseEvaluation(string[] parameters, List<LogoObject> objects, StringToken stringToken)
        {
            // Before we start the evaluation we need to distinguish between binary and unary plus and minus operators
            // ( '5 - 4' verses '-3') We will convert unary minus to '#' and unary plus to '@'
            for (int i = 0; i < parameters.Count(); i++)
            {
                string previousToken = (i == 0) ? null : parameters[i - 1];
                string currentToken = parameters[i];
                string nextToken = (i >= parameters.Count() - 1) ? null : parameters[i + 1];

                if ((previousToken == null) ||
                  ((previousToken != null) && (GetOperatorSymbol(previousToken) != OperatorType.Invalid)))
                {
                    if (currentToken.Equals(StringTokeniser.MINUS.ToString()))
                    {
                        parameters[i] = StringTokeniser.UNARY_MINUS.ToString();
                    }
                    else if (currentToken.Equals(StringTokeniser.PLUS.ToString()))
                    {
                        parameters[i] = StringTokeniser.UNARY_PLUS.ToString();
                    }
                }
            }

            // See https://en.wikipedia.org/wiki/Shunting-yard_algorithm for more details on this algorithm    
            Stack<Eval> evalStack = new Stack<Eval>();
            Stack<OperatorType> operatorStack = new Stack<OperatorType>();

            foreach (string parameter in parameters)
            {
                if (parameter.Equals(StringTokeniser.COMMA))
                {
                    continue;
                }

                OperatorType currentOperator = GetOperatorSymbol(parameter);
                if (currentOperator == OperatorType.Invalid)
                {
                    LogoObject logoObject = ParseObject(parameter, objects, stringToken);
                    if (logoObject is NumberObject)
                    {
                        evalStack.Push(new NumberValueEval(logoObject as NumberObject));
                    }
                    else if (logoObject is BooleanObject)
                    {
                        evalStack.Push(new BooleanValueEval(logoObject as BooleanObject));
                    }
                    else
                    {
                        throw new Exception("?!?!?");
                    }
                }
                else
                {
                    if (currentOperator == OperatorType.StartParentheses)
                    {
                        operatorStack.Push(currentOperator);
                    }
                    else if (currentOperator == OperatorType.EndParentheses)
                    {
                        while (operatorStack.Count > 0)
                        {
                            if (operatorStack.Peek() == OperatorType.StartParentheses)
                            {
                                operatorStack.Pop();
                                break;
                            }

                            int currentOperatorPrecedence = GetOperatorPrecedence(currentOperator);
                            int previousOperatorPrecedence = GetOperatorPrecedence(operatorStack.Peek());

                            if (previousOperatorPrecedence > currentOperatorPrecedence)
                            {
                                PopOperatorAndEvaluate(operatorStack, evalStack, stringToken);
                            }
                        }
                    }
                    else
                    {
                        int currentOperatorPrecedence = GetOperatorPrecedence(currentOperator);

                        if (currentOperatorPrecedence == -1)
                        {
                            ThrowError(string.Format("Unable to parse '{0}'", stringToken), stringToken);
                            return null;
                        }

                        if (operatorStack.Count > 0)
                        {
                            int previousOperatorPrecedence = GetOperatorPrecedence(operatorStack.Peek());
                            if (previousOperatorPrecedence > currentOperatorPrecedence)
                            {
                                PopOperatorAndEvaluate(operatorStack, evalStack, stringToken);
                            }
                        }

                        operatorStack.Push(currentOperator);
                    }
                }
            }

            while (operatorStack.Count > 0)
            {
                PopOperatorAndEvaluate(operatorStack, evalStack, stringToken);
            }

            if (evalStack.Count() == 1)
            {
                return evalStack.Pop();
            }
            else
            {
                ThrowError(string.Format("Unable to parse '{0}'", stringToken), stringToken);
                return null;
            }
        }

        private void PopOperatorAndEvaluate(Stack<OperatorType> operatorStack, Stack<Eval> evalStack, StringToken stringToken)
        {
            OperatorType currentOperator = operatorStack.Pop();

            switch (currentOperator)
            {
                case OperatorType.UnaryMinus:
                case OperatorType.UnaryPlus:
                case OperatorType.UnarySin:
                case OperatorType.UnaryCos:
                case OperatorType.UnaryTan:
                    {
                        Eval eval = SafePop(evalStack, stringToken);
                        if (eval is NumberEval)
                        {
                            if (currentOperator == OperatorType.UnaryMinus)
                            {
                                evalStack.Push(new NumberUnaryMinusEval(eval as NumberEval));
                            }
                            else if (currentOperator == OperatorType.UnaryPlus)
                            {
                                evalStack.Push(new NumberUnaryPlusEval(eval as NumberEval));
                            }
                            else if (currentOperator == OperatorType.UnaryCos)
                            {
                                evalStack.Push(new NumberUnaryCosEval(eval as NumberEval));
                            }
                            else if (currentOperator == OperatorType.UnarySin)
                            {
                                evalStack.Push(new NumberUnarySinEval(eval as NumberEval));
                            }
                            else if (currentOperator == OperatorType.UnaryTan)
                            {
                                evalStack.Push(new NumberUnaryTanEval(eval as NumberEval));
                            }
                            else
                            {
                                ThrowError("Unable to parse", stringToken);
                            }
                        }
                        else
                        {
                            ThrowError(string.Format("'{0}' can only be used with numbers", currentOperator), stringToken);
                        }
                        break;
                    }
                case OperatorType.Plus:
                case OperatorType.Minus:
                case OperatorType.Multiply:
                case OperatorType.Divide:
                case OperatorType.Modulus:
                case OperatorType.Exponential:
                case OperatorType.Min:
                case OperatorType.Max:
                    {
                        Eval eval2 = SafePop(evalStack, stringToken);
                        Eval eval1 = SafePop(evalStack, stringToken);
                        if ((eval1 is NumberEval) && (eval2 is NumberEval))
                        {
                            if (currentOperator == OperatorType.Plus)
                            {
                                evalStack.Push(new NumberPlusEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else if (currentOperator == OperatorType.Minus)
                            {
                                evalStack.Push(new NumberMinusEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else if (currentOperator == OperatorType.Multiply)
                            {
                                evalStack.Push(new NumberMultiplyEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else if (currentOperator == OperatorType.Divide)
                            {
                                evalStack.Push(new NumberDivideEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else if (currentOperator == OperatorType.Exponential)
                            {
                                evalStack.Push(new NumberExponentialEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else if (currentOperator == OperatorType.Modulus)
                            {
                                evalStack.Push(new NumberModulusEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else if (currentOperator == OperatorType.Min)
                            {
                                evalStack.Push(new NumberMinEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else if (currentOperator == OperatorType.Max)
                            {
                                evalStack.Push(new NumberMaxEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else
                            {
                                ThrowError("Unable to parse", stringToken);
                            }
                        }
                        else
                        {
                            ThrowError(string.Format("'{0}' can only be used with numbers", StringTokeniser.PLUS), stringToken);
                        }
                        break;
                    }
                case OperatorType.UnaryNot:
                    {
                        Eval eval = SafePop(evalStack, stringToken);
                        if (eval is BooleanEval)
                        {
                            evalStack.Push(new BooleanUnaryNotEval(eval as BooleanEval));
                        }
                        else
                        {
                            ThrowError(string.Format("'{0}' can only be used with numbers", StringTokeniser.UNARY_NOT), stringToken);
                        }
                        break;
                    }
                case OperatorType.And:
                case OperatorType.Or:
                case OperatorType.Xor:
                    {
                        Eval eval2 = SafePop(evalStack, stringToken);
                        Eval eval1 = SafePop(evalStack, stringToken);
                        if ((eval1 is BooleanEval) && (eval2 is BooleanEval))
                        {
                            if (currentOperator == OperatorType.And)
                            {
                                evalStack.Push(new BooleanAndEval(eval1 as BooleanEval, eval2 as BooleanEval));
                            }
                            else if (currentOperator == OperatorType.Or)
                            {
                                evalStack.Push(new BooleanOrEval(eval1 as BooleanEval, eval2 as BooleanEval));
                            }
                            else if (currentOperator == OperatorType.Xor)
                            {
                                evalStack.Push(new BooleanXorEval(eval1 as BooleanEval, eval2 as BooleanEval));
                            }
                            else
                            {
                                ThrowError("Unable to parse", stringToken);
                            }
                        }
                        else
                        {
                            ThrowError(string.Format("'{0}' can only be used with numbers", StringTokeniser.PLUS), stringToken);
                        }
                        break;
                    }
                case OperatorType.Equality:
                case OperatorType.Inequality:
                case OperatorType.GreaterThan:
                case OperatorType.LessThan:
                case OperatorType.GreaterThanOrEqual:
                case OperatorType.LessThanOrEqual:
                    {
                        // NO! This can be number and number or bool and bool!
                        Eval eval2 = SafePop(evalStack, stringToken);
                        Eval eval1 = SafePop(evalStack, stringToken);
                        if ((eval1 is NumberEval) && (eval2 is NumberEval))
                        {
                            if (currentOperator == OperatorType.Equality)
                            {
                                evalStack.Push(new BooleanNumberEqualityEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else if (currentOperator == OperatorType.Inequality)
                            {
                                evalStack.Push(new BooleanNumberInequalityEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else if (currentOperator == OperatorType.GreaterThan)
                            {
                                evalStack.Push(new BooleanNumberGreaterThanEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else if (currentOperator == OperatorType.LessThan)
                            {
                                evalStack.Push(new BooleanNumberLessThanEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else if (currentOperator == OperatorType.GreaterThanOrEqual)
                            {
                                evalStack.Push(new BooleanNumberGreaterThanOrEqualEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else if (currentOperator == OperatorType.LessThanOrEqual)
                            {
                                evalStack.Push(new BooleanNumberLessThanOrEqualEval(eval1 as NumberEval, eval2 as NumberEval));
                            }
                            else
                            {
                                ThrowError("Unable to parse", stringToken);
                            }
                        }
                        else if ((eval1 is BooleanEval) && (eval2 is BooleanEval))
                        {
                            if (currentOperator == OperatorType.Equality)
                            {
                                evalStack.Push(new BooleanBooleanEqualityEval(eval1 as BooleanEval, eval2 as BooleanEval));
                            }
                            else if (currentOperator == OperatorType.Inequality)
                            {
                                evalStack.Push(new BooleanBooleanInequalityEval(eval1 as BooleanEval, eval2 as BooleanEval));
                            }
                            else
                            {
                                ThrowError("Unable to parse", stringToken);
                            }
                        }
                        else
                        {
                            ThrowError(string.Format("'{0}' can only be used with matching types (number and numbers or booleans and booleans)", currentOperator.ToString()), stringToken);
                        }
                        break;
                    }
                default:
                    {
                        ThrowError(string.Format("Unable to parse '{0}'", stringToken), stringToken);
                        break;
                    }
            }
        }

        private Eval SafePop(Stack<Eval> evalStack, StringToken stringToken)
        {
            if (evalStack.Count > 0)
            {
                return evalStack.Pop();
            }
            else
            {
                ThrowError(string.Format("Unable to parse '{0}'", stringToken), stringToken);
                return null;
            }
        }

        private OperatorType GetOperatorSymbol(string str)
        {
            if (str.Equals(StringTokeniser.PLUS.ToString())) return OperatorType.Plus;
            else if (str.Equals(StringTokeniser.MINUS.ToString())) return OperatorType.Minus;
            else if (str.Equals(StringTokeniser.DIVIDE.ToString())) return OperatorType.Divide;
            else if (str.Equals(StringTokeniser.MULTIPLY.ToString())) return OperatorType.Multiply;
            else if (str.Equals(StringTokeniser.MODULUS.ToString())) return OperatorType.Modulus;
            else if (str.Equals(StringTokeniser.EXPONENTIAL.ToString())) return OperatorType.Exponential;
            else if (str.Equals(StringTokeniser.UNARY_MINUS.ToString())) return OperatorType.UnaryMinus;
            else if (str.Equals(StringTokeniser.UNARY_PLUS.ToString())) return OperatorType.UnaryPlus;
            else if (str.Equals(StringTokeniser.UNARY_NOT)) return OperatorType.UnaryNot;
            else if (str.Equals(StringTokeniser.AND)) return OperatorType.And;
            else if (str.Equals(StringTokeniser.OR)) return OperatorType.Or;
            else if (str.Equals(StringTokeniser.XOR)) return OperatorType.Xor;
            else if (str.Equals(StringTokeniser.EQUALITY)) return OperatorType.Equality;
            else if (str.Equals(StringTokeniser.INEQUALITY)) return OperatorType.Inequality;
            else if (str.Equals(StringTokeniser.GREATER_THAN)) return OperatorType.GreaterThan;
            else if (str.Equals(StringTokeniser.LESS_THAN)) return OperatorType.LessThan;
            else if (str.Equals(StringTokeniser.GREATER_THAN_OR_EQUAL)) return OperatorType.GreaterThanOrEqual;
            else if (str.Equals(StringTokeniser.LESS_THAN_OR_EQUAL)) return OperatorType.LessThanOrEqual;
            else if (str.Equals(StringTokeniser.START_PARENTHESIS.ToString())) return OperatorType.StartParentheses;
            else if (str.Equals(StringTokeniser.END_PARENTHESIS.ToString())) return OperatorType.EndParentheses;
            else if (str.Equals(StringTokeniser.UNARY_SIN)) return OperatorType.UnarySin;
            else if (str.Equals(StringTokeniser.UNARY_COS)) return OperatorType.UnaryCos;
            else if (str.Equals(StringTokeniser.UNARY_TAN)) return OperatorType.UnaryTan;
            else if (str.Equals(StringTokeniser.MIN)) return OperatorType.Min;
            else if (str.Equals(StringTokeniser.MAX)) return OperatorType.Max;

            return OperatorType.Invalid;
        }

        private int GetOperatorPrecedence(OperatorType mathType)
        {
            switch (mathType)
            {
                case OperatorType.Exponential:
                    return 8;
                case OperatorType.UnaryMinus:
                case OperatorType.UnarySin:
                case OperatorType.UnaryCos:
                case OperatorType.UnaryTan:
                case OperatorType.UnaryPlus:
                case OperatorType.UnaryNot:
                case OperatorType.Min:
                case OperatorType.Max:
                    return 7;
                case OperatorType.Multiply:
                case OperatorType.Divide:
                case OperatorType.Modulus:
                    return 6;
                case OperatorType.Plus:
                case OperatorType.Minus:
                    return 5;
                case OperatorType.GreaterThan:
                case OperatorType.LessThan:
                case OperatorType.GreaterThanOrEqual:
                case OperatorType.LessThanOrEqual:
                    return 4;
                case OperatorType.Equality:
                case OperatorType.Inequality:
                    return 3;
                case OperatorType.And:
                    return 2;
                case OperatorType.Or:
                    return 1;
                case OperatorType.Xor:
                    return 0;

            }

            return -1;
        }

        #endregion

        #endregion

        #region Errors

        private void ThrowError(string message, StringToken stringToken)
        {
            throw new Exception(string.Format("{0}:{1} -- {2}", stringToken.LineNumber, stringToken.OriginalString, message));
        }

        #endregion

    }
}