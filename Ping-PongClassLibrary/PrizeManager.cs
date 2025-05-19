using System;
using System.Collections.Generic;
using System.Linq; // Добавляем эту директиву для использования LINQ

namespace Ping_PongClassLibrary
{
    /// <summary>
    /// Управляет призами в игре "Пинг-понг", включая их создание, удаление, столкновения и эффекты.
    /// </summary>
    public class PrizeManager
    {
        private readonly List<IPrize> prizes = new List<IPrize>();
        private readonly List<(IPrize Prize, double ActivationTime, double Duration)> player1ActivePrizes = new List<(IPrize, double, double)>();
        private readonly List<(IPrize Prize, double ActivationTime, double Duration)> player2ActivePrizes = new List<(IPrize, double, double)>();
        private readonly PrizeFactory prizeFactory = new RandomPrizeFactory();
        private readonly Random random = new Random();
        private double gameTime;
        private double nextPrizeSpawnTime;
        private readonly int screenWidth;
        private readonly int screenHeight;
        private readonly Table table;
        private const int MaxPrizes = 1;
        private const double PrizeSpawnInterval = 8.0;
        private const double PrizeLifetime = 5.0;
        private const int MaxSpawnAttempts = 10;

        /// <summary>
        /// Получает список активных призов на поле (только для чтения).
        /// </summary>
        public IReadOnlyList<IPrize> Prizes => prizes.AsReadOnly();

        /// <summary>
        /// Получает список активных эффектов призов для игрока 1 (только для чтения).
        /// </summary>
        public IReadOnlyList<(IPrize Prize, double ActivationTime, double Duration)> Player1ActivePrizes => player1ActivePrizes.AsReadOnly();

        /// <summary>
        /// Получает список активных эффектов призов для игрока 2 (только для чтения).
        /// </summary>
        public IReadOnlyList<(IPrize Prize, double ActivationTime, double Duration)> Player2ActivePrizes => player2ActivePrizes.AsReadOnly();

        /// <summary>
        /// Инициализирует новый экземпляр менеджера призов.
        /// </summary>
        /// <param name="screenWidth">Ширина экрана.</param>
        /// <param name="screenHeight">Высота экрана.</param>
        /// <param name="table">Объект стола с границами игрового поля.</param>
        public PrizeManager(int screenWidth, int screenHeight, Table table)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.table = table;
            nextPrizeSpawnTime = PrizeSpawnInterval;
        }

        /// <summary>
        /// Обновляет состояние призов и их эффектов.
        /// </summary>
        public void Update(double deltaTime, ref IPaddle player1Paddle, ref IPaddle player2Paddle)
        {
            gameTime += deltaTime;
            RemoveExpiredPrizes();
            SpawnPrizes(ref player1Paddle, ref player2Paddle);
            CheckPrizeCollisions(ref player1Paddle, ref player2Paddle);
            UpdateActivePrizes(ref player1Paddle, ref player2Paddle);
        }

        /// <summary>
        /// Удаляет призы, чье время жизни истекло.
        /// </summary>
        private void RemoveExpiredPrizes()
        {
            var expired = prizes.Where(p => gameTime >= p.SpawnTime + PrizeLifetime).ToList();
            foreach (var prize in expired)
            {
                prizes.Remove(prize);
            }
        }

        /// <summary>
        /// Пытается заспавнить новый приз, если выполняются условия.
        /// </summary>
        private void SpawnPrizes(ref IPaddle player1Paddle, ref IPaddle player2Paddle)
        {
            if (!ShouldSpawnPrize()) return;

            bool spawned = SpawnPrize(ref player1Paddle, ref player2Paddle);
            if (spawned)
            {
                nextPrizeSpawnTime = gameTime + PrizeSpawnInterval;
            }
            else
            {
                nextPrizeSpawnTime = gameTime + 0.5;
            }
        }

        /// <summary>
        /// Проверяет, можно ли заспавнить новый приз.
        /// </summary>
        private bool ShouldSpawnPrize()
        {
            return gameTime >= nextPrizeSpawnTime && prizes.Count < MaxPrizes;
        }

        /// <summary>
        /// Спавнит новый приз в случайной позиции, избегая пересечения с ракетками.
        /// </summary>
        private bool SpawnPrize(ref IPaddle player1Paddle, ref IPaddle player2Paddle)
        {
            int attempts = 0;
            double x, y;
            IPrize prize;
            int textureId = 4 + random.Next(3);
            bool isLeftSide = random.Next(2) == 0;

            do
            {
                x = isLeftSide
                    ? random.NextDouble() * (table.Left - 40)
                    : table.Right + random.NextDouble() * (screenWidth - table.Right - 40);
                y = table.Top + random.NextDouble() * (table.Height - 40);
                prize = prizeFactory.CreatePrize(x, y, textureId, gameTime);
                attempts++;
            } while ((IsCollidingWithPaddle(prize, player1Paddle) ||
                      IsCollidingWithPaddle(prize, player2Paddle)) && attempts < MaxSpawnAttempts);

            if (attempts >= MaxSpawnAttempts)
            {
                return false;
            }

            prizes.Add(prize);
            return true;
        }

        /// <summary>
        /// Проверяет, пересекается ли приз с ракеткой.
        /// </summary>
        private bool IsCollidingWithPaddle(IPrize prize, IPaddle paddle)
        {
            return prize.X < paddle.X + paddle.Width &&
                   prize.X + prize.Width > paddle.X &&
                   prize.Y < paddle.Y + paddle.Height / 2 &&
                   prize.Y + prize.Height > paddle.Y - paddle.Height / 2;
        }

        /// <summary>
        /// Проверяет столкновения призов с ракетками и применяет эффекты.
        /// </summary>
        private void CheckPrizeCollisions(ref IPaddle player1Paddle, ref IPaddle player2Paddle)
        {
            var toRemove = new List<IPrize>();
            foreach (var prize in prizes)
            {
                if (IsCollidingWithPaddle(prize, player1Paddle))
                {
                    player1ActivePrizes.Clear();
                    player1ActivePrizes.Add((prize, gameTime, GetPrizeDuration(prize)));
                    player1Paddle = CreateBasePaddle(true, player1Paddle.Y);
                    player1Paddle = prize.Apply(player1Paddle);
                    toRemove.Add(prize);
                }
                else if (IsCollidingWithPaddle(prize, player2Paddle))
                {
                    player2ActivePrizes.Clear();
                    player2ActivePrizes.Add((prize, gameTime, GetPrizeDuration(prize)));
                    player2Paddle = CreateBasePaddle(false, player2Paddle.Y);
                    player2Paddle = prize.Apply(player2Paddle);
                    toRemove.Add(prize);
                }
            }

            foreach (var prize in toRemove)
            {
                prizes.Remove(prize);
            }
        }

        /// <summary>
        /// Определяет длительность действия эффекта приза.
        /// </summary>
        private double GetPrizeDuration(IPrize prize)
        {
            if (prize is IncreaseLengthPrize) return 6.0;
            if (prize is IncreaseWidthPrize) return 6.0;
            if (prize is ChangeMaterialPrize) return 4.0;
            return 6.0;
        }

        /// <summary>
        /// Обновляет активные эффекты призов для обоих игроков.
        /// </summary>
        private void UpdateActivePrizes(ref IPaddle player1Paddle, ref IPaddle player2Paddle)
        {
            UpdatePlayerPrizes(ref player1Paddle, player1ActivePrizes, true);
            UpdatePlayerPrizes(ref player2Paddle, player2ActivePrizes, false);
        }

        /// <summary>
        /// Обновляет активные эффекты призов для конкретного игрока.
        /// </summary>
        private void UpdatePlayerPrizes(ref IPaddle paddle, List<(IPrize Prize, double ActivationTime, double Duration)> activePrizes, bool isPlayer1)
        {
            for (int i = activePrizes.Count - 1; i >= 0; i--)
            {
                if (gameTime >= activePrizes[i].ActivationTime + activePrizes[i].Duration)
                {
                    activePrizes.RemoveAt(i);
                    paddle = CreateBasePaddle(isPlayer1, paddle.Y);
                }
            }
        }

        /// <summary>
        /// Создает базовую ракетку для игрока.
        /// </summary>
        private IPaddle CreateBasePaddle(bool isPlayer1, double currentY)
        {
            double paddleWidth = 120;
            double visibleWidth = paddleWidth * 0.9;
            double overlap = 10;

            return new Paddle(
                isPlayer1 ? table.Left - visibleWidth + overlap : table.Right - (paddleWidth - visibleWidth) - overlap,
                currentY,
                paddleWidth,
                120,
                table.Top,
                table.Bottom,
                table.Left,
                table.Right,
                screenWidth,
                screenHeight
            );
        }
    }
}