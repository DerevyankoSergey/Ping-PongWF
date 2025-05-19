using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ping_PongClassLibrary;

namespace Ping_PongClassLibrary.Tests
{
    [TestClass]
    public class PaddleTests
    {
        private const double DeltaTime = 0.016;
        private const double Tolerance = 0.001;

        [TestMethod]
        public void UpdateVelocity_WhenTargetYIsAbove_MovesUp()
        {
            var paddle = new Paddle(50, 300, 120, 120, 50, 550, 50, 750, 800, 600);
            double targetY = 200;

            paddle.UpdateVelocity(targetY, DeltaTime);

            double expectedY = 300 - 600 * DeltaTime;
            Assert.AreEqual(expectedY, paddle.Y, Tolerance);
        }

        [TestMethod]
        public void Strike_WhenCalled_SetsStrikingState()
        {
            var paddle = new Paddle(50, 300, 120, 120, 50, 550, 50, 750, 800, 600);

            paddle.Strike();

            Assert.IsTrue(paddle.IsStriking);
        }
    }
}