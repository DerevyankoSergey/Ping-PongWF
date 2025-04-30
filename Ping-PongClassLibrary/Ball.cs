using System;

namespace Ping_PongClassLibrary
{
    public class Ball
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Vx { get; private set; }
        public double Vy { get; private set; }
        public double Radius { get; private set; }
        private const double BallSpeed = 600;
        private const double StrikeSpeedMultiplier = 1.5;
        private bool hasCollidedWithPaddle;
        private readonly Random random = new Random();
        private IPaddle player1Paddle;
        private IPaddle player2Paddle;
        private bool isPaused;
        private double pauseTime;
        private const double PauseDuration = 1.0;
        private bool isPausedDueToOutOfBounds;
        private bool isWaitingForServe;
        private bool isPlayer1Serving;
        private readonly Table table;
        private bool hasTouchedOpponentTable;

        public event Action<bool> OnScore;

        public Ball(double x, double y, double radius,
                IPaddle player1Paddle, IPaddle player2Paddle, Table table)
        {
            X = x;
            Y = y;
            Radius = radius;
            this.player1Paddle = player1Paddle;
            this.player2Paddle = player2Paddle;
            Vx = 0;
            Vy = 0;
            hasCollidedWithPaddle = false;
            isPaused = false;
            pauseTime = 0;
            isPausedDueToOutOfBounds = false;
            isWaitingForServe = false;
            hasTouchedOpponentTable = false;
            this.table = table;
        }

        public bool IsPaused => isPaused;
        public double GetPauseTime => pauseTime;
        public bool IsPausedDueToOutOfBounds => isPausedDueToOutOfBounds;

        public void UpdatePaddles(IPaddle newPlayer1Paddle, IPaddle newPlayer2Paddle)
        {
            player1Paddle = newPlayer1Paddle;
            player2Paddle = newPlayer2Paddle;
        }

        public void Update(double deltaTime, int screenWidth, int screenHeight,
            int tableLeft, int tableRight, int tableTop, int tableBottom,
            bool isPlayer1Serving)
        {
            if (isPaused)
            {
                pauseTime -= deltaTime;
                if (pauseTime <= 0)
                {
                    isPaused = false;
                    ResetForServe(screenWidth, screenHeight, tableLeft, tableRight,
                                  tableTop, tableBottom, isPlayer1Serving);
                }
                return;
            }

            if (isWaitingForServe)
            {
                IPaddle servingPaddle = isPlayer1Serving ? player1Paddle : player2Paddle;
                bool serveResult = ServeByPaddle(servingPaddle, isPlayer1Serving);
                if (serveResult)
                {
                    isWaitingForServe = false;
                    hasTouchedOpponentTable = false;
                }
                return;
            }

            if (Vx == 0 && Vy == 0)
            {
                return;
            }

            double previousX = X;
            double previousY = Y;

            X += Vx * deltaTime;
            Y += Vy * deltaTime;

            if (!hasTouchedOpponentTable)
            {
                double netX = (tableLeft + tableRight) / 2;
                bool isMovingLeft = Vx < 0;
                bool isMovingRight = Vx > 0;

                bool targetingRightHalf = isMovingRight && hasCollidedWithPaddle && previousX < netX;
                bool targetingLeftHalf = isMovingLeft && hasCollidedWithPaddle && previousX > netX;

                if ((targetingRightHalf || targetingLeftHalf) &&
                    Y >= tableTop && Y <= tableBottom)
                {
                    if (targetingRightHalf && X >= netX && previousX < netX)
                    {
                        hasTouchedOpponentTable = true;
                    }
                    else if (targetingLeftHalf && X <= netX && previousX > netX)
                    {
                        hasTouchedOpponentTable = true;
                    }
                }
            }

            if (Vx < 0)
            {
                CheckContinuousCollision(X - Vx * deltaTime, Y - Vy * deltaTime, player1Paddle, true);
            }
            else if (Vx > 0)
            {
                CheckContinuousCollision(X - Vx * deltaTime, Y - Vy * deltaTime, player2Paddle, false);
            }

            bool isScore = false;
            bool isPlayer1Miss = false;

            if (X - Radius < 0)
            {
                isScore = true;
                isPlayer1Miss = hasTouchedOpponentTable || !hasCollidedWithPaddle;
            }
            else if (X + Radius > screenWidth)
            {
                isScore = true;
                isPlayer1Miss = !(hasTouchedOpponentTable || !hasCollidedWithPaddle);
            }

            if (isScore)
            {
                OnScore?.Invoke(isPlayer1Miss);
                hasCollidedWithPaddle = false;
                isPaused = true;
                pauseTime = PauseDuration;
                isPausedDueToOutOfBounds = true;
                hasTouchedOpponentTable = false;
            }
        }

        private void CheckContinuousCollision(double prevX, double prevY, IPaddle paddle, bool isLeftPaddle)
        {
            double paddingX = paddle.Width * 0.05;
            double paddingY = paddle.Height * 0.05;

            double paddleLeft = paddle.X + paddingX;
            double paddleRight = paddle.X + paddle.Width - paddingX;
            double paddleTop = paddle.Y - paddle.Height / 2 + paddingY;
            double paddleBottom = paddle.Y + paddle.Height / 2 - paddingY;

            double deltaX = X - prevX;
            double deltaY = Y - prevY;

            if ((isLeftPaddle && Vx < 0) || (!isLeftPaddle && Vx > 0))
            {
                double closestX, closestY;

                if (isLeftPaddle)
                {
                    closestX = paddleRight;
                    if (prevX - Radius <= closestX && X - Radius >= closestX)
                    {
                        return;
                    }
                }
                else
                {
                    closestX = paddleLeft;
                    if (prevX + Radius >= closestX && X + Radius <= closestX)
                    {
                        return;
                    }
                }

                double t = (closestX - (prevX + (isLeftPaddle ? -Radius : Radius))) / deltaX;

                if (t < 0 || t > 1)
                {
                    return;
                }

                closestY = prevY + t * deltaY;

                if (closestY >= paddleTop - Radius && closestY <= paddleBottom + Radius)
                {
                    HandlePaddleCollision(paddle, isLeftPaddle);
                    X = isLeftPaddle ? paddleRight + Radius : paddleLeft - Radius;
                    Y = closestY;
                    hasTouchedOpponentTable = false;
                }
            }
        }

        private void HandlePaddleCollision(IPaddle paddle, bool isLeftPaddle)
        {
            hasCollidedWithPaddle = true;

            double hitY = (Y - paddle.Y) / (paddle.Height / 2);
            hitY = Math.Max(-1.0, Math.Min(1.0, hitY));

            const double maxAngle = Math.PI / 4;

            double baseAngle = hitY * maxAngle;

            double paddleInfluence = paddle.Vy / 600.0;
            double angleAdjustment = paddleInfluence * (Math.PI / 12);
            double bounceAngle = baseAngle + angleAdjustment;

            bounceAngle = Math.Max(-maxAngle, Math.Min(maxAngle, bounceAngle));

            double randomVariation = (random.NextDouble() - 0.5) * (Math.PI / 36);
            bounceAngle += randomVariation;
            bounceAngle = Math.Max(-maxAngle, Math.Min(maxAngle, bounceAngle));

            // Фиксированная скорость мяча
            double newSpeed = BallSpeed;

            if (paddle is Paddle p)
            {
                // Увеличиваем скорость только если активен ChangeMaterialPrize
                newSpeed *= p.SpeedModifier; // SpeedModifier = 2.0 для ChangeMaterialPrize, иначе 1.0
                bounceAngle *= p.BounceModifier;
                Console.WriteLine($"Paddle collision, SpeedModifier: {p.SpeedModifier}, newSpeed: {newSpeed}");
            }

            Vx = newSpeed * Math.Cos(bounceAngle) * (isLeftPaddle ? 1 : -1);
            Vy = newSpeed * Math.Sin(bounceAngle);
        }

        public bool ServeByPaddle(IPaddle paddle, bool isLeftPaddle)
        {
            double paddingX = paddle.Width * 0.05;
            double paddingY = paddle.Height * 0.05;

            double paddleX = paddle.X;
            double paddleLeft = paddleX + paddingX;
            double paddleRight = paddleX + paddle.Width - paddingX;
            double paddleTop = paddle.Y - paddle.Height / 2 + paddingY;
            double paddleBottom = paddle.Y + paddle.Height / 2 - paddingY;

            double ballLeft = X - Radius;
            double ballRight = X + Radius;
            double ballTop = Y - Radius;
            double ballBottom = Y + Radius;

            bool isColliding = ballRight >= paddleLeft && ballLeft <= paddleRight &&
                               ballBottom >= paddleTop && ballTop <= paddleBottom;

            if (isColliding)
            {
                hasCollidedWithPaddle = true;

                double hitPosition = (Y - paddle.Y) / (paddle.Height / 2);
                hitPosition = Math.Max(-1, Math.Min(1, hitPosition));

                double maxBounceAngle = Math.PI / 12;
                double bounceAngle = hitPosition * maxBounceAngle;

                double speed = BallSpeed;
                Paddle p = paddle as Paddle;
                if (p != null)
                {
                    speed *= p.SpeedModifier;
                    if (p.IsStriking)
                    {
                        speed *= StrikeSpeedMultiplier;
                    }
                }

                Vx = speed * Math.Cos(bounceAngle) * (isLeftPaddle ? 1 : -1);
                Vy = speed * Math.Sin(bounceAngle);

                if (isLeftPaddle)
                {
                    X = paddleRight + Radius;
                }
                else
                {
                    X = paddleLeft - Radius;
                }

                return true;
            }

            return false;
        }

        public void ResetForServe(int screenWidth, int screenHeight,
                 int tableLeft, int tableRight,
                 int tableTop, int tableBottom,
                 bool isPlayer1Serving)
        {
            double centerY = (tableTop + tableBottom) / 2;
            if (isPlayer1Serving)
            {
                X = tableLeft + 60;
                Y = Math.Max(Radius, Math.Min(screenHeight - Radius, centerY));
            }
            else
            {
                X = tableRight - 60;
                Y = Math.Max(Radius, Math.Min(screenHeight - Radius, centerY));
            }
            Vx = 0;
            Vy = 0;
            hasCollidedWithPaddle = false;
            isPausedDueToOutOfBounds = false;
            isWaitingForServe = true;
            this.isPlayer1Serving = isPlayer1Serving;
            hasTouchedOpponentTable = false;
        }

        public void Serve(bool isLeft)
        {
            Vx = BallSpeed * (isLeft ? 1 : -1);
            Vy = 0;
            hasCollidedWithPaddle = false;
            isWaitingForServe = false;
            hasTouchedOpponentTable = false;
        }

        public void InitiateServe(bool isPlayer1Serving)
        {
            isWaitingForServe = true;
            this.isPlayer1Serving = isPlayer1Serving;
            hasTouchedOpponentTable = false;
        }
    }
}