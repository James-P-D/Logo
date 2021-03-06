﻿using System;
using System.Collections.Generic;
using LogicalParser.Commands;
using LogicalParser.Objects;
using LogicalParser.Commands.Control;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using LogicalParser.Commands.BooleanEvaluation;
using LogicalParser.Commands.NumberEvaluation;

namespace Executor
{
  public class Executor
  {
    #region Properties

    private ManualResetEvent waitHandle = new ManualResetEvent(initialState: true);

    #endregion

    #region Events and Delegates

    public delegate void AddOutputTextDelegate(string text);

    public event AddOutputTextDelegate AddOutputTextEvent;

    private void AddOutputText(string text)
    {
      AddOutputTextEvent?.Invoke(text);
    }

    public delegate void UpdateDelegate(Turtle turtle, int x1, int y1);

    public event UpdateDelegate UpdateEvent;

    private void Update(Turtle turtle, int x1, int y1)
    {
      Task task = Task.Factory.StartNew(() =>
        {
          UpdateEvent?.Invoke(turtle, x1, y1);
        }
      );
    }

    public delegate void OnEndDelegate();

    public event OnEndDelegate OnEndEvent;

    private void OnEnd()
    {
      Task task = Task.Factory.StartNew(() =>
        {
          OnEndEvent?.Invoke();
        }
      );
    }

    #endregion

    public bool Execute(List<Command> commands, List<LogoObject> objects, Turtle turtle, int depth,
      ref bool breakOut, ref bool continueOut)
    {
      breakOut = false;
      try
      {
        foreach (var command in commands)
        {
          if (!Running)
          {
            return true;
          }

          bool requiresRedraw = false;
#if DEBUG
          AddOutputText($"{GetIndent(depth)}{command}");
#endif
          if (command is NumberAssign)
          {
            (command as NumberAssign).NumberVar.Value = (command as NumberAssign).NumberEval.Value;
          }
          else if (command is BooleanAssign)
          {
            (command as BooleanAssign).BooleanVar.Value = (command as BooleanAssign).BooleanEval.Value;
          }
          else
          {
            var x1 = (int) turtle.X;
            var y1 = (int) turtle.Y;
            var direction1 = turtle.Direction;

            if (command is RightTurn)
            {
              turtle.RightTurn((command as RightTurn).Angle);
              requiresRedraw = true;
            }
            else if (command is LeftTurn)
            {
              turtle.LeftTurn((command as LeftTurn).Angle);
              requiresRedraw = true;
            }
            else if (command is Forward)
            {
              turtle.Forward((command as Forward).Distance);
              requiresRedraw = true;
            }
            else if (command is Backward)
            {
              turtle.Backward((command as Backward).Distance);
              requiresRedraw = true;
            }
            else if (command is Left)
            {
              turtle.Left((command as Left).Distance);
              requiresRedraw = true;
            }
            else if (command is Right)
            {
              turtle.Right((command as Right).Distance);
              requiresRedraw = true;
            }
            else if (command is PenUp)
            {
              turtle.PenUp();
            }
            else if (command is PenDown)
            {
              turtle.PenDown();
            }
            else if (command is SetColorA)
            {
              turtle.SetColorA((command as SetColorA).A);
              requiresRedraw = true;
            }
            else if (command is SetColorR)
            {
              turtle.SetColorR((command as SetColorR).R);
              requiresRedraw = true;
            }
            else if (command is SetColorG)
            {
              turtle.SetColorG((command as SetColorG).G);
              requiresRedraw = true;
            }
            else if (command is SetColorB)
            {
              turtle.SetColorB((command as SetColorB).B);
              requiresRedraw = true;
            }
            else if (command is HideTurtle)
            {
              turtle.Hide();
              requiresRedraw = true;
            }
            else if (command is ShowTurtle)
            {
              turtle.Show();
              requiresRedraw = true;
            }
            else if (command is CenterTurtle)
            {
              turtle.CenterTurtle();

              // Reset x1 and x2 otherwise we will draw a line between
              // the current position of the turtle, and the initial
              // position before we returned to center.
              x1 = (int) turtle.X;
              y1 = (int) turtle.Y;
              direction1 = turtle.Direction;
              requiresRedraw = true;
            }
            else if (command is SetDirection)
            {
              turtle.SetDirection((command as SetDirection).Direction);
              requiresRedraw = true;
            }
            else if (command is SetX)
            {
              turtle.SetX((command as SetX).X);

              // Reset x1 and x2 otherwise we will draw a line between
              // the current position of the turtle, and the initial
              // position before we set X.
              x1 = (int) turtle.X;
              y1 = (int) turtle.Y;
              direction1 = turtle.Direction;
              requiresRedraw = true;
            }
            else if (command is SetY)
            {
              turtle.SetY((command as SetY).Y);

              // Reset x1 and x2 otherwise we will draw a line between
              // the current position of the turtle, and the initial
              // position before we set X.
              x1 = (int) turtle.X;
              y1 = (int) turtle.Y;
              direction1 = turtle.Direction;
              requiresRedraw = true;
            }
            else if (command is Output)
            {
              var eval = (command as Output).Value;
              if (eval is NumberValueEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberValueEval).Value}");
              }
              else if (eval is NumberPlusEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberPlusEval).Value}");
              }
              else if (eval is NumberMinusEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberMinusEval).Value}");
              }
              else if (eval is NumberMultiplyEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberMultiplyEval).Value}");
              }
              else if (eval is NumberDivideEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberDivideEval).Value}");
              }
              else if (eval is NumberExponentialEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberExponentialEval).Value}");
              }
              else if (eval is NumberModulusEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberModulusEval).Value}");
              }
              else if (eval is NumberUnaryMinusEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberUnaryMinusEval).Value}");
              }
              else if (eval is NumberUnaryPlusEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberUnaryPlusEval).Value}");
              }
              else if (eval is BooleanValueEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as BooleanValueEval).Value}");
              }
              else if (eval is BooleanUnaryNotEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as BooleanUnaryNotEval).Value}");
              }
              else if (eval is BooleanAndEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as BooleanAndEval).Value}");
              }
              else if (eval is BooleanOrEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as BooleanOrEval).Value}");
              }
              else if (eval is BooleanXorEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as BooleanXorEval).Value}");
              }
              else if (eval is BooleanBooleanEqualityEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as BooleanBooleanEqualityEval).Value}");
              }
              else if (eval is BooleanNumberEqualityEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as BooleanNumberEqualityEval).Value}");
              }
              else if (eval is BooleanBooleanInequalityEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as BooleanBooleanInequalityEval).Value}");
              }
              else if (eval is BooleanNumberInequalityEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as BooleanNumberInequalityEval).Value}");
              }
              else if (eval is BooleanNumberGreaterThanEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as BooleanNumberGreaterThanEval).Value}");
              }
              else if (eval is BooleanNumberLessThanEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as BooleanNumberLessThanEval).Value}");
              }
              else if (eval is BooleanNumberGreaterThanOrEqualEval)
              {
                AddOutputText(
                  $"OUTPUT: {(command as Output).Name} = {(eval as BooleanNumberGreaterThanOrEqualEval).Value}");
              }
              else if (eval is BooleanNumberLessThanOrEqualEval)
              {
                AddOutputText(
                  $"OUTPUT: {(command as Output).Name} = {(eval as BooleanNumberLessThanOrEqualEval).Value}");
              }
              else if (eval is NumberUnarySinEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberUnarySinEval).Value}");
              }
              else if (eval is NumberUnaryCosEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberUnaryCosEval).Value}");
              }
              else if (eval is NumberUnaryTanEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberUnaryTanEval).Value}");
              }
              else if (eval is NumberMinEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberMinEval).Value}");
              }
              else if (eval is NumberMaxEval)
              {
                AddOutputText($"OUTPUT: {(command as Output).Name} = {(eval as NumberMaxEval).Value}");
              }
              else
              {
                // More eval types need to be added here (unless we fix .Value ....)
                ThrowError($"Unable to output data-type {command}", command);
              }
            }
            else if (command is Repeat)
            {
              var repeatCount = (command as Repeat).Counter;
              for (var i = 0; i < repeatCount; i++)
              {
                var repeatBreak = false;
                var repeatContinue = false;
                if (!Execute((command as Repeat).Commands, objects, turtle, depth + 2, ref repeatBreak,
                  ref repeatContinue))
                {
                  return false;
                }

                if (repeatBreak)
                {
                  return true;
                }
              }

              // Reset x1 and x2 otherwise we will draw a line between
              // the current position of the turtle, and the initial
              // position before the Repeat loop was executed.
              x1 = (int) turtle.X;
              y1 = (int) turtle.Y;
              direction1 = turtle.Direction;
            }
            else if (command is While)
            {
              while ((command as While).Value)
              {
                var whileBreak = false;
                var whileContinue = false;
                if (!Execute((command as While).Commands, objects, turtle, depth + 2, ref whileBreak,
                  ref whileContinue))
                {
                  return false;
                }

                if (whileBreak)
                {
                  return true;
                }
              }

              // Reset x1 and x2 otherwise we will draw a line between
              // the current position of the turtle, and the initial
              // position before the Repeat loop was executed.
              x1 = (int) turtle.X;
              y1 = (int) turtle.Y;
              direction1 = turtle.Direction;
            }
            else if (command is If)
            {
              if ((command as If).Value)
              {
                var ifBreak = false;
                var ifContinue = false;
                if (!Execute((command as If).ThenCommands, objects, turtle, depth + 2, ref ifBreak,
                  ref ifContinue))
                {
                  return false;
                }

                if (ifBreak)
                {
                  // If break was fired in the IF, then we need to break out of whatever we are in 
                  breakOut = true;
                  return true;
                }

                if (ifContinue)
                {
                  continueOut = true;
                  return true;
                }
              }
              else
              {
                var elseBreak = false;
                var elseContinue = false;
                if (!Execute((command as If).ElseCommands, objects, turtle, depth + 2, ref elseBreak,
                  ref elseContinue))
                {
                  return false;
                }

                if (elseBreak)
                {
                  // If break was fired in the IF, then we need to break out of whatever we are in 
                  breakOut = true;
                  return true;
                }

                if (elseContinue)
                {
                  continueOut = true;
                  return true;
                }
              }

              // Reset x1 and x2 otherwise we will draw a line between
              // the current position of the turtle, and the initial
              // position before the Repeat loop was executed.
              x1 = (int) turtle.X;
              y1 = (int) turtle.Y;
              direction1 = turtle.Direction;
            }
            else if (command is Break)
            {
              breakOut = true;
              return true;
            }
            else if (command is Continue)
            {
              continueOut = true;
              return true;
            }
            else
            {
              ThrowError($"Don't recognise command '{command}'", command);
            }

            if (requiresRedraw)
            {
              Update(turtle, x1, y1);
              this.PauseThread();
            }
          }
        }

        return true;
      }
      catch (Exception ex)
      {
        AddOutputText("ERROR: " + ex.Message);
        return false;
      }
      finally
      {
        if (depth == 0)
        {
          this.Running = false;
          this.OnEnd();
        }
      }
    }

    private void PauseThread()
    {
      this.waitHandle.Reset();
      this.waitHandle.WaitOne();
    }

    public void ResumeThread()
    {
      this.waitHandle.Set();
    }
    
    public bool Running { get; set; }

    private string GetIndent(int indent)
    {
      var result = string.Empty;
      for (var i = 0; i < indent; i++) result += " ";
      return result;
    }

    #region Errors

    private void ThrowError(string message, Command command)
    {
      throw new Exception($"{message} - {command.ToString()}");
    }

    #endregion
  }
}