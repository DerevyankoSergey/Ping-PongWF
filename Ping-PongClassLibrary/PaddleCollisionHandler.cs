using System;

namespace Ping_PongClassLibrary
{
    public class PaddleCollisionHandler
    {
        private readonly BallMovement movement;
        private readonly Random random = new Random();
        private IPaddle player1Paddle;
        private IPaddle player2Paddle;
        public bool HasCollidedWithPaddle { get; set; }
        public int LastPaddleHit { get; set; }

        private const double BallMass = 0.0027;
        private const double BaseSpeed = 600;
        private const double StrikeSpeedMultiplier = 1.5;
        private const double CoefficientOfRestitution = 0.9;

        /// <summary>
        /// Инициализирует новый экземпляр обработчика столкновений мяча с ракетками.
        /// </summary>
        public PaddleCollisionHandler(BallMovement movement, IPaddle player1Paddle, IPaddle player2Paddle)
        {
            this.movement = movement;
            this.player1Paddle = player1Paddle;
            this.player2Paddle = player2Paddle;
        }

        /// <summary>
        /// Обновляет ссылки на ракетки игроков.
        /// </summary>
        public void UpdatePaddles(IPaddle newPlayer1Paddle, IPaddle newPlayer2Paddle)
        {
            player1Paddle = newPlayer1Paddle;
            player2Paddle = newPlayer2Paddle;
        }

        /// <summary>
        /// Проверяет столкновение мяча с ракеткой в зависимости от направления его движения.
        /// </summary>
        public void CheckCollisions(double previousX, double previousY, bool isPlayer1Serving)
        {
            if (movement.Vx < 0)
                HandlePaddleCollision(previousX, previousY, player1Paddle, true, isPlayer1Serving);
            else if (movement.Vx > 0)
                HandlePaddleCollision(previousX, previousY, player2Paddle, false, isPlayer1Serving);
        }

        /// <summary>
        /// Обрабатывает столкновение мяча с конкретной ракеткой.
        /// </summary>
        private void HandlePaddleCollision(double previousX, double previousY, IPaddle paddle, bool isLeftPaddle, bool isPlayer1Serving)
        {
            double paddleLeft = paddle.X + paddle.Width * 0.05;
            double paddleRight = paddle.X + paddle.Width * 0.95;
            double paddleTop = paddle.Y - paddle.Height * 0.45;
            double paddleBottom = paddle.Y + paddle.Height * 0.45;
            double closestX = isLeftPaddle ? paddleRight : paddleLeft;

            if ((isLeftPaddle && (previousX - movement.Radius <= closestX && movement.X - movement.Radius >= closestX)) ||
                (!isLeftPaddle && (previousX + movement.Radius >= closestX && movement.X + movement.Radius <= closestX)))
                return;

            double timeToCollision = (closestX - (previousX + (isLeftPaddle ? -movement.Radius : movement.Radius))) / (movement.X - previousX);
            if (timeToCollision < 0 || timeToCollision > 1) return;

            double collisionY = previousY + timeToCollision * (movement.Y - previousY);
            if (collisionY < paddleTop - movement.Radius || collisionY > paddleBottom + movement.Radius) return;

            ProcessPaddleCollision(paddle, isLeftPaddle);
            movement.SetPosition(isLeftPaddle ? paddleRight + movement.Radius : paddleLeft - movement.Radius, collisionY);
            HasCollidedWithPaddle = true;
            LastPaddleHit = isLeftPaddle ? 1 : 2;
        }

        /// <summary>
        /// Обрабатывает физику отскока мяча после столкновения с ракеткой.
        /// </summary>
        private void ProcessPaddleCollision(IPaddle paddle, bool isLeftPaddle)
        {
            double hitPosition = Math.Max(-1.0, Math.Min(1.0, (movement.Y - paddle.Y) / (paddle.BaseHeight / 2)));

            double baseAngle = hitPosition * (Math.PI / 6);
            double paddleInfluence = paddle.Vy / 600.0 * (Math.PI / 36);
            double randomVariation = (random.NextDouble() - 0.5) * (Math.PI / 90);
            double bounceAngle = Math.Max(-Math.PI / 12, Math.Min(Math.PI / 12, baseAngle + paddleInfluence + randomVariation));
            bounceAngle *= paddle.BounceModifier;

            double paddleMass = paddle.Mass;
            double totalMass = BallMass + paddleMass;
            double massFactor = paddleMass / BallMass;
            double speedAdjustment = 1 + massFactor * 0.005;
            double speed = BaseSpeed * paddle.SpeedModifier * speedAdjustment * CoefficientOfRestitution;
            if (paddle is Paddle concretePaddle && concretePaddle.IsStriking)
            {
                speed *= StrikeSpeedMultiplier;
            }
            speed = Math.Min(speed, 1200);

            movement.SetVelocity(speed * Math.Cos(bounceAngle) * (isLeftPaddle ? 1 : -1), speed * Math.Sin(bounceAngle));
        }
    }
}