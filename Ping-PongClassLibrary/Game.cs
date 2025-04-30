using System;
using System.Collections.Generic;
using System.Linq;

namespace Ping_PongClassLibrary
{
    public class Game
    {
        private readonly Ball ball;
        private IPaddle player1Paddle;
        private IPaddle player2Paddle;
        private readonly Table table;
        private readonly GameManager gameManager;
        private readonly List<IPrize> prizes;
        private readonly PrizeFactory prizeFactory;
        private readonly Random random;
        private double gameTime;
        private double nextPrizeSpawnTime;
        private const double PrizeSpawnInterval = 5.0;
        private readonly List<(IPrize Prize, double ActivationTime, double Duration)> player1ActivePrizes;
        private readonly List<(IPrize Prize, double ActivationTime, double Duration)> player2ActivePrizes;
        private readonly int screenWidth;
        private readonly int screenHeight;

        public Game(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            table = new Table(screenWidth, screenHeight);
            gameManager = new GameManager();
            prizes = new List<IPrize>();
            prizeFactory = new RandomPrizeFactory();
            random = new Random();

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

            ball = new Ball(
                table.Left + 60,
                table.Top + table.Height / 2,
                15,
                player1Paddle,
                player2Paddle,
                table);

            ball.OnScore += gameManager.AwardPoint;
            gameTime = 0;
            nextPrizeSpawnTime = PrizeSpawnInterval;
            player1ActivePrizes = new List<(IPrize, double, double)>();
            player2ActivePrizes = new List<(IPrize, double, double)>();
        }

        public void Update(double deltaTime)
        {
            gameTime += deltaTime;

            ball.Update(deltaTime, screenWidth, screenHeight,
                        (int)table.Left, (int)table.Right,
                        (int)table.Top, (int)table.Bottom,
                        gameManager.IsPlayer1Turn);

            UpdatePrizes(gameTime);

            if (gameTime >= nextPrizeSpawnTime && prizes.Count == 0)
            {
                SpawnPrize();
                nextPrizeSpawnTime = gameTime + PrizeSpawnInterval;
            }

            CheckPrizeCollisions();
            UpdatePrizeCooldowns(player1ActivePrizes, ref player1Paddle, true);
            UpdatePrizeCooldowns(player2ActivePrizes, ref player2Paddle, false);
        }

        private void UpdatePrizes(double gameTime)
        {
            var toRemove = prizes.Where(p => gameTime >= p.SpawnTime + p.Lifetime).ToList();
            foreach (var prize in toRemove)
            {
                prizes.Remove(prize);
            }
        }

        private void SpawnPrize()
        {
            if (prizes.Count > 0)
                return;

            bool isLeftSide = random.Next(2) == 0;

            double x, y;
            double prizeWidth = 40; 
            double prizeHeight = 40;

            if (isLeftSide)
            {
                x = random.NextDouble() * (table.Left - prizeWidth);
            }
            else
            {
                x = table.Right + random.NextDouble() * (screenWidth - table.Right - prizeWidth);
            }

            y = table.Top + random.NextDouble() * (table.Height - prizeHeight);

            // Создаем приз
            int textureId = 4 + random.Next(3); 
            var prize = prizeFactory.CreatePrize(x, y, textureId, gameTime);
            prizes.Add(prize);
        }

        private void UpdatePrizeCooldowns(List<(IPrize Prize, double ActivationTime, double Duration)> activePrizes,
                                         ref IPaddle paddle, bool isPlayer1)
        {
            var toRemove = new List<(IPrize, double, double)>();
            foreach (var (prize, time, duration) in activePrizes)
            {
                if (gameTime >= time + duration)
                {
                    toRemove.Add((prize, time, duration));
                }
            }

            if (toRemove.Count > 0)
            {
                activePrizes.RemoveAll(x => toRemove.Contains(x));
                ResetPaddle(ref paddle, isPlayer1, activePrizes);
                ball.UpdatePaddles(player1Paddle, player2Paddle);
            }
        }

