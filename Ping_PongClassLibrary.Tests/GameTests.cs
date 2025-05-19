using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ping_PongClassLibrary;

namespace Ping_PongClassLibrary.Tests
{
    [TestClass]
    public class GameTests
    {
        private const double DeltaTime = 0.016;
        private const double Tolerance = 0.001;

        [TestMethod]
        public void MovePaddle1_UpdatesPaddlePosition()
        {
            var game = new Game(800, 600);
            var paddle1 = game.GetPlayer1Paddle();
            double initialY = paddle1.Y;
            double targetY = initialY - 50;

            game.MovePaddle1(targetY);

            double expectedY = initialY - 600 * DeltaTime;
            Assert.AreEqual(expectedY, paddle1.Y, Tolerance);
        }

        [TestMethod]
        public void StrikePaddle1_SetsStrikingState()
        {
            var game = new Game(800, 600);

            game.StrikePaddle1();

            Assert.IsTrue(game.GetPlayer1Paddle() is Paddle paddle && paddle.IsStriking);
        }

        [TestMethod]
        public void HandlePotentialScore_WhenScoreAwarded_PausesBall()
        {
            var game = new Game(800, 600);
            var ballState = new BallPhysics.BallState
            {
                X = -10,
                Y = 300,
                Vx = -600,
                Vy = 0,
                HasTouchedOpponentTable = true,
                LastPaddleHit = 2,
                Radius = 15
            };

            game.GetType()
                .GetMethod("HandlePotentialScore", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(game, new object[] { ballState });

            Assert.IsTrue(game.GetBall().IsPaused);
            Assert.AreEqual(0, game.GetGameManager().Score1);
            Assert.AreEqual(1, game.GetGameManager().Score2);
        }
    }
}