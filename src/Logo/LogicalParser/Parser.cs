using System;
using System.Collections.Generic;
using System.Linq;
using LogicalParser.Commands;
using LogicalParser.Commands.BooleanEvaluation;
using StringParser;
using LogicalParser.Objects;
using LogicalParser.Commands.Control;
using LogicalParser.Commands.NumberEvaluation;
using LogicalParser.Objects.Boolean;
using LogicalParser.Objects.Number;

namespace LogicalParser
{
    public class Parser
    {
        #region Commands

        public const string Number = "number";
        public const string Boolean = "boolean";

        public const string True = "true";
        public const string False = "false";

        public const string PenUp = "penup";
        public const string PenDown = "pendown";
        public const string ColorA = "colora";
        public const string ColorR = "colorr";
        public const string ColorG = "colorg";
        public const string ColorB = "colorb";
        public const string CenterTurtle = "centerturtle";
        public const string HideTurtle = "hideturtle";
        public const string ShowTurtle = "showturtle";

        public const string Forward = "forward";
        public const string Backward = "backward";
        public const string RightTurn = "rightturn";
        public const string LeftTurn = "leftturn";
        public const string Left = "left";
        public const string Right = "right";

        public const string SetDirection = "setdirection";
        public const string SetX = "setx";
        public const string SetY = "sety";

        public const string Sin = "sin";
        public const string Cos = "cos";
        public const string Tan = "tan";

        public const string Min = "min";
        public const string Max = "max";

        public const string Repeat = "repeat";
        public const string While = "while";
        public const string If = "if";
        public const string Else = "else";
        public const string Break = "break";
        public const string Continue = "continue";

        public const string StartBlock = "{";
        public const string EndBlock = "}";

        public const string Output = "output";

        private readonly string[] reservedWords = {
                                        Number,

                                        Boolean,
                                        True, False,

                                        PenUp,
                                        PenDown,
                                        ColorA, ColorR, ColorG, ColorB,
                                        CenterTurtle,
                                        HideTurtle,
                                        ShowTurtle,
                                        Forward,
                                        Backward,
                                        Left,
                                        Right,
                                        RightTurn,
                                        LeftTurn,
                                        SetDirection,
                                        SetX,
                                        SetY,

                                        Sin, Cos, Tan,
                                        Min, Max,

                                        Repeat,
                                        While,
                                        If,
                                        Else,
                                        Break,
                                        Continue,

                                        Output
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
            Parse(0, out _, stringTokens, commands, objects, true);
        }

        private void Parse(int stringTokenIndex, out int newStringTokenIndex, StringToken[] stringTokens, List<Command> commands, List<LogoObject> objects, bool outerLoop)
        {
            // Makes sure we add any constants before we start parsing
            objects.Add(new NumberConstant("pi", (float)Math.PI));

            while (stringTokenIndex < stringTokens.Length)
            {
                var stringToken = stringTokens[stringTokenIndex];
                var firstToken = stringToken.Tokens.First();
                var parameters = stringToken.Tokens.Length - 1;

                if (firstToken.Equals(Repeat))
                {
                    var lastParam = stringToken.Tokens.Last();

                    if (lastParam.Equals(StartBlock))
                    {
                        var repeatCommands = new List<Command>();
                        Parse(stringTokenIndex + 1, out var updatedStringTokenIndex, stringTokens, repeatCommands, objects, false);
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1, parameters - 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new Repeat(numberEval, repeatCommands.ToArray()));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }

                        stringTokenIndex = updatedStringTokenIndex;
                    }
                    else
                    {
                        ThrowError($"Expected '{StartBlock}' character not '{lastParam}'", stringToken);
                        newStringTokenIndex = stringTokenIndex;
                        return;
                    }
                }
                else if (firstToken.Equals(While))
                {
                    var lastParam = stringToken.Tokens.Last();

                    if (lastParam.Equals(StartBlock))
                    {
                        var whileCommands = new List<Command>();
                        Parse(stringTokenIndex + 1, out var updatedStringTokenIndex, stringTokens, whileCommands, objects, false);
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1, parameters - 1), objects, stringToken);
                        if (eval is BooleanEval booleanEval)
                        {
                            commands.Add(new While(booleanEval, whileCommands.ToArray()));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Boolean", stringToken);
                        }

                        stringTokenIndex = updatedStringTokenIndex;
                    }
                    else
                    {
                        ThrowError($"Expected '{StartBlock}' character not '{lastParam}'", stringToken);
                        newStringTokenIndex = stringTokenIndex;
                        return;
                    }
                }
                else if (firstToken.Equals(If))
                {
                    var lastParam = stringToken.Tokens.Last();

                    if (lastParam.Equals(StartBlock))
                    {
                        var thenCommands = new List<Command>();
                        Parse(stringTokenIndex + 1, out var updatedStringTokenIndex, stringTokens, thenCommands, objects, false);
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1, parameters - 1), objects, stringToken);
                        if (eval is BooleanEval booleanEval)
                        {
                            commands.Add(new If(booleanEval, thenCommands.ToArray()));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Boolean", stringToken);
                        }

                        stringTokenIndex = updatedStringTokenIndex;
                    }
                    else
                    {
                        ThrowError($"Expected '{StartBlock}' character not '{lastParam}'", stringToken);
                        newStringTokenIndex = stringTokenIndex;
                        return;
                    }
                }
                else if (firstToken.Equals(Else))
                {
                    var lastParam = stringToken.Tokens.Last();

                    if (lastParam.Equals(StartBlock))
                    {
                        var elseCommands = new List<Command>();
                        Parse(stringTokenIndex + 1, out var updatedStringTokenIndex, stringTokens, elseCommands, objects, false);

                        var lastCommand = commands.Last();
                        if (lastCommand is If @if)
                        {
                            @if.SetElseCommands(elseCommands.ToArray());
                        }
                        else
                        {
                            ThrowError($"Orphan '{Else}' without an '{If}'", stringToken);
                        }

                        stringTokenIndex = updatedStringTokenIndex;
                    }
                    else
                    {
                        ThrowError($"Expected '{StartBlock}' character not '{lastParam}'", stringToken);
                        newStringTokenIndex = stringTokenIndex;
                        return;
                    }
                }
                else if (firstToken.Equals(EndBlock))
                {
                    if (outerLoop)
                    {
                        ThrowError($"Unexpected '{firstToken}'", stringToken);
                    }
                    else
                    {
                        newStringTokenIndex = stringTokenIndex + 1;
                        return;
                    }
                }
                else
                {
                    if (firstToken.Equals(Number))
                    {
                        if (parameters == 0)
                        {
                            ThrowError($"Parameter expected to variable '{firstToken}'", stringToken);
                        }
                        else if (parameters == 1)
                        {
                            var param1 = stringToken.Tokens[1];

                            AddNewNumberVariable(param1, objects, stringToken);
                        }
                        else if (parameters >= 3)
                        {
                            var param1 = stringToken.Tokens[1];
                            var param2 = stringToken.Tokens[2];

                            if (param2.Equals(StringTokeniser.Assignment))
                            {
                                AddNewNumberVariable(param1, objects, stringToken);

                                commands.Add(ParseNumberAssignment(param1, CopyArray(stringToken.Tokens, 3), objects, stringToken));
                            }
                            else
                            {
                                ThrowError($"Cannot parse '{param2}'", stringToken);
                            }
                        }
                        else
                        {
                            ThrowError($"Cannot parse '{stringToken.OriginalString}'", stringToken);
                        }
                    }
                    else if (firstToken.Equals(Boolean))
                    {
                        if (parameters == 0)
                        {
                            ThrowError($"Parameter expected to variable '{firstToken}'", stringToken);
                        }
                        else if (parameters == 1)
                        {
                            var param1 = stringToken.Tokens[1];

                            AddNewBooleanVariable(param1, objects, stringToken);
                        }
                        else if (parameters >= 3)
                        {
                            var param1 = stringToken.Tokens[1];
                            var param2 = stringToken.Tokens[2];

                            if (param2.Equals(StringTokeniser.Assignment))
                            {
                                AddNewBooleanVariable(param1, objects, stringToken);

                                commands.Add(ParseBooleanAssignment(param1, CopyArray(stringToken.Tokens, 3), objects, stringToken));
                            }
                            else
                            {
                                ThrowError($"Cannot parse '{param2}'", stringToken);
                            }
                        }
                        else
                        {
                            ThrowError($"Cannot parse '{stringToken.OriginalString}'", stringToken);
                        }
                    }
                    else if (GetExistingObject(firstToken, objects) != null)
                    {
                        if (parameters >= 2)
                        {
                            var param1 = stringToken.Tokens[1];

                            if (param1.Equals(StringTokeniser.Assignment))
                            {
                                var variable = GetExistingObject(firstToken, objects);

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
                                    ThrowError($"Cannot assign value to constant '{firstToken}'", stringToken);
                                }
                                else
                                {
                                    ThrowError($"Cannot parse '{param1}'", stringToken);
                                }
                            }
                            else
                            {
                                ThrowError($"Cannot parse '{param1}'", stringToken);
                            }
                        }
                        else
                        {
                            ThrowError($"Cannot parse '{stringToken.OriginalString}'", stringToken);
                        }
                    }
                    else if (firstToken.Equals(Forward))
                    {
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new Forward(numberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(Backward))
                    {
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new Backward(numberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(Left))
                    {
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new Left(numberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(Right))
                    {
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new Right(numberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(RightTurn))
                    {
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new RightTurn(numberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(LeftTurn))
                    {
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new LeftTurn(numberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(SetDirection))
                    {
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new SetDirection(numberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(SetX))
                    {
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new SetX(numberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(SetY))
                    {
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new SetY(numberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(PenUp))
                    {
                        commands.Add(new PenUp());
                    }
                    else if (firstToken.Equals(PenDown))
                    {
                        commands.Add(new PenDown());
                    }
                    else if (firstToken.Equals(ColorA))
                    {
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new SetColorA(numberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(ColorR))
                    {
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new SetColorR(numberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(ColorG))
                    {
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new SetColorG(numberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(ColorB))
                    {
                        var eval = ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            commands.Add(new SetColorB(numberEval));
                        }
                        else
                        {
                            ThrowError("Value doesn't resolve to a Number", stringToken);
                        }
                    }
                    else if (firstToken.Equals(CenterTurtle))
                    {
                        commands.Add(new CenterTurtle());
                    }
                    else if (firstToken.Equals(HideTurtle))
                    {
                        commands.Add(new HideTurtle());
                    }
                    else if (firstToken.Equals(ShowTurtle))
                    {
                        commands.Add(new ShowTurtle());
                    }
                    else if (firstToken.Equals(Output))
                    {
                        commands.Add(new Output(ParseEvaluation(CopyArray(stringToken.Tokens, 1), objects, stringToken)));
                    }
                    else if (firstToken.Equals(Break))
                    {
                        commands.Add(new Break());
                    }
                    else if (firstToken.Equals(Continue))
                    {
                        commands.Add(new Continue());
                    }
                    else
                    {
                        ThrowError($"Don't recognise command '{firstToken}'", stringToken);
                    }

                    stringTokenIndex++;
                }
            }
            newStringTokenIndex = stringTokenIndex + 1;
        }

        #region Parser Functions

        private string[] CopyArray(string[] items, int startIndex)
        {
            var length = items.Length - startIndex;
            var subItems = new string[length];
            Array.Copy(items, startIndex, subItems, 0, length);
            return subItems;
        }

        private string[] CopyArray(string[] items, int startIndex, int endIndex)
        {
            var length = endIndex - startIndex + 1;
            var subItems = new string[length];
            Array.Copy(items, startIndex, subItems, 0, length);
            return subItems;
        }

        #endregion

        #endregion

        #region Variable Parsing and Evaulation

        private bool IsValidVariableName(string variableName)
        {
            if (string.IsNullOrEmpty(variableName)) return false;

            for (var i = 0; i < variableName.Length; i++)
            {
                var currentChar = variableName[i];
                if (i == 0)
                {
                    if (!char.IsLetter(currentChar) && currentChar != '_')
                    {
                        return false;
                    }
                }
                else
                {
                    if (!char.IsLetterOrDigit(currentChar) && currentChar != '_')
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
              v is NumberVariable numberVariable && numberVariable.Name.Equals(variableName) ||
              v is BooleanVariable booleanVariable && booleanVariable.Name.Equals(variableName) ||
              v is NumberConstant numberConstant && numberConstant.Name.Equals(variableName) ||
              v is BooleanConstant booleanConstant && booleanConstant.Name.Equals(variableName));
        }

        private LogoObject ParseObject(string str, List<LogoObject> objects, StringToken stringToken)
        {
            var variable = GetExistingObject(str, objects);

            if (variable != null)
            {
                return variable;
            }
            else
            {
                if (str.ToLower().Equals(True))
                {
                    return new BooleanLiteral(true);
                }
                else if (str.ToLower().Equals(False))
                {
                    return new BooleanLiteral(false);
                }

                if (float.TryParse(str, out var floatValue))
                {
                    return new NumberLiteral(floatValue);
                }
            }

            ThrowError($"Unable to parse parameter '{str}'", stringToken);
            return null;
        }

        #region Number Variables

        private void AddNewNumberVariable(string variableName, List<LogoObject> objects, StringToken stringToken)
        {
            if (reservedWords.Contains(variableName))
            {
                ThrowError($"Cannot declare variable with name '{variableName}'.", stringToken);
            }
            if (!IsValidVariableName(variableName))
            {
                ThrowError($"Variable '{variableName}' is not a valid name.", stringToken);
            }
            else
            {
                if (GetExistingObject(variableName, objects) == null)
                {
                    objects.Add(new NumberVariable(variableName));
                }
                else
                {
                    ThrowError($"Cannot redeclare variable '{variableName}'", stringToken);
                }
            }
        }

        private Command ParseNumberAssignment(string variableName, string[] subTokens, List<LogoObject> objects, StringToken stringToken)
        {
            var variable = GetExistingObject(variableName, objects);
            if (variable is NumberVariable numberVariable)
            {
                object evaluation = ParseEvaluation(subTokens, objects, stringToken);
                if (evaluation is NumberEval numberEval)
                {
                    return new NumberAssign(numberVariable, numberEval);
                }
                else
                {
                    ThrowError($"Cannot parse '{stringToken}'", stringToken);
                    return null;
                }
            }
            else
            {
                ThrowError($"Unable to find variable '{variableName}'", stringToken);
                return null;
            }
        }

        #endregion

        #region Boolean Variables

        private void AddNewBooleanVariable(string variableName, List<LogoObject> objects, StringToken stringToken)
        {
            if (reservedWords.Contains(variableName))
            {
                ThrowError($"Cannot declare variable with name '{variableName}'.", stringToken);
            }
            if (!IsValidVariableName(variableName))
            {
                ThrowError($"Variable '{variableName}' is not a valid name.", stringToken);
            }
            else
            {
                if (GetExistingObject(variableName, objects) == null)
                {
                    objects.Add(new BooleanVariable(variableName));
                }
                else
                {
                    ThrowError($"Cannot redeclare variable '{variableName}'", stringToken);
                }
            }
        }

        private Command ParseBooleanAssignment(string variableName, string[] subTokens, List<LogoObject> objects, StringToken stringToken)
        {
            var variable = GetExistingObject(variableName, objects);
            if (variable is BooleanVariable booleanVariable)
            {
                object evaluation = ParseEvaluation(subTokens, objects, stringToken);
                if (evaluation is BooleanEval booleanEval)
                {
                    return new BooleanAssign(booleanVariable, booleanEval);
                }
                else
                {
                    ThrowError($"Cannot parse '{stringToken}'", stringToken);
                    return null;
                }
            }
            else
            {
                ThrowError($"Unable to find variable '{variableName}'", stringToken);
                return null;
            }
        }

        #endregion

        #region Evaluation

        private Eval ParseEvaluation(string[] parameters, List<LogoObject> objects, StringToken stringToken)
        {
            // Before we start the evaluation we need to distinguish between binary and unary plus and minus operators
            // ( '5 - 4' verses '-3') We will convert unary minus to '#' and unary plus to '@'
            for (var i = 0; i < parameters.Length; i++)
            {
                var previousToken = i == 0 ? null : parameters[i - 1];
                var currentToken = parameters[i];

                if (previousToken == null || GetOperatorSymbol(previousToken) != OperatorType.Invalid)
                {
                    if (currentToken.Equals(StringTokeniser.Minus))
                    {
                        parameters[i] = StringTokeniser.UnaryMinus;
                    }
                    else if (currentToken.Equals(StringTokeniser.Plus))
                    {
                        parameters[i] = StringTokeniser.UnaryPlus;
                    }
                }
            }

            // See https://en.wikipedia.org/wiki/Shunting-yard_algorithm for more details on this algorithm    
            var evalStack = new Stack<Eval>();
            var operatorStack = new Stack<OperatorType>();

            foreach (var parameter in parameters)
            {
                if (parameter.Equals(StringTokeniser.Comma))
                {
                    continue;
                }

                var currentOperator = GetOperatorSymbol(parameter);
                if (currentOperator == OperatorType.Invalid)
                {
                    var logoObject = ParseObject(parameter, objects, stringToken);
                    if (logoObject is NumberObject numberObject)
                    {
                        evalStack.Push(new NumberValueEval(numberObject));
                    }
                    else if (logoObject is BooleanObject booleanObject)
                    {
                        evalStack.Push(new BooleanValueEval(booleanObject));
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

                            var currentOperatorPrecedence = GetOperatorPrecedence(currentOperator);
                            var previousOperatorPrecedence = GetOperatorPrecedence(operatorStack.Peek());

                            if (previousOperatorPrecedence > currentOperatorPrecedence)
                            {
                                PopOperatorAndEvaluate(operatorStack, evalStack, stringToken);
                            }
                        }
                    }
                    else
                    {
                        var currentOperatorPrecedence = GetOperatorPrecedence(currentOperator);

                        if (currentOperatorPrecedence == -1)
                        {
                            ThrowError($"Unable to parse '{stringToken}'", stringToken);
                            return null;
                        }

                        if (operatorStack.Count > 0)
                        {
                            var previousOperatorPrecedence = GetOperatorPrecedence(operatorStack.Peek());
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

            if (evalStack.Count == 1)
            {
                return evalStack.Pop();
            }
            else
            {
                ThrowError($"Unable to parse '{stringToken}'", stringToken);
                return null;
            }
        }

        private void PopOperatorAndEvaluate(Stack<OperatorType> operatorStack, Stack<Eval> evalStack, StringToken stringToken)
        {
            var currentOperator = operatorStack.Pop();

            switch (currentOperator)
            {
                case OperatorType.UnaryMinus:
                case OperatorType.UnaryPlus:
                case OperatorType.UnarySin:
                case OperatorType.UnaryCos:
                case OperatorType.UnaryTan:
                    {
                        var eval = SafePop(evalStack, stringToken);
                        if (eval is NumberEval numberEval)
                        {
                            if (currentOperator == OperatorType.UnaryMinus)
                            {
                                evalStack.Push(new NumberUnaryMinusEval(numberEval));
                            }
                            else if (currentOperator == OperatorType.UnaryPlus)
                            {
                                evalStack.Push(new NumberUnaryPlusEval(numberEval));
                            }
                            else if (currentOperator == OperatorType.UnaryCos)
                            {
                                evalStack.Push(new NumberUnaryCosEval(numberEval));
                            }
                            else if (currentOperator == OperatorType.UnarySin)
                            {
                                evalStack.Push(new NumberUnarySinEval(numberEval));
                            }
                            else if (currentOperator == OperatorType.UnaryTan)
                            {
                                evalStack.Push(new NumberUnaryTanEval(numberEval));
                            }
                            else
                            {
                                ThrowError("Unable to parse", stringToken);
                            }
                        }
                        else
                        {
                            ThrowError($"'{currentOperator}' can only be used with numbers", stringToken);
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
                        var eval2 = SafePop(evalStack, stringToken);
                        var eval1 = SafePop(evalStack, stringToken);
                        if (eval1 is NumberEval numberEval1 && eval2 is NumberEval numberEval2)
                        {
                            if (currentOperator == OperatorType.Plus)
                            {
                                evalStack.Push(new NumberPlusEval(numberEval1, numberEval2));
                            }
                            else if (currentOperator == OperatorType.Minus)
                            {
                                evalStack.Push(new NumberMinusEval(numberEval1, numberEval2));
                            }
                            else if (currentOperator == OperatorType.Multiply)
                            {
                                evalStack.Push(new NumberMultiplyEval(numberEval1, numberEval2));
                            }
                            else if (currentOperator == OperatorType.Divide)
                            {
                                evalStack.Push(new NumberDivideEval(numberEval1, numberEval2));
                            }
                            else if (currentOperator == OperatorType.Exponential)
                            {
                                evalStack.Push(new NumberExponentialEval(numberEval1, numberEval2));
                            }
                            else if (currentOperator == OperatorType.Modulus)
                            {
                                evalStack.Push(new NumberModulusEval(numberEval1, numberEval2));
                            }
                            else if (currentOperator == OperatorType.Min)
                            {
                                evalStack.Push(new NumberMinEval(numberEval1, numberEval2));
                            }
                            else if (currentOperator == OperatorType.Max)
                            {
                                evalStack.Push(new NumberMaxEval(numberEval1, numberEval2));
                            }
                            else
                            {
                                ThrowError("Unable to parse", stringToken);
                            }
                        }
                        else
                        {
                            ThrowError($"'{StringTokeniser.Plus}' can only be used with numbers", stringToken);
                        }
                        break;
                    }
                case OperatorType.UnaryNot:
                    {
                        var eval = SafePop(evalStack, stringToken);
                        if (eval is BooleanEval booleanEval)
                        {
                            evalStack.Push(new BooleanUnaryNotEval(booleanEval));
                        }
                        else
                        {
                            ThrowError($"'{StringTokeniser.UnaryNot}' can only be used with numbers", stringToken);
                        }
                        break;
                    }
                case OperatorType.And:
                case OperatorType.Or:
                case OperatorType.Xor:
                    {
                        var eval2 = SafePop(evalStack, stringToken);
                        var eval1 = SafePop(evalStack, stringToken);
                        if (eval1 is BooleanEval booleanEval1 && eval2 is BooleanEval booleanEval2)
                        {
                            if (currentOperator == OperatorType.And)
                            {
                                evalStack.Push(new BooleanAndEval(booleanEval1, booleanEval2));
                            }
                            else if (currentOperator == OperatorType.Or)
                            {
                                evalStack.Push(new BooleanOrEval(booleanEval1, booleanEval2));
                            }
                            else if (currentOperator == OperatorType.Xor)
                            {
                                evalStack.Push(new BooleanXorEval(booleanEval1, booleanEval2));
                            }
                            else
                            {
                                ThrowError("Unable to parse", stringToken);
                            }
                        }
                        else
                        {
                            ThrowError($"'{StringTokeniser.Plus}' can only be used with numbers", stringToken);
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
                        var eval2 = SafePop(evalStack, stringToken);
                        var eval1 = SafePop(evalStack, stringToken);
                        if (eval1 is NumberEval numberEval1 && eval2 is NumberEval numberEval2)
                        {
                            if (currentOperator == OperatorType.Equality)
                            {
                                evalStack.Push(new BooleanNumberEqualityEval(numberEval1, numberEval2));
                            }
                            else if (currentOperator == OperatorType.Inequality)
                            {
                                evalStack.Push(new BooleanNumberInequalityEval(numberEval1, numberEval2));
                            }
                            else if (currentOperator == OperatorType.GreaterThan)
                            {
                                evalStack.Push(new BooleanNumberGreaterThanEval(numberEval1, numberEval2));
                            }
                            else if (currentOperator == OperatorType.LessThan)
                            {
                                evalStack.Push(new BooleanNumberLessThanEval(numberEval1, numberEval2));
                            }
                            else if (currentOperator == OperatorType.GreaterThanOrEqual)
                            {
                                evalStack.Push(new BooleanNumberGreaterThanOrEqualEval(numberEval1, numberEval2));
                            }
                            else if (currentOperator == OperatorType.LessThanOrEqual)
                            {
                                evalStack.Push(new BooleanNumberLessThanOrEqualEval(numberEval1, numberEval2));
                            }
                            else
                            {
                                ThrowError("Unable to parse", stringToken);
                            }
                        }
                        else if (eval1 is BooleanEval booleanEval1 && eval2 is BooleanEval booleanEval2)
                        {
                            if (currentOperator == OperatorType.Equality)
                            {
                                evalStack.Push(new BooleanBooleanEqualityEval(booleanEval1, booleanEval2));
                            }
                            else if (currentOperator == OperatorType.Inequality)
                            {
                                evalStack.Push(new BooleanBooleanInequalityEval(booleanEval1, booleanEval2));
                            }
                            else
                            {
                                ThrowError("Unable to parse", stringToken);
                            }
                        }
                        else
                        {
                            ThrowError(
                                $"'{currentOperator.ToString()}' can only be used with matching types (number and numbers or booleans and booleans)", stringToken);
                        }
                        break;
                    }
                default:
                    {
                        ThrowError($"Unable to parse '{stringToken}'", stringToken);
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
                ThrowError($"Unable to parse '{stringToken}'", stringToken);
                return null;
            }
        }

        private OperatorType GetOperatorSymbol(string str)
        {
            if (str.Equals(StringTokeniser.Plus)) return OperatorType.Plus;
            else if (str.Equals(StringTokeniser.Minus)) return OperatorType.Minus;
            else if (str.Equals(StringTokeniser.Divide)) return OperatorType.Divide;
            else if (str.Equals(StringTokeniser.Multiply)) return OperatorType.Multiply;
            else if (str.Equals(StringTokeniser.Modulus)) return OperatorType.Modulus;
            else if (str.Equals(StringTokeniser.Exponential)) return OperatorType.Exponential;
            else if (str.Equals(StringTokeniser.UnaryMinus)) return OperatorType.UnaryMinus;
            else if (str.Equals(StringTokeniser.UnaryPlus)) return OperatorType.UnaryPlus;
            else if (str.Equals(StringTokeniser.UnaryNot)) return OperatorType.UnaryNot;
            else if (str.Equals(StringTokeniser.And)) return OperatorType.And;
            else if (str.Equals(StringTokeniser.Or)) return OperatorType.Or;
            else if (str.Equals(StringTokeniser.Xor)) return OperatorType.Xor;
            else if (str.Equals(StringTokeniser.Equality)) return OperatorType.Equality;
            else if (str.Equals(StringTokeniser.Inequality)) return OperatorType.Inequality;
            else if (str.Equals(StringTokeniser.GreaterThan)) return OperatorType.GreaterThan;
            else if (str.Equals(StringTokeniser.LessThan)) return OperatorType.LessThan;
            else if (str.Equals(StringTokeniser.GreaterThanOrEqual)) return OperatorType.GreaterThanOrEqual;
            else if (str.Equals(StringTokeniser.LessThanOrEqual)) return OperatorType.LessThanOrEqual;
            else if (str.Equals(StringTokeniser.StartParenthesis)) return OperatorType.StartParentheses;
            else if (str.Equals(StringTokeniser.EndParenthesis)) return OperatorType.EndParentheses;
            else if (str.Equals(StringTokeniser.UnarySin)) return OperatorType.UnarySin;
            else if (str.Equals(StringTokeniser.UnaryCos)) return OperatorType.UnaryCos;
            else if (str.Equals(StringTokeniser.UnaryTan)) return OperatorType.UnaryTan;
            else if (str.Equals(StringTokeniser.Min)) return OperatorType.Min;
            else if (str.Equals(StringTokeniser.Max)) return OperatorType.Max;

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
            throw new Exception($"{stringToken.LineNumber}:{stringToken.OriginalString} -- {message}");
        }

        #endregion

    }
}