        private void ResetPaddle(ref IPaddle paddle, bool isPlayer1, List<(IPrize Prize, double, double)> activePrizes)
        {
            double paddleWidth = 120;
            double visibleWidth = paddleWidth * 0.9;
            double overlap = 10;
            paddle = new Paddle(
                isPlayer1 ? table.Left - visibleWidth + overlap : table.Right - (paddleWidth - visibleWidth) - overlap,
                paddle.Y,
                paddleWidth,
                120,
                table.Top,
                table.Bottom,
                table.Left,
                table.Right,
                screenWidth,
                screenHeight
            );

            foreach (var (prize, _, _) in activePrizes)
            {
                paddle = prize.Apply(paddle);
            }

            paddle.Y = Math.Max(0 + paddle.Height / 2,
                               Math.Min(screenHeight - paddle.Height / 2, paddle.Y));
        }

        private void CheckPrizeCollisions()
        {
            var toRemove = new List<IPrize>();
            foreach (var prize in prizes)
            {
                if (IsCollidingWithPaddle(prize, player1Paddle))
                {
                    if (player1ActivePrizes.Count > 0)
                    {
                        continue;
                    }

                    double duration = GetPrizeDuration(prize);
                    player1Paddle = prize.Apply(player1Paddle);
                    player1Paddle.Y = Math.Max(0 + player1Paddle.Height / 2,
                                              Math.Min(screenHeight - player1Paddle.Height / 2, player1Paddle.Y));
                    player1ActivePrizes.Add((prize, gameTime, duration));
                    toRemove.Add(prize);
                    ball.UpdatePaddles(player1Paddle, player2Paddle);
                }
                else if (IsCollidingWithPaddle(prize, player2Paddle))
                {
                    if (player2ActivePrizes.Count > 0)
                    {
                        continue;
                    }

                    double duration = GetPrizeDuration(prize);
                    player2Paddle = prize.Apply(player2Paddle);
                    player2Paddle.Y = Math.Max(0 + player2Paddle.Height / 2,
                                              Math.Min(screenHeight - player2Paddle.Height / 2, player2Paddle.Y));
                    player2ActivePrizes.Add((prize, gameTime, duration));
                    toRemove.Add(prize);
                    ball.UpdatePaddles(player1Paddle, player2Paddle);
                }
            }
            foreach (var prize in toRemove)
            {
                prizes.Remove(prize);
            }
        }

        private double GetPrizeDuration(IPrize prize)
        {
            if (prize is IncreaseLengthPrize)
                return 5.0;
            else if (prize is IncreaseWidthPrize)
                return 5.0;
            else if (prize is ChangeMaterialPrize)
                return 5.0;
            return 10.0;
        }

        private bool IsCollidingWithPaddle(IPrize prize, IPaddle paddle)
        {
            return prize.X < paddle.X + paddle.Width &&
                   prize.X + prize.Width > paddle.X &&
                   prize.Y < paddle.Y + paddle.Height / 2 &&
                   prize.Y + prize.Height > paddle.Y - paddle.Height / 2;
        }

        public void TryServe(bool isPlayer1)
        {
            var paddle = isPlayer1 ? player1Paddle : player2Paddle;
            paddle.Strike();
            ball.InitiateServe(isPlayer1);
        }

        public List<IPrize> GetPrizes() => prizes;

        public void MovePaddle1(double targetY)
        {
            player1Paddle.UpdateVelocity(targetY, 0.016f);
        }

        public void MovePaddle2(double targetY)
        {
            player2Paddle.UpdateVelocity(targetY, 0.016f);
        }

        public void StrikePaddle1()
        {
            player1Paddle.Strike();
        }

        public void StrikePaddle2()
        {
            player2Paddle.Strike();
        }

        public Ball GetBall() => ball;
        public IPaddle GetPlayer1Paddle() => player1Paddle;
        public IPaddle GetPlayer2Paddle() => player2Paddle;
        public Table GetTable() => table;
        public GameManager GetGameManager() => gameManager;
    }
}