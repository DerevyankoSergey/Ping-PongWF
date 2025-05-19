using System;

namespace Ping_PongClassLibrary
{
    public class Ball
    {
        private readonly BallPhysics physics;
        private readonly BallAnimation animation;

        public double X => physics.X;
        public double Y => physics.Y;
        public double Vx => physics.Vx;
        public double Vy => physics.Vy;
        public double Radius => physics.Radius;
        public bool IsPaused => physics.IsPaused;
        public double GetPauseTime => physics.GetPauseTime;
        public bool IsPausedDueToOutOfBounds => physics.IsPausedDueToOutOfBounds;
        public float ScaleX => animation.ScaleX;
        public float ScaleY => animation.ScaleY;
        public bool IsImpactAnimationActive => animation.IsImpactAnimationActive;
        public double ImpactX => animation.ImpactX;
        public double ImpactY => animation.ImpactY;
        public int ImpactFrame => animation.ImpactFrame;

        public event Action<BallPhysics.BallState> OnPotentialScore
        {
            add => physics.OnPotentialScore += value;
            remove => physics.OnPotentialScore -= value;
        }

        public Ball(double x, double y, double radius, IPaddle player1Paddle, IPaddle player2Paddle, Table table)
        {
            physics = new BallPhysics(x, y, radius, player1Paddle, player2Paddle, table);
            animation = new BallAnimation();
        }

        /// <summary>
        /// Обновляет состояние мяча на каждом игровом цикле, включая физику движения и анимацию.
        /// </summary>
        public void Update(double deltaTime, int screenWidth, int screenHeight,
                           int tableLeft, int tableRight, int tableTop, int tableBottom,
                           bool isPlayer1Serving)
        {
            physics.Update(deltaTime, screenWidth, screenHeight, tableLeft, tableRight, tableTop, tableBottom, isPlayer1Serving);
            animation.Update(deltaTime, physics, tableLeft, tableRight);
        }

        /// <summary>
        /// Обновляет ссылки на ракетки игроков, передавая их в объект физики мяча.
        /// </summary>
        public void UpdatePaddles(IPaddle newPlayer1Paddle, IPaddle newPlayer2Paddle)
        {
            physics.UpdatePaddles(newPlayer1Paddle, newPlayer2Paddle);
        }

        /// <summary>
        /// Выполняет подачу мяча ударом ракеткой.
        /// </summary>
        public bool ServeByPaddle(IPaddle paddle, bool isLeftPaddle)
        {
            bool served = physics.ServeByPaddle(paddle, isLeftPaddle);
            return served;
        }

        /// <summary>
        /// Приостанавливает движение мяча, переводя его в состояние паузы.
        /// </summary>
        public void PauseBall()
        {
            physics.PauseBall();
        }

        /// <summary>
        /// Подготавливает мяч к подаче, размещая его рядом с ракеткой подающего игрока и сбрасывая анимацию.
        /// </summary>
        public void ResetForServe(int screenWidth, int screenHeight,
                                  int tableLeft, int tableRight,
                                  int tableTop, int tableBottom,
                                  bool isPlayer1Serving)
        {
            physics.ResetForServe(screenWidth, screenHeight, tableLeft, tableRight, tableTop, tableBottom, isPlayer1Serving);
            animation.ResetAnimation();
        }

        /// <summary>
        /// Выполняет автоматическую подачу мяча и запускает анимацию столкновения.
        /// </summary>
        public void Serve(bool isLeft)
        {
            physics.Serve(isLeft);
            animation.StartCollisionAnimation();
        }

        /// <summary>
        /// Инициирует процесс подачи, подготавливая состояние мяча и сбрасывая анимацию.
        /// </summary>
        public void InitiateServe(bool isPlayer1Serving)
        {
            physics.InitiateServe(isPlayer1Serving);
            animation.ResetAnimation();
        }

        /// <summary>
        /// Запускает анимацию подачи мяча (визуальный эффект столкновения).
        /// </summary>
        public void TriggerServeAnimation()
        {
            animation.StartCollisionAnimation();
        }
    }
}