using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ping_PongClassLibrary;

namespace Ping_PongClassLibrary.Tests
{
    [TestClass]
    public class BallPhysicsTests
    {
        private const double DeltaTime = 0.016;
        private const double Tolerance = 0.001;

        private class MockPaddle : IPaddle
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public double BaseHeight { get; set; }
            public double Vy { get; set; }
            public double SpeedModifier { get; set; }
            public double BounceModifier { get; set; }
            public double Mass { get; set; }

            public MockPaddle(double x, double y, double width, double height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                BaseHeight = height;
                Vy = 0;
                SpeedModifier = 1.0;
                BounceModifier = 1.0;
                Mass = 0.15;
            }

            public void UpdateVelocity(double newY, double deltaTime) { Y = newY; }
            public void Strike() { }
        }

        [TestMethod]
        public void Update_WhenPaused_DoesNotMove()
        {
            var player1Paddle = new MockPaddle(50, 300, 120, 120);
            var player2Paddle = new MockPaddle(680, 300, 120, 120);
            var table = new Table(800, 600);
            var ballPhysics = new BallPhysics(100, 100, 15, player1Paddle, player2Paddle, table);
            ballPhysics.PauseBall();

            ballPhysics.Update(DeltaTime, 800, 600, (int)table.Left, (int)table.Right, (int)table.Top, (int)table.Bottom, true);

            Assert.AreEqual(100, ballPhysics.X);
            Assert.AreEqual(100, ballPhysics.Y);
        }

        [TestMethod]
        public void ServeByPaddle_WhenBallInRange_ServesSuccessfully()
        {
            var player1Paddle = new MockPaddle(50, 300, 120, 120);
            var player2Paddle = new MockPaddle(680, 300, 120, 120);
            var table = new Table(800, 600);
            var ballPhysics = new BallPhysics(60, 300, 15, player1Paddle, player2Paddle, table);

            bool served = ballPhysics.ServeByPaddle(player1Paddle, true);

            Assert.IsTrue(served);
            Assert.IsTrue(ballPhysics.HasCollidedWithPaddle);
            Assert.AreEqual(1, ballPhysics.LastPaddleHit);
            Assert.IsTrue(ballPhysics.Vx > 0);
        }
    }
}