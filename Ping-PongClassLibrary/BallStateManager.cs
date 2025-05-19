using System;

namespace Ping_PongClassLibrary
{
    public class BallStateManager
    {
        private readonly BallMovement movement;
        private readonly PaddleCollisionHandler collisionHandler;
        private readonly Table gameTable;
        private bool isPaused;
        private double getPauseTime;
        private bool isPausedDueToOutOfBounds;
        private bool hasTouchedOpponentTable;
        private bool isOverNet;
        private const double PauseDuration = 1.0;

        public event Action<BallPhysics.BallState> OnPotentialScore;

        public BallStateManager(BallMovement movement, PaddleCollisionHandler collisionHandler, Table gameTable)
        {
            this.movement = movement;
            this.collisionHandler = collisionHandler;
            this.gameTable = gameTable;
        }

        /// <summary>
        /// Обновляет состояние мяча на каждом игровом цикле, включая проверку паузы, касания стола, пересечения сетки и начисления очков.
        /// </summary>
        public void UpdateState(double deltaTime, int screenWidth, int screenHeight, int tableLeft, int tableRight, int tableTop, int tableBottom)
        {
            if (isPaused)
            {
                getPauseTime -= deltaTime;
                if (getPauseTime <= 0)
                    isPaused = isPausedDueToOutOfBounds = false;
                return;
            }

            if (!hasTouchedOpponentTable)
            {
                double netX = (tableLeft + tableRight) / 2.0;
                bool isMovingLeft = movement.Vx < 0;
                bool isMovingRight = movement.Vx > 0;

                bool targetingRightHalf = isMovingRight && collisionHandler.HasCollidedWithPaddle && movement.PreviousX < netX;
                bool targetingLeftHalf = isMovingLeft && collisionHandler.HasCollidedWithPaddle && movement.PreviousX > netX;

                if ((targetingRightHalf || targetingLeftHalf) && movement.Y >= tableTop && movement.Y <= tableBottom)
                {
                    if (targetingRightHalf && movement.X >= netX && movement.PreviousX < netX)
                    {
                        hasTouchedOpponentTable = true;
                    }
                    else if (targetingLeftHalf && movement.X <= netX && movement.PreviousX > netX)
                    {
                        hasTouchedOpponentTable = true;
                    }
                }
            }

            double tableMidpoint = (tableLeft + tableRight) / 2.0;
            if ((movement.PreviousX <= tableMidpoint && movement.X > tableMidpoint && movement.Vx > 0) ||
                (movement.PreviousX >= tableMidpoint && movement.X < tableMidpoint && movement.Vx < 0))
            {
                isOverNet = true;
            }

            CheckForScore(screenWidth, screenHeight, tableTop, tableBottom);
        }

        /// <summary>
        /// Проверяет, может ли мяч привести к начислению очков, и вызывает событие OnPotentialScore при необходимости.
        /// </summary>
        private void CheckForScore(int screenWidth, int screenHeight, int tableTop, int tableBottom)
        {
            bool isPotentialScore = false;

            if (movement.X - movement.Radius < 0 && movement.Vx < 0)
            {
                isPotentialScore = true;
            }
            else if (movement.X + movement.Radius > screenWidth && movement.Vx > 0)
            {
                isPotentialScore = true;
            }
            else if ((movement.Y - movement.Radius < 0 && movement.Vy < 0) ||
                     (movement.Y + movement.Radius > screenHeight && movement.Vy > 0))
            {
                if (collisionHandler.LastPaddleHit != 0)
                {
                    isPotentialScore = true;
                }
            }

            if (isPotentialScore)
            {
                OnPotentialScore?.Invoke(new BallPhysics.BallState
                {
                    X = movement.X,
                    Y = movement.Y,
                    Vx = movement.Vx,
                    Vy = movement.Vy,
                    HasTouchedOpponentTable = hasTouchedOpponentTable,
                    LastPaddleHit = collisionHandler.LastPaddleHit,
                    Radius = movement.Radius,
                    IsOverNet = isOverNet
                });
            }
        }

        /// <summary>
        /// Приостанавливает мяч, устанавливая паузу и сбрасывая состояние касания стола и пересечения сетки.
        /// </summary>
        public void PauseBall()
        {
            isPaused = true;
            getPauseTime = PauseDuration;
            isPausedDueToOutOfBounds = true;
            hasTouchedOpponentTable = false;
            isOverNet = false;
        }

        public bool IsPaused => isPaused;
        public double GetPauseTime => getPauseTime;
        public bool IsPausedDueToOutOfBounds => isPausedDueToOutOfBounds;
        public bool HasTouchedOpponentTable => hasTouchedOpponentTable;
        public bool IsOverNet => isOverNet;
    }
}