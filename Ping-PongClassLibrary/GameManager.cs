namespace Ping_PongClassLibrary
{
    public class GameManager
    {
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public int ServeCount { get; set; }
        public bool IsPlayer1Turn {  get; set; }
        public bool GameOver { get; set; }
        public int Winner {  get; set; }

        public GameManager()
        {
            ResetGame();
        }
        //Сброс игры
        public void ResetGame()
        {
            Score1 = 0;
            Score2 = 0;
            ServeCount = 0;
            IsPlayer1Turn = true;
            GameOver = false;
            Winner = 0;
            GameOver = false;
        }

        //Начисление очков
        public void AwardPoint(bool toPlayer1)
        {
            if (GameOver) return;

            if (toPlayer1)
                Score1++;
            else 
                Score2++;

            UpdateServe();
            CheckWinner();
        }

        //Обновление подач
        public void UpdateServe()
        {
            ServeCount++;

            if(Score1 >= 10 &&  Score2 >= 10)
            {
                IsPlayer1Turn = !IsPlayer1Turn;
                ServeCount = 0;
            }
            else if(ServeCount >= 2)
            {
                IsPlayer1Turn = !IsPlayer1Turn;
                ServeCount = 0;
            }
        }

        //Проверка победителя
        public void CheckWinner()
        {
            if(Score1 >= 11)
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
    }
}
