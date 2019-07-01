using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicalParser.Commands;
using LogicalParser.Objects;
using System.Drawing;
using LogicalParser.Commands.Evaluation;
using LogicalParser.Commands.Control;
using System.ComponentModel;
using System.Threading;

namespace Executor
{
    public class Executor
    {
        #region Events and Delegates

        public delegate void AddOutputTextDelegate(string text);
        public event AddOutputTextDelegate AddOutputTextEvent;

        private void AddOutputText(string text)
        {
            if (this.AddOutputTextEvent != null)
            {
                this.AddOutputTextEvent.Invoke(text);
            }
        }

        public delegate void UpdateDelegate(Turtle turtle, int x1, int y1);
        public event UpdateDelegate UpdateEvent;

        private void Update(Turtle turtle, int x1, int y1)
        {
            if (this.UpdateEvent != null)
            {
                this.UpdateEvent(turtle, x1, y1);
            }
        }

        #endregion

        public bool Execute(object sender, List<Command> commands, List<LogoObject> objects, Turtle turtle, int indent, ref bool breakOut, ref bool continueOut)
        {
            breakOut = false;
            try
            {
                foreach (Command command in commands)
                {
                    //AddOutputText(string.Format("{0}{1}", GetIndent(indent), command.ToString()));
                    Thread.Sleep(1);

                    if (!this.Running)
                    {
                        return true;
                    }

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
                        int x1 = (int)turtle.X;
                        int y1 = (int)turtle.Y;
                        float direction1 = turtle.Direction;

                        if (command is RightTurn)
                        {
                            turtle.RightTurn((command as RightTurn).Angle);
                        }
                        else if (command is LeftTurn)
                        {
                            turtle.LeftTurn((command as LeftTurn).Angle);
                        }
                        else if (command is Forward)
                        {
                            turtle.Forward((command as Forward).Distance);
                        }
                        else if (command is Backward)
                        {
                            turtle.Backward((command as Backward).Distance);
                        }
                        else if (command is Left)
                        {
                            turtle.Left((command as Left).Distance);
                        }
                        else if (command is Right)
                        {
                            turtle.Right((command as Right).Distance);
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
                        }
                        else if (command is SetColorR)
                        {
                            turtle.SetColorR((command as SetColorR).R);
                        }
                        else if (command is SetColorG)
                        {
                            turtle.SetColorG((command as SetColorG).G);
                        }
                        else if (command is SetColorB)
                        {
                            turtle.SetColorB((command as SetColorB).B);
                        }
                        else if (command is HideTurtle)
                        {
                            turtle.Hide();
                        }
                        else if (command is ShowTurtle)
                        {
                            turtle.Show();
                        }
                        else if (command is CenterTurtle)
                        {
                            turtle.CenterTurtle();

                            // Reset x1 and x2 otherwise we will draw a line between
                            // the current position of the turtle, and the initial
                            // position before we returned to center.
                            x1 = (int)turtle.X;
                            y1 = (int)turtle.Y;
                            direction1 = turtle.Direction;
                        }
                        else if (command is SetDirection)
                        {
                            turtle.SetDirection((command as SetDirection).Direction);
                        }
                        else if (command is SetX)
                        {
                            turtle.SetX((command as SetX).X);

                            // Reset x1 and x2 otherwise we will draw a line between
                            // the current position of the turtle, and the initial
                            // position before we set X.
                            x1 = (int)turtle.X;
                            y1 = (int)turtle.Y;
                            direction1 = turtle.Direction;
                        }
                        else if (command is SetY)
                        {
                            turtle.SetY((command as SetY).Y);

                            // Reset x1 and x2 otherwise we will draw a line between
                            // the current position of the turtle, and the initial
                            // position before we set X.
                            x1 = (int)turtle.X;
                            y1 = (int)turtle.Y;
                            direction1 = turtle.Direction;
                        }
                        else if (command is Output)
                        {
                            Eval eval = (command as Output).Value;
                            if (eval is NumberValueEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberValueEval).Value));
                            }
                            else if (eval is NumberPlusEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberPlusEval).Value));
                            }
                            else if (eval is NumberMinusEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberMinusEval).Value));
                            }
                            else if (eval is NumberMultiplyEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberMultiplyEval).Value));
                            }
                            else if (eval is NumberDivideEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberDivideEval).Value));
                            }
                            else if (eval is NumberExponentialEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberExponentialEval).Value));
                            }
                            else if (eval is NumberModulusEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberModulusEval).Value));
                            }
                            else if (eval is NumberUnaryMinusEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberUnaryMinusEval).Value));
                            }
                            else if (eval is NumberUnaryPlusEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberUnaryPlusEval).Value));
                            }
                            else if (eval is BooleanValueEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as BooleanValueEval).Value));
                            }
                            else if (eval is BooleanUnaryNotEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as BooleanUnaryNotEval).Value));
                            }
                            else if (eval is BooleanAndEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as BooleanAndEval).Value));
                            }
                            else if (eval is BooleanOrEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as BooleanOrEval).Value));
                            }
                            else if (eval is BooleanXorEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as BooleanXorEval).Value));
                            }
                            else if (eval is BooleanBooleanEqualityEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as BooleanBooleanEqualityEval).Value));
                            }
                            else if (eval is BooleanNumberEqualityEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as BooleanNumberEqualityEval).Value));
                            }
                            else if (eval is BooleanBooleanInequalityEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as BooleanBooleanInequalityEval).Value));
                            }
                            else if (eval is BooleanNumberInequalityEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as BooleanNumberInequalityEval).Value));
                            }
                            else if (eval is BooleanNumberGreaterThanEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as BooleanNumberGreaterThanEval).Value));
                            }
                            else if (eval is BooleanNumberLessThanEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as BooleanNumberLessThanEval).Value));
                            }
                            else if (eval is BooleanNumberGreaterThanOrEqualEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as BooleanNumberGreaterThanOrEqualEval).Value));
                            }
                            else if (eval is BooleanNumberLessThanOrEqualEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as BooleanNumberLessThanOrEqualEval).Value));
                            }
                            else if (eval is NumberUnarySinEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberUnarySinEval).Value));
                            }
                            else if (eval is NumberUnaryCosEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberUnaryCosEval).Value));
                            }
                            else if (eval is NumberUnaryTanEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberUnaryTanEval).Value));
                            }
                            else if (eval is NumberMinEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberMinEval).Value));
                            }
                            else if (eval is NumberMaxEval)
                            {
                                AddOutputText(string.Format("OUTPUT: {0} = {1}", (command as Output).Name, (eval as NumberMaxEval).Value));
                            }
                            else
                            {
                                // More eval types need to be added here :(
                                throw new Exception("Unable to parse!!!");
                            }
                        }
                        else if (command is Repeat)
                        {
                            int repeatCount = (command as Repeat).Counter;
                            for (int i = 0; i < repeatCount; i++)
                            {
                                bool repeatBreak = false;
                                bool repeatContinue = false;
                                if (!Execute(sender, (command as Repeat).commands, objects, turtle, indent + 2, ref repeatBreak, ref repeatContinue))
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
                            x1 = (int)turtle.X;
                            y1 = (int)turtle.Y;
                            direction1 = turtle.Direction;
                        }
                        else if (command is While)
                        {
                            while ((command as While).Value)
                            {
                                bool whileBreak = false;
                                bool whileContinue = false;
                                if (!Execute(sender, (command as While).commands, objects, turtle, indent + 2, ref whileBreak, ref whileContinue))
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
                            x1 = (int)turtle.X;
                            y1 = (int)turtle.Y;
                            direction1 = turtle.Direction;
                        }
                        else if (command is If)
                        {
                            if ((command as If).Value)
                            {
                                bool ifBreak = false;
                                bool ifContinue = false;
                                if (!Execute(sender, (command as If).thenCommands, objects, turtle, indent + 2, ref ifBreak, ref ifContinue))
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
                                bool elseBreak = false;
                                bool elseContinue = false;
                                if (!Execute(sender, (command as If).elseCommands, objects, turtle, indent + 2, ref elseBreak, ref elseContinue))
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
                            x1 = (int)turtle.X;
                            y1 = (int)turtle.Y;
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
                            throw new Exception(string.Format("Don't recognise command '{0}'", command));
                        }

                        var foo = sender;
                        (sender as BackgroundWorker).ReportProgress(0, new object[] { turtle, x1, y1 });
                        
                        //TODO: Do we need this?
                        //Update(turtle, x1, y1);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                AddOutputText("ERROR: " + ex.Message);
                return false;
            }
        }

        public bool Running { get; set; }

        private string GetIndent(int indent)
        {
            string result = string.Empty;
            for (int i = 0; i < indent; i++) result += " ";
            return result;
        }

    }
}