using System;

namespace Ping_PongClassLibrary
{
    public class Paddle : IPaddle
    {
        public double X { get; set; }
        public double Y { get; set; }
        private double _width;
        private double _height;
        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                VisibleWidth = _width * 0.9;
                AdjustPositionAfterWidthChange();
            }
        }
        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                VisibleHeight = _height * 0.9;
                AdjustPositionAfterHeightChange();
            }
        }
        public double VisibleWidth { get; set; }
        public double VisibleHeight { get; set; }
        public double Vy { get; set; }
        public bool IsStriking { get; set; }
        public double BounceModifier { get; set; }
        private readonly double screenWidth;
        private readonly double screenHeight;
        public double SpeedModifier { get; set; }
        private readonly double tableTop;
        private readonly double tableBottom;
        private readonly double tableLeft;
        private readonly double tableRight;
        private double originalX;
        private double strikeTime;
        private const double StrikeDuration = 0.3;
        public static double StrikeDistance { get; } = 70;

        public Paddle(double x, double y, double width, double height,
                      double tableTop, double tableBottom, double tableLeft, double tableRight,
                      double screenWidth, double screenHeight)
        {
            X = x;
            originalX = x;
            Y = y;
            _width = width;
            _height = height;
            VisibleWidth = width * 0.9;
            VisibleHeight = height * 0.9;
            this.tableTop = tableTop;
            this.tableBottom = tableBottom;
            this.tableLeft = tableLeft;
            this.tableRight = tableRight;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            IsStriking = false;
            BounceModifier = 1.0;
            SpeedModifier = 1.0;
            strikeTime = 0;
        }

        private void AdjustPositionAfterWidthChange()
        {
            double overlap = 10;
            if (originalX < screenWidth / 2) // Игрок 1 (слева)
            {
                X = tableLeft - VisibleWidth + overlap;
                originalX = tableLeft - VisibleWidth + overlap;
            }
            else // Игрок 2 (справа)
            {
                X = tableRight - (Width - VisibleWidth) - overlap;
                originalX = tableRight - (Width - VisibleWidth) - overlap;
            }
        }

        private void AdjustPositionAfterHeightChange()
        {
            Y = Math.Max(Height / 2, Math.Min(screenHeight - Height / 2, Y));
        }

        public void UpdateVelocity(double targetY, double deltaTime)
        {
            double speed = 600.0;
            double newY = Y;

            if (targetY > Y)
            {
                newY = Y + speed * deltaTime;
            }
            else if (targetY < Y)
            {
                newY = Y - speed * deltaTime;
            }

            newY = Math.Max(Height / 2, Math.Min(screenHeight - Height / 2, newY));
            Vy = (newY - Y) / deltaTime;
            Y = newY;

            if (strikeTime > 0)
            {
                strikeTime -= deltaTime;
                if (strikeTime <= 0)
                {
                    X = originalX;
                    IsStriking = false;
                }
                else
                {
                    double progress = 1 - (strikeTime / StrikeDuration);
                    double direction = (originalX < screenWidth / 2) ? 1 : -1;
                    double offset = StrikeDistance * progress * direction;
                    double newX = originalX + offset;
                    double overlap = 10;

                    if (direction > 0)
                    {
                        newX = Math.Min(tableLeft - VisibleWidth / 2 + overlap, newX);
                    }
                    else
                    {
                        newX = Math.Max(tableRight - (Width - VisibleWidth / 2) - overlap, newX);
                    }
                    X = newX;
                }
            }
        }

        public void Strike()
        {
            if (!IsStriking)
            {
                IsStriking = true;
                strikeTime = StrikeDuration;
            }
        }
    }
}