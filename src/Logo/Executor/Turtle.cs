using System;

namespace Executor
{
    public class Turtle
    {
        public Turtle(float x, float y, float direction)
        {
            InitialX = x;
            InitialY = y;
            InitialDirection = direction;
            CenterTurtle();

            IsPenDown = true;
            IsVisible = true;

            ColorA = 255;
            ColorR = 0;
            ColorG = 0;
            ColorB = 0;
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
            Direction = CalculateNewDirection(angle);
        }

        public void LeftTurn(float angle)
        {
            Direction = CalculateNewDirection(-angle);
        }

        public void Forward(float distance)
        {
            var newX = X;
            var newY = Y;
            CalculateNewPosition(Direction, distance, ref newX, ref newY);
            X = newX;
            Y = newY;
        }

        public void Backward(float distance)
        {
            var newX = X;
            var newY = Y;
            var tempDirection = CalculateNewDirection(180);
            CalculateNewPosition(tempDirection, distance, ref newX, ref newY);
            X = newX;
            Y = newY;
        }

        public void Left(float distance)
        {
            var newX = X;
            var newY = Y;
            var tempDirection = CalculateNewDirection(-90);
            CalculateNewPosition(tempDirection, distance, ref newX, ref newY);
            X = newX;
            Y = newY;
        }

        public void Right(float distance)
        {
            var newX = X;
            var newY = Y;
            var tempDirection = CalculateNewDirection(90);
            CalculateNewPosition(tempDirection, distance, ref newX, ref newY);
            X = newX;
            Y = newY;
        }

        #region Direction and Position Calculation

        public float CalculateNewDirection(float angle)
        {
            var newDirection = Direction;

            newDirection += angle;
            while (newDirection > 359) newDirection -= 360;
            while (newDirection < 0) newDirection += 360;

            return newDirection;
        }

        public void CalculateNewPosition(float angle, float distance, ref float newX, ref float newY)
        {
            newX = X;
            newY = Y;

            if (angle == 0)
            {
                newY -= (int)Math.Round(distance);
            }
            else if (angle > 0 && angle < 90)
            {
                //int foo = (int)Math.Round((float)Math.Sin((Math.PI / 180) * (float)angle) * (float)distance);
                newX += (int)Math.Round(Math.Sin(Math.PI / 180 * (float)angle) * (float)distance);
                newY -= (int)Math.Round(Math.Cos(Math.PI / 180 * (float)angle) * (float)distance);
            }
            else if (angle == 90)
            {
                newX += (int)Math.Round(distance);
            }
            else if (angle > 90 && angle < 180)
            {
                newY += (int)Math.Round(Math.Sin(Math.PI / 180 * ((float)angle - 90)) * (float)distance);
                newX += (int)Math.Round(Math.Cos(Math.PI / 180 * ((float)angle - 90)) * (float)distance);
            }
            else if (angle == 180)
            {
                newY += (int)Math.Round(distance);
            }
            else if (angle > 180 && angle < 270)
            {
                newX -= (int)Math.Round(Math.Sin(Math.PI / 180 * ((float)angle - 180)) * (float)distance);
                newY += (int)Math.Round(Math.Cos(Math.PI / 180 * ((float)angle - 180)) * (float)distance);
            }
            else if (angle == 270)
            {
                newX -= (int)Math.Round(distance);
            }
            else
            {
                newY -= (int)Math.Round(Math.Sin(Math.PI / 180 * ((float)angle - 270)) * (float)distance);
                newX -= (int)Math.Round(Math.Cos(Math.PI / 180 * ((float)angle - 270)) * (float)distance);
            }
        }

        #endregion

        #endregion

        #region Pen Up/Down

        public void PenUp()
        {
            IsPenDown = false;
        }

        public void PenDown()
        {
            IsPenDown = true;
        }

        #endregion

        #region Hiding/Showing Turtle

        public void Hide()
        {
            IsVisible = false;
        }

        public void Show()
        {
            IsVisible = true;
        }

        #endregion

        #region Colors

        public void SetColorA(int colorA)
        {
            ColorA = (byte)Math.Max(0, Math.Min(colorA, 255));
        }

        public void SetColorR(int colorR)
        {
            ColorR = (byte)Math.Max(0, Math.Min(colorR, 255));
        }

        public void SetColorG(int colorG)
        {
            ColorG = (byte)Math.Max(0, Math.Min(colorG, 255));
        }

        public void SetColorB(int colorB)
        {
            ColorB = (byte)Math.Max(0, Math.Min(colorB, 255));
        }

        #endregion

        #region Static Positioning

        public void CenterTurtle()
        {
            X = InitialX;
            Y = InitialY;
            Direction = InitialDirection;
        }

        public void SetDirection(float direction)
        {
            Direction = direction;
        }

        public void SetX(float x)
        {
            X = x;
        }

        public void SetY(float y)
        {
            Y = y;
        }

        #endregion
    }
}