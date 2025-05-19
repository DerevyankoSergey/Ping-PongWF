using System;

namespace Ping_PongClassLibrary
{
    public class BallAnimation
    {
        public float ScaleX { get; private set; } = 1.0f;
        public float ScaleY { get; private set; } = 1.0f;
        public bool IsImpactAnimationActive { get; private set; }
        public double ImpactX { get; private set; }
        public double ImpactY { get; private set; }
        public int ImpactFrame => IsImpactAnimationActive
            ? Math.Min(ImpactFrameCount - 1, (int)((ImpactAnimationDuration - impactAnimationTime) / ImpactAnimationDuration * ImpactFrameCount))
            : 0;

        private double collisionAnimationTime;
        private double flightAnimationTime;
        private double impactAnimationTime;

        private const double CollisionAnimationDuration = 0.15;
        private const double FlightAnimationDuration = 0.5;
        private const float MinScale = 0.8f;
        private const float MaxFlightScale = 1.4f;
        private const float AnimationSpeedFactor = 1.0f;
        private const double ImpactAnimationDuration = 0.3;
        private const int ImpactFrameCount = 4;

        /// <summary>
        /// Обновляет анимацию мяча на каждом игровом цикле, включая анимацию столкновения, полета и удара о стол.
        /// </summary>
        public void Update(double deltaTime, BallPhysics physics, int tableLeft, int tableRight)
        {
            if (collisionAnimationTime > 0)
            {
                collisionAnimationTime -= deltaTime;
                float progress = (float)(collisionAnimationTime / CollisionAnimationDuration);
                ScaleX = ScaleY = MinScale + (1.0f - MinScale) * progress;
                if (collisionAnimationTime <= 0)
                {
                    ScaleX = ScaleY = 1.0f;
                    flightAnimationTime = 0.0;
                }
            }

            if (physics.HasCollidedWithPaddle && collisionAnimationTime <= 0)
            {
                flightAnimationTime += deltaTime;
                UpdateFlightAnimation(physics, tableLeft, tableRight);
            }

            UpdateImpactAnimation(deltaTime, physics);
        }

        /// <summary>
        /// Обновляет анимацию полета мяча, изменяя его масштаб в зависимости от положения относительно стола.
        /// </summary>
        private void UpdateFlightAnimation(BallPhysics physics, int tableLeft, int tableRight)
        {
            if (physics.Vx == 0) return;

            bool isMovingLeft = physics.Vx < 0;
            double tableMidpoint = (tableLeft + tableRight) / 2.0;

            double startX = isMovingLeft ? tableRight : tableLeft;
            double endX = isMovingLeft ? tableLeft : tableRight;

            if (!physics.IsOverNet)
            {
                ScaleX = ScaleY = 1.0f;
                return;
            }

            double minSpeed = 600;
            double maxSpeed = 600 * 1.5;
            double currentSpeed = Math.Abs(physics.Vx);
            double speedFactor = (currentSpeed - minSpeed) / (maxSpeed - minSpeed);
            speedFactor = Math.Max(0, Math.Min(1, speedFactor));

            double midPointRange = Math.Abs(endX - tableMidpoint);
            double minOffsetFactor = 0.4;
            double maxOffsetFactor = 0.7;
            double offset = midPointRange * (minOffsetFactor + (maxOffsetFactor - minOffsetFactor) * speedFactor);
            double adjustedMidX = isMovingLeft
                ? tableMidpoint - offset
                : tableMidpoint + offset;

            adjustedMidX = isMovingLeft
                ? Math.Max(endX, Math.Min(tableMidpoint, adjustedMidX))
                : Math.Min(endX, Math.Max(tableMidpoint, adjustedMidX));

            float progressFromNetToMid = 0f;
            float progressFromMidToEnd = 0f;

            if (isMovingLeft)
            {
                progressFromNetToMid = (float)Math.Max(0, (tableMidpoint - physics.X) / (tableMidpoint - adjustedMidX));
                progressFromMidToEnd = (float)Math.Max(0, (adjustedMidX - physics.X) / (adjustedMidX - endX));
            }
            else
            {
                progressFromNetToMid = (float)Math.Max(0, (physics.X - tableMidpoint) / (adjustedMidX - tableMidpoint));
                progressFromMidToEnd = (float)Math.Max(0, (physics.X - adjustedMidX) / (endX - adjustedMidX));
            }

            progressFromNetToMid = SmoothStep(Math.Min(1.0f, progressFromNetToMid * AnimationSpeedFactor));
            progressFromMidToEnd = SmoothStep(Math.Min(1.0f, progressFromMidToEnd * AnimationSpeedFactor));

            if (progressFromNetToMid < 1.0f)
            {
                ScaleX = ScaleY = 1.0f - (1.0f - MinScale) * progressFromNetToMid;
            }
            else if (physics.HasTouchedOpponentTable)
            {
                ScaleX = ScaleY = MinScale + (MaxFlightScale - MinScale) * progressFromMidToEnd;
                if (Math.Abs(progressFromNetToMid - 1.0f) < 0.001f && !IsImpactAnimationActive)
                {
                    StartImpactAnimation(physics.X, physics.Y);
                }
            }
            else
            {
                ScaleX = ScaleY = MinScale;
            }

            ScaleX = ScaleY = Math.Max(MinScale, Math.Min(MaxFlightScale, ScaleX));
        }

        /// <summary>
        /// Применяет функцию плавного перехода (SmoothStep) для сглаживания изменения масштаба.
        /// </summary>
        private float SmoothStep(float t)
        {
            return t * t * (3 - 2 * t);
        }

        /// <summary>
        /// Запускает анимацию удара о стол, фиксируя координаты точки удара.
        /// </summary>
        private void StartImpactAnimation(double x, double y)
        {
            IsImpactAnimationActive = true;
            impactAnimationTime = ImpactAnimationDuration;
            ImpactX = x;
            ImpactY = y;
        }

        /// <summary>
        /// Обновляет состояние анимации удара о стол, уменьшая время анимации.
        /// </summary>
        private void UpdateImpactAnimation(double deltaTime, BallPhysics physics)
        {
            if (!IsImpactAnimationActive) return;

            impactAnimationTime -= deltaTime;
            if (impactAnimationTime <= 0)
            {
                IsImpactAnimationActive = false;
                impactAnimationTime = 0;
            }
        }

        /// <summary>
        /// Запускает анимацию столкновения (например, при ударе о ракетку), сжимая мяч и сбрасывая другие анимации.
        /// </summary>
        public void StartCollisionAnimation()
        {
            ScaleX = ScaleY = MinScale;
            collisionAnimationTime = CollisionAnimationDuration;
            flightAnimationTime = 0.0;
            IsImpactAnimationActive = false;
            impactAnimationTime = 0.0;
        }

        /// <summary>
        /// Сбрасывает все анимации мяча, возвращая масштаб и таймеры в начальное состояние.
        /// </summary>
        public void ResetAnimation()
        {
            ScaleX = ScaleY = 1.0f;
            collisionAnimationTime = 0.0;
            flightAnimationTime = 0.0;
            IsImpactAnimationActive = false;
            impactAnimationTime = 0.0;
        }
    }
}