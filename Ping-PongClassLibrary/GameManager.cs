using System;

namespace Ping_PongClassLibrary
{
    public class GameManager
    {
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public int ServeCount { get; set; }
        public bool IsPlayer1Turn { get; set; }
        public bool GameOver { get; set; }
        public int Winner { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр GameManager и сбрасывает состояние игры.
        /// </summary>
        public GameManager()
        {
            ResetGame();
        }

        /// <summary>
        /// Сбрасывает состояние игры до начального: обнуляет счет, устанавливает подачу за игроком 1 и сбрасывает флаг завершения игры.
        /// </summary>
        public void ResetGame()
        {
            Score1 = 0;
            Score2 = 0;
            ServeCount = 0;
            IsPlayer1Turn = true;
            GameOver = false;
            Winner = 0;
        }

        /// <summary>
        /// Начисляет очко одному из игроков на основе того, кто допустил ошибку, и обновляет состояние игры.
        /// </summary>
        public void AwardPoint(bool isPlayer1Miss)
        {
            if (isPlayer1Miss)
            {
                Score2++;
            }
            else
            {
                Score1++;
            }

            UpdateServe();
            CheckWinner();
        }

        /// <summary>
        /// Обновляет очередность подачи согласно правилам: смена после 2 подач или после каждой подачи при счете 10:10 и выше.
        /// </summary>
        public void UpdateServe()
        {
            ServeCount++;

            if (Score1 >= 10 && Score2 >= 10)
            {
                IsPlayer1Turn = !IsPlayer1Turn;
                ServeCount = 0;
            }
            else if (ServeCount >= 2)
            {
                IsPlayer1Turn = !IsPlayer1Turn;
                ServeCount = 0;
            }
        }

        /// <summary>
        /// Проверяет, достиг ли один из игроков победного счета (11 очков), и устанавливает флаг завершения игры и победителя.
        /// </summary>
        public void CheckWinner()
        {
            if (Score1 >= 11)
            {
                GameOver = true;
                Winner = 1;
            }
            else if (Score2 >= 11)
            {
                GameOver = true;
                Winner = 2;
            }
        }

        /// <summary>
        /// Определяет, нужно ли начислить очко, и кому, на основе состояния мяча и правил игры.
        /// </summary>
        public void DetermineScore(BallPhysics.BallState ballState, int screenWidth, int screenHeight, int tableTop, int tableBottom)
        {
            bool isScore = false;
            bool isPlayer1Miss = false;

            if (ballState.X - ballState.Radius < 0 && ballState.Vx < 0)
            {
                isScore = true;
                isPlayer1Miss = ballState.LastPaddleHit == 2 && ballState.HasTouchedOpponentTable;
            }
            else if (ballState.X + ballState.Radius > screenWidth && ballState.Vx > 0)
            {
                isScore = true;
                isPlayer1Miss = !(ballState.LastPaddleHit == 1 && ballState.HasTouchedOpponentTable);
            }
            else if ((ballState.Y - ballState.Radius < tableTop && ballState.Vy < 0) ||
                     (ballState.Y + ballState.Radius > tableBottom && ballState.Vy > 0))
            {
                isScore = true;
                isPlayer1Miss = ballState.LastPaddleHit == 1;
            }

            if (isScore)
            {
                AwardPoint(isPlayer1Miss);
            }
        }
    }
}