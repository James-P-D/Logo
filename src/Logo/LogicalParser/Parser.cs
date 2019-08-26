﻿using System;
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

        private string[] reservedWords = {
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
            int newStringTokenIndex;
            Parse(0, out newStringTokenIndex, stringTokens, commands, objects, true);
        }

        private void Parse(int stringTokenIndex, out int newStringTokenIndex, StringToken[] stringTokens, List<Command> commands, List<LogoObject> objects, bool outerLoop)
        {
            // Makes sure we add any constants before we start parsing
            objects.Add(new NumberConstant("pi", (float)Math.PI));

            while (stringTokenIndex < stringTokens.Length)
            {
                StringToken stringToken = stringTokens[stringTokenIndex];
                string firstToken = stringToken.Tokens.First();
                int parameters = stringToken.Tokens.Length - 1;

                if (firstToken.Equals(Repeat))
                {
                    string lastParam = stringToken.Tokens.Last();

                    if (lastParam.Equals(StartBlock))
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
                        ThrowError($"Expected '{StartBlock}' character not '{lastParam}'", stringToken);
                        newStringTokenIndex = stringTokenIndex;
                        return;
                    }
                }
                else if (firstToken.Equals(While))
                {
                    string lastParam = stringToken.Tokens.Last();

                    if (lastParam.Equals(StartBlock))
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
                        ThrowError($"Expected '{StartBlock}' character not '{lastParam}'", stringToken);
                        newStringTokenIndex = stringTokenIndex;
                        return;
                    }
                }
                else if (firstToken.Equals(If))
                {
                    string lastParam = stringToken.Tokens.Last();

                    if (lastParam.Equals(StartBlock))
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
                        ThrowError($"Expected '{StartBlock}' character not '{lastParam}'", stringToken);
                        newStringTokenIndex = stringTokenIndex;
                        return;
                    }
                }
                else if (firstToken.Equals(Else))
                {
                    string lastParam = stringToken.Tokens.Last();

                    if (lastParam.Equals(StartBlock))
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
                            string param1 = stringToken.Tokens[1];

                            AddNewNumberVariable(param1, objects, stringToken);
                        }
                        else if (parameters >= 3)
                        {
                            string param1 = stringToken.Tokens[1];
                            string param2 = stringToken.Tokens[2];

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
                            string param1 = stringToken.Tokens[1];

                            AddNewBooleanVariable(param1, objects, stringToken);
                        }
                        else if (parameters >= 3)
                        {
                            string param1 = stringToken.Tokens[1];
                            string param2 = stringToken.Tokens[2];

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
                            string param1 = stringToken.Tokens[1];

                            if (param1.Equals(StringTokeniser.Assignment))
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
                    else if (firstToken.Equals(Backward))
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
                    else if (firstToken.Equals(Left))
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
                    else if (firstToken.Equals(Right))
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
                    else if (firstToken.Equals(RightTurn))
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
                    else if (firstToken.Equals(LeftTurn))
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
                    else if (firstToken.Equals(SetDirection))
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
                    else if (firstToken.Equals(SetX))
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
                    else if (firstToken.Equals(SetY))
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
                    else if (firstToken.Equals(ColorR))
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
                    else if (firstToken.Equals(ColorG))
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
                    else if (firstToken.Equals(ColorB))
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
            int length = items.Length - startIndex;
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
                if (str.ToLower().Equals(True))
                {
                    return new BooleanLiteral(true);
                }
                else if (str.ToLower().Equals(False))
                {
                    return new BooleanLiteral(false);
                }

                float floatValue;
                if (float.TryParse(str, out floatValue))
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
            for (int i = 0; i < parameters.Length; i++)
            {
                string previousToken = (i == 0) ? null : parameters[i - 1];
                string currentToken = parameters[i];
                string nextToken = (i >= parameters.Length - 1) ? null : parameters[i + 1];

                if ((previousToken == null) ||
                  ((previousToken != null) && (GetOperatorSymbol(previousToken) != OperatorType.Invalid)))
                {
                    if (currentToken.Equals(StringTokeniser.Minus.ToString()))
                    {
                        parameters[i] = StringTokeniser.UnaryMinus.ToString();
                    }
                    else if (currentToken.Equals(StringTokeniser.Plus.ToString()))
                    {
                        parameters[i] = StringTokeniser.UnaryPlus.ToString();
                    }
                }
            }

            // See https://en.wikipedia.org/wiki/Shunting-yard_algorithm for more details on this algorithm    
            Stack<Eval> evalStack = new Stack<Eval>();
            Stack<OperatorType> operatorStack = new Stack<OperatorType>();

            foreach (string parameter in parameters)
            {
                if (parameter.Equals(StringTokeniser.Comma))
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
                            ThrowError($"Unable to parse '{stringToken}'", stringToken);
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
                            ThrowError($"'{StringTokeniser.Plus}' can only be used with numbers", stringToken);
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
                            ThrowError($"'{StringTokeniser.UnaryNot}' can only be used with numbers", stringToken);
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
            if (str.Equals(StringTokeniser.Plus.ToString())) return OperatorType.Plus;
            else if (str.Equals(StringTokeniser.Minus.ToString())) return OperatorType.Minus;
            else if (str.Equals(StringTokeniser.Divide.ToString())) return OperatorType.Divide;
            else if (str.Equals(StringTokeniser.Multiply.ToString())) return OperatorType.Multiply;
            else if (str.Equals(StringTokeniser.Modulus.ToString())) return OperatorType.Modulus;
            else if (str.Equals(StringTokeniser.Exponential.ToString())) return OperatorType.Exponential;
            else if (str.Equals(StringTokeniser.UnaryMinus.ToString())) return OperatorType.UnaryMinus;
            else if (str.Equals(StringTokeniser.UnaryPlus.ToString())) return OperatorType.UnaryPlus;
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
            else if (str.Equals(StringTokeniser.StartParenthesis.ToString())) return OperatorType.StartParentheses;
            else if (str.Equals(StringTokeniser.EndParenthesis.ToString())) return OperatorType.EndParentheses;
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