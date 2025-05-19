using System;

namespace Ping_PongClassLibrary
{
    public class Paddle : IPaddle
    {
        private const double VisiblePartRatio = 0.9;
        private const double Overlap = 10;
        private const double BaseSpeed = 600.0;
        private const double StrikeDuration = 0.3;
        private const double StrikeDistance = 70.0;
        private const double BaseMass = 0.15;

        private double _width;
        private double _height;
        private double _mass = BaseMass;
        private double _speedModifier = 1.0;
        private double _bounceModifier = 1.0;
        private double _strikeTime;
        private double _originalX;

        public double X { get; private set; }
        public double Y { get; set; }
        public double Vy { get; private set; }
        public double SpeedModifier
        {
            get => _speedModifier;
            set => _speedModifier = value;
        }
        public double BounceModifier
        {
            get => _bounceModifier;
            set => _bounceModifier = value;
        }
        public double Mass
        {
            get => _mass;
            set => _mass = value;
        }
        public double BaseHeight { get; } = 120;
        public bool IsStriking { get; private set; }

        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                VisibleWidth = _width * VisiblePartRatio;
                AdjustPositionAfterWidthChange();
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                VisibleHeight = _height * VisiblePartRatio;
                AdjustPositionAfterHeightChange();
            }
        }

        public double VisibleWidth { get; private set; }
        public double VisibleHeight { get; private set; }

        private readonly double _screenWidth;
        private readonly double _screenHeight;
        private readonly double _tableLeft;
        private readonly double _tableRight;
        private readonly double _tableTop;
        private readonly double _tableBottom;

        /// <summary>
        /// Инициализирует новый экземпляр ракетки с заданными параметрами.
        /// </summary>
        public Paddle(double x, double y, double width, double height,
                     double tableTop, double tableBottom, double tableLeft, double tableRight,
                     double screenWidth, double screenHeight)
        {
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _tableLeft = tableLeft;
            _tableRight = tableRight;
            _tableTop = tableTop;
            _tableBottom = tableBottom;

            X = x;
            _originalX = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Обновляет скорость и позицию ракетки на основе целевой позиции и времени.
        /// </summary>
        public void UpdateVelocity(double targetY, double deltaTime)
        {
            UpdateVerticalPosition(targetY, deltaTime);
            UpdateStrikeMovement(deltaTime);
        }

        /// <summary>
        /// Обновляет вертикальную позицию ракетки, перемещая её к целевой Y-координате.
        /// </summary>
        private void UpdateVerticalPosition(double targetY, double deltaTime)
        {
            double newY = CalculateNewYPosition(targetY, deltaTime);
            newY = ClampYPosition(newY);

            Vy = (newY - Y) / deltaTime;
            Y = newY;
        }

        /// <summary>
        /// Вычисляет новую Y-координату ракетки на основе целевой позиции и скорости.
        /// </summary>
        private double CalculateNewYPosition(double targetY, double deltaTime)
        {
            if (targetY > Y)
                return Y + BaseSpeed * deltaTime;
            if (targetY < Y)
                return Y - BaseSpeed * deltaTime;
            return Y;
        }

        /// <summary>
        /// Ограничивает Y-координату ракетки, чтобы она не выходила за пределы экрана.
        /// </summary>
        private double ClampYPosition(double y)
        {
            double minY = Height / 2;
            double maxY = _screenHeight - Height / 2;
            return Math.Min(Math.Max(y, minY), maxY);
        }

        /// <summary>
        /// Обновляет движение ракетки во время анимации удара.
        /// </summary>
        private void UpdateStrikeMovement(double deltaTime)
        {
            if (!IsStriking) return;

            _strikeTime -= deltaTime;

            if (_strikeTime <= 0)
            {
                ResetStrike();
            }
            else
            {
                UpdateStrikePosition();
            }
        }

        /// <summary>
        /// Сбрасывает состояние удара, возвращая ракетку в исходную позицию.
        /// </summary>
        private void ResetStrike()
        {
            X = _originalX;
            IsStriking = false;
        }

        /// <summary>
        /// Обновляет X-координату ракетки во время анимации удара.
        /// </summary>
        private void UpdateStrikePosition()
        {
            double progress = 1 - (_strikeTime / StrikeDuration);
            double direction = IsLeftPaddle ? 1 : -1;
            double offset = StrikeDistance * Math.Sin(progress * Math.PI) * direction;

            X = CalculateNewXPosition(_originalX + offset, direction);
        }

        /// <summary>
        /// Вычисляет новую X-координату ракетки с учетом направления удара и границ стола.
        /// </summary>
        private double CalculateNewXPosition(double newX, double direction)
        {
            if (direction > 0)
            {
                return Math.Min(_tableLeft - VisibleWidth / 2 + Overlap, newX);
            }
            return Math.Max(_tableRight - (Width - VisibleWidth / 2) - Overlap, newX);
        }

        /// <summary>
        /// Запускает анимацию удара ракеткой, если она ещё не активна.
        /// </summary>
        public void Strike()
        {
            if (IsStriking) return;

            IsStriking = true;
            _strikeTime = StrikeDuration;
        }

        private bool IsLeftPaddle => _originalX < _screenWidth / 2;

        /// <summary>
        /// Корректирует X-координату ракетки после изменения её ширины.
        /// </summary>
        public void AdjustPositionAfterWidthChange()
        {
            if (IsLeftPaddle)
            {
                X = _tableLeft - VisibleWidth + Overlap;
                _originalX = X;
            }
            else
            {
                X = _tableRight - (Width - VisibleWidth) - Overlap;
                _originalX = X;
            }
        }

        /// <summary>
        /// Корректирует Y-координату ракетки после изменения её высоты.
        /// </summary>
        public void AdjustPositionAfterHeightChange()
        {
            Y = ClampYPosition(Y);
        }

        /// <summary>
        /// Обновляет размеры ракетки.
        /// </summary>
        public void UpdateSize(double newWidth, double newHeight)
        {
            Width = newWidth;
            Height = newHeight;
        }

        /// <summary>
        /// Обновляет физические параметры ракетки.
        /// </summary>
        public void UpdatePhysics(double newMass, double newSpeedModifier, double newBounceModifier)
        {
            Mass = newMass;
            SpeedModifier = newSpeedModifier;
            BounceModifier = newBounceModifier;
        }
    }
}