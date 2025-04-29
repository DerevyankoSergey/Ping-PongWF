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

        public GameManager()
        {
            ResetGame();
        }

        public void ResetGame()
        {
            Score1 = 0;
            Score2 = 0;
            ServeCount = 0;
            IsPlayer1Turn = true;
            GameOver = false;
            Winner = 0;
        }

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

        public void CheckWinner()
        {
            if (Score1 >= 11 && Score1 >= Score2 + 2)
            {
                GameOver = true;
                Winner = 1;
            }
            else if (Score2 >= 11 && Score2 >= Score1 + 2)
            {
                GameOver = true;
                Winner = 2;
            }
        }
    }
}