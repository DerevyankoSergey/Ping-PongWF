using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_PongClassLibrary
{
    public class Game
    {
        private readonly Ball ball;
        private Paddle player1Paddle;
        private Paddle player2Paddle;
        private readonly Table table;
        private readonly GameManager gameManager;

        public Game(int screenWidth, int screenHeight)
        {
            table = new Table(screenWidth, screenHeight);
            gameManager = new GameManager();
            player1Paddle = new Paddle(table.Left + 20, table.Top + table.Height / 2, 10, 60, 2.0, table.Top, table.Bottom);
            player2Paddle = new Paddle(table.Right - 30, table.Top + table.Height / 2, 10, 60, 2.0, table.Top, table.Bottom);
            ball = new Ball(table.Left + 20, table.Top + table.Height / 2, 5, 1.0);
            ball.OnScore += gameManager.AwardPoint;
        }

        public void Update(double deltaTime)
        {
            ball.Update(deltaTime, (int)table.Width, (int)table.Height, (int)table.Left, (int)table.Right, (int)table.Top, (int)table.Bottom, gameManager.IsPlayer1Turn);
            ball.CollideWithPaddle(player1Paddle, true);
            ball.CollideWithPaddle(player2Paddle, false);
        }

        public void MovePaddle1(double newY) => player1Paddle.UpdateVelocity(newY, 0.016);
        public void MovePaddle2(double newY) => player2Paddle.UpdateVelocity(newY, 0.016);

    }
}
