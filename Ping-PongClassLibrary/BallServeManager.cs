using System;

namespace Ping_PongClassLibrary
{
    public class BallServeManager
    {
        private readonly BallMovement movement;
        private readonly PaddleCollisionHandler collisionHandler;
        private bool isWaitingForServe;
        private bool isPlayer1Serving;
        private const double BallMass = 0.0027;
        private const double BaseSpeed = 600;
        private const double StrikeSpeedMultiplier = 1.5;

        public BallServeManager(BallMovement movement, PaddleCollisionHandler collisionHandler)
        {
            this.movement = movement;
            this.collisionHandler = collisionHandler;
        }

        /// <summary>
        /// Выполняет подачу мяча путем удара ракеткой.
        /// Проверяет столкновение мяча с ракеткой, рассчитывает угол и скорость отскока, обновляет состояние игры.
        /// </summary>
        public bool ServeByPaddle(IPaddle paddle, bool isLeftPaddle)
        {
            double paddleLeft = paddle.X + paddle.Width * 0.05;
            double paddleRight = paddle.X + paddle.Width * 0.95;
            double paddleTop = paddle.Y - paddle.Height * 0.45;
            double paddleBottom = paddle.Y + paddle.Height * 0.45;

            if (!(movement.X + movement.Radius >= paddleLeft && movement.X - movement.Radius <= paddleRight &&
                  movement.Y + movement.Radius >= paddleTop && movement.Y - movement.Radius <= paddleBottom))
                return false;

            collisionHandler.HasCollidedWithPaddle = true;
            collisionHandler.LastPaddleHit = isLeftPaddle ? 1 : 2;
            double hitPosition = Math.Max(-1, Math.Min(1, (movement.Y - paddle.Y) / (paddle.BaseHeight / 2)));
            double bounceAngle = hitPosition * (Math.PI / 18);

            double speed = BaseSpeed;
            double paddleMass = paddle.Mass;
            double massFactor = paddleMass / BallMass;
            double speedAdjustment = 1 + massFactor * 0.005;
            speed *= paddle.SpeedModifier * speedAdjustment;
            if (paddle is Paddle concretePaddle && concretePaddle.IsStriking)
            {
                speed *= StrikeSpeedMultiplier;
            }
            speed = Math.Min(speed, 1200);

            movement.SetVelocity(speed * Math.Cos(bounceAngle) * (isLeftPaddle ? 1 : -1), speed * Math.Sin(bounceAngle));
            movement.SetPosition(isLeftPaddle ? paddleRight + movement.Radius : paddleLeft - movement.Radius, movement.Y);

            isWaitingForServe = false;
            return true;
        }

        /// <summary>
        /// Подготавливает мяч к подаче, размещая его рядом с ракеткой подающего игрока и сбрасывая состояние игры.
        /// </summary>
        public void ResetForServe(int screenWidth, int screenHeight, int tableLeft, int tableRight, int tableTop, int tableBottom, bool isPlayer1Serving)
        {
            double centerY = (tableTop + tableBottom) / 2;
            if (isPlayer1Serving)
            {
                movement.SetPosition(tableLeft + 60, Math.Max(movement.Radius, Math.Min(screenHeight - movement.Radius, centerY)));
            }
            else
            {
                movement.SetPosition(tableRight - 60, Math.Max(movement.Radius, Math.Min(screenHeight - movement.Radius, centerY)));
            }

            movement.SetVelocity(0, 0);
            isWaitingForServe = true;
            this.isPlayer1Serving = isPlayer1Serving;
            collisionHandler.HasCollidedWithPaddle = false;
            collisionHandler.LastPaddleHit = 0;
        }

        /// <summary>
        /// Выполняет автоматическую подачу мяча, задавая ему начальную скорость и обновляя состояние игры.
        /// </summary>
        public void Serve(bool isLeft)
        {
            movement.SetVelocity(BaseSpeed * (isLeft ? 1 : -1), 0);
            collisionHandler.HasCollidedWithPaddle = false;
            collisionHandler.LastPaddleHit = isLeft ? 1 : 2;
            isWaitingForServe = false;
        }

        /// <summary>
        /// Инициирует процесс подачи, подготавливая состояние игры.
        /// </summary>
        public void InitiateServe(bool isPlayer1Serving)
        {
            isWaitingForServe = true;
            this.isPlayer1Serving = isPlayer1Serving;
            collisionHandler.HasCollidedWithPaddle = false;
            collisionHandler.LastPaddleHit = 0;
        }

        public bool IsWaitingForServe => isWaitingForServe;
        public bool IsPlayer1Serving => isPlayer1Serving;
    }
}