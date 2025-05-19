using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ping_PongClassLibrary;

namespace Ping_PongClassLibrary.Tests
{
    [TestClass]
    public class PrizeManagerTests
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
        public void Update_WhenPrizeExpires_RemovesPrize()
        {
            var table = new Table(800, 600);
            var prizeManager = new PrizeManager(800, 600, table);
            var prize = new IncreaseLengthPrize(400, 300, 40, 40, 5, 0);

            var prizesField = prizeManager.GetType().GetField("prizes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (prizesField == null)
            {
                Assert.Fail("Поле 'prizes' не найдено.");
            }
            prizesField.SetValue(prizeManager, new System.Collections.Generic.List<IPrize> { prize });

            var gameTimeField = prizeManager.GetType().GetField("gameTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (gameTimeField == null)
            {
                Assert.Fail("Поле 'gameTime' не найдено.");
            }
            gameTimeField.SetValue(prizeManager, 11.0 - DeltaTime);

            var nextPrizeSpawnTimeField = prizeManager.GetType().GetField("nextPrizeSpawnTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (nextPrizeSpawnTimeField == null)
            {
                Assert.Fail("Поле 'nextPrizeSpawnTime' не найдено.");
            }
            nextPrizeSpawnTimeField.SetValue(prizeManager, 12.0);

            IPaddle player1Paddle = new MockPaddle(50, 300, 120, 120);
            IPaddle player2Paddle = new MockPaddle(680, 300, 120, 120);

            prizeManager.Update(DeltaTime, ref player1Paddle, ref player2Paddle);

            Assert.AreEqual(0, prizeManager.Prizes.Count);
        }

        [TestMethod]
        public void CheckPrizeCollisions_WhenPlayer1Collides_AppliesPrize()
        {
            var table = new Table(800, 600);
            var prizeManager = new PrizeManager(800, 600, table);
            var prize = new IncreaseLengthPrize(50, 300, 40, 40, 5, 0);
            prizeManager.GetType().GetField("prizes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(prizeManager, new System.Collections.Generic.List<IPrize> { prize });

            IPaddle player1Paddle = new MockPaddle(50, 300, 120, 120);
            IPaddle player2Paddle = new MockPaddle(680, 300, 120, 120);

            prizeManager.Update(DeltaTime, ref player1Paddle, ref player2Paddle);

            Assert.AreEqual(0, prizeManager.Prizes.Count);
            Assert.AreEqual(1, prizeManager.Player1ActivePrizes.Count);
            Assert.IsTrue(player1Paddle is LengthIncreaseDecorator);
        }
    }
}