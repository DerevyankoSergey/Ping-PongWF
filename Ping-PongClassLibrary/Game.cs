using System;
using System.Collections.Generic;

namespace Ping_PongClassLibrary
{
    public class Game : IGame
    {
        private readonly Ball ball;
        private IPaddle player1Paddle;
        private IPaddle player2Paddle;
        private readonly Table table;
        private readonly GameManager gameManager;
        private readonly PrizeManager prizeManager;
        private readonly int screenWidth;
        private readonly int screenHeight;

        /// <summary>
        /// Инициализирует новый экземпляр игры с заданными размерами экрана.
        /// </summary>
        public Game(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            table = new Table(screenWidth, screenHeight);
            gameManager = new GameManager();
            prizeManager = new PrizeManager(screenWidth, screenHeight, table);

            InitializePaddles();

            ball = new Ball(table.Left + 60, table.Top + table.Height / 2, 15, player1Paddle, player2Paddle, table);
            ball.OnPotentialScore += HandlePotentialScore;
        }

        /// <summary>
        /// Инициализирует ракетки для обоих игроков, размещая их по краям стола.
        /// </summary>
        private void InitializePaddles()
        {
            double paddleWidth = 120;
            double paddleHeight = 120;
            double visibleWidth = paddleWidth * 0.9;
            double overlap = 10;

            player1Paddle = new Paddle(
                table.Left - visibleWidth + overlap,
                table.Top + table.Height / 2,
                paddleWidth,
                paddleHeight,
                table.Top,
                table.Bottom,
                table.Left,
                table.Right,
                screenWidth,
                screenHeight);

            player2Paddle = new Paddle(
                table.Right - (paddleWidth - visibleWidth) - overlap,
                table.Top + table.Height / 2,
                paddleWidth,
                paddleHeight,
                table.Top,
                table.Bottom,
                table.Left,
                table.Right,
                screenWidth,
                screenHeight);
        }

        /// <summary>
        /// Обрабатывает событие OnPotentialScore, определяя начисление очков и приостанавливая мяч.
        /// </summary>
        private void HandlePotentialScore(BallPhysics.BallState ballState)
        {
            gameManager.DetermineScore(ballState, screenWidth, screenHeight, (int)table.Top, (int)table.Bottom);
            ball.PauseBall();
        }

        /// <summary>
        /// Обновляет состояние игры на каждом игровом цикле, включая призы, мяч и ракетки.
        /// </summary>
        public void Update(double deltaTime)
        {
            prizeManager.Update(deltaTime, ref player1Paddle, ref player2Paddle);
            ball.Update(deltaTime, screenWidth, screenHeight,
                (int)table.Left, (int)table.Right,
                (int)table.Top, (int)table.Bottom,
                gameManager.IsPlayer1Turn);
            ball.UpdatePaddles(player1Paddle, player2Paddle);
        }

        /// <summary>
        /// Пытается выполнить подачу мяча указанным игроком.
        /// </summary>
        public void TryServe(bool isPlayer1)
        {
            if (isPlayer1)
            {
                player1Paddle.Strike();
                ball.InitiateServe(true);
            }
            else
            {
                player2Paddle.Strike();
                ball.InitiateServe(false);
            }
        }

        public void MovePaddle1(double targetY) => player1Paddle.UpdateVelocity(targetY, 0.016f);

        public void MovePaddle2(double targetY) => player2Paddle.UpdateVelocity(targetY, 0.016f);

        public void StrikePaddle1() => player1Paddle.Strike();

        public void StrikePaddle2() => player2Paddle.Strike();

        public Ball GetBall() => ball;

        public IPaddle GetPlayer1Paddle() => player1Paddle;

        public IPaddle GetPlayer2Paddle() => player2Paddle;

        public Table GetTable() => table;

        public GameManager GetGameManager() => gameManager;

        public IReadOnlyList<IPrize> GetPrizes() => prizeManager.Prizes;

        public IReadOnlyList<(IPrize Prize, double ActivationTime, double Duration)> Player1ActivePrizes => prizeManager.Player1ActivePrizes;

        public IReadOnlyList<(IPrize Prize, double ActivationTime, double Duration)> Player2ActivePrizes => prizeManager.Player2ActivePrizes;
    }
}