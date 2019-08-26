using System;

namespace Executor
{
    public class Turtle
    {
        public Turtle(float x, float y, float direction)
        {
            this.InitialX = x;
            this.InitialY = y;
            this.InitialDirection = direction;
            this.CenterTurtle();

            this.IsPenDown = true;
            this.IsVisible = true;

            this.ColorA = 255;
            this.ColorR = 0;
            this.ColorG = 0;
            this.ColorB = 0;
        }

        #region Properties

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Direction { get; private set; }

        public float InitialX { get; }
        public float InitialY { get; }
        public float InitialDirection { get; }

        public bool IsPenDown { get; private set; }
        public bool IsVisible { get; private set; }

        public byte ColorA { get; private set; }
        public byte ColorR { get; private set; }
        public byte ColorG { get; private set; }
        public byte ColorB { get; private set; }

        #endregion

        #region Movement and Rotation

        public void RightTurn(float angle)
        {
            this.Direction = CalculateNewDirection(angle);
        }

        public void LeftTurn(float angle)
        {
            this.Direction = CalculateNewDirection(-angle);
        }

        public void Forward(float distance)
        {
            float newX = this.X;
            float newY = this.Y;
            CalculateNewPosition(this.Direction, distance, ref newX, ref newY);
            this.X = newX;
            this.Y = newY;
        }

        public void Backward(float distance)
        {
            float newX = this.X;
            float newY = this.Y;
            float tempDirection = CalculateNewDirection(180);
            CalculateNewPosition(tempDirection, distance, ref newX, ref newY);
            this.X = newX;
            this.Y = newY;
        }

        public void Left(float distance)
        {
            float newX = this.X;
            float newY = this.Y;
            float tempDirection = CalculateNewDirection(-90);
            CalculateNewPosition(tempDirection, distance, ref newX, ref newY);
            this.X = newX;
            this.Y = newY;
        }

        public void Right(float distance)
        {
            float newX = this.X;
            float newY = this.Y;
            float tempDirection = CalculateNewDirection(90);
            CalculateNewPosition(tempDirection, distance, ref newX, ref newY);
            this.X = newX;
            this.Y = newY;
        }

        #region Direction and Position Calculation

        public float CalculateNewDirection(float angle)
        {
            float newDirection = this.Direction;

            newDirection += angle;
            while (newDirection > 359) newDirection -= 360;
            while (newDirection < 0) newDirection += 360;

            return newDirection;
        }

        public void CalculateNewPosition(float angle, float distance, ref float newX, ref float newY)
        {
            newX = this.X;
            newY = this.Y;

            if (angle == 0)
            {
                newY -= (int)Math.Round(distance);
            }
            else if (angle > 0 && angle < 90)
            {
                //int foo = (int)Math.Round((float)Math.Sin((Math.PI / 180) * (float)angle) * (float)distance);
                newX += (int)Math.Round((Math.Sin((Math.PI / 180) * (float)angle) * (float)distance));
                newY -= (int)Math.Round((Math.Cos((Math.PI / 180) * (float)angle) * (float)distance));
            }
            else if (angle == 90)
            {
                newX += (int)Math.Round(distance);
            }
            else if (angle > 90 && angle < 180)
            {
                newY += (int)Math.Round((Math.Sin((Math.PI / 180) * ((float)angle - 90)) * (float)distance));
                newX += (int)Math.Round((Math.Cos((Math.PI / 180) * ((float)angle - 90)) * (float)distance));
            }
            else if (angle == 180)
            {
                newY += (int)Math.Round(distance);
            }
            else if (angle > 180 && angle < 270)
            {
                newX -= (int)Math.Round((Math.Sin((Math.PI / 180) * ((float)angle - 180)) * (float)distance));
                newY += (int)Math.Round((Math.Cos((Math.PI / 180) * ((float)angle - 180)) * (float)distance));
            }
            else if (angle == 270)
            {
                newX -= (int)Math.Round(distance);
            }
            else
            {
                newY -= (int)Math.Round((Math.Sin((Math.PI / 180) * ((float)angle - 270)) * (float)distance));
                newX -= (int)Math.Round((Math.Cos((Math.PI / 180) * ((float)angle - 270)) * (float)distance));
            }
        }

        #endregion

        #endregion

        #region Pen Up/Down

        public void PenUp()
        {
            this.IsPenDown = false;
        }

        public void PenDown()
        {
            this.IsPenDown = true;
        }

        #endregion

        #region Hiding/Showing Turtle

        public void Hide()
        {
            this.IsVisible = false;
        }

        public void Show()
        {
            this.IsVisible = true;
        }

        #endregion

        #region Colors

        public void SetColorA(int colorA)
        {
            this.ColorA = (byte)Math.Max(0, Math.Min(colorA, 255));
        }

        public void SetColorR(int colorR)
        {
            this.ColorR = (byte)Math.Max(0, Math.Min(colorR, 255));
        }

        public void SetColorG(int colorG)
        {
            this.ColorG = (byte)Math.Max(0, Math.Min(colorG, 255));
        }

        public void SetColorB(int colorB)
        {
            this.ColorB = (byte)Math.Max(0, Math.Min(colorB, 255));
        }

        #endregion

        #region Static Positioning

        public void CenterTurtle()
        {
            this.X = this.InitialX;
            this.Y = this.InitialY;
            this.Direction = this.InitialDirection;
        }

        public void SetDirection(float direction)
        {
            this.Direction = direction;
        }

        public void SetX(float x)
        {
            this.X = x;
        }

        public void SetY(float y)
        {
            this.Y = y;
        }

        #endregion
    }
}