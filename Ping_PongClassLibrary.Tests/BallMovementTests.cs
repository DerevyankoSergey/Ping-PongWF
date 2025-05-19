using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ping_PongClassLibrary;

namespace Ping_PongClassLibrary.Tests
{
    [TestClass]
    public class BallMovementTests
    {
        private const double DeltaTime = 0.016;
        private const double Tolerance = 0.001;

        [TestMethod]
        public void Update_WhenVelocityIsSet_MovesCorrectly()
        {
            var ballMovement = new BallMovement(100, 100, 15);
            ballMovement.SetVelocity(600, 300);

            ballMovement.Update(DeltaTime);

            double expectedX = 100 + 600 * DeltaTime;
            double expectedY = 100 + 300 * DeltaTime;
            Assert.AreEqual(expectedX, ballMovement.X, Tolerance);
            Assert.AreEqual(expectedY, ballMovement.Y, Tolerance);
        }

        [TestMethod]
        public void SetPosition_UpdatesPositionAndPreviousPosition()
        {
            var ballMovement = new BallMovement(100, 100, 15);
            ballMovement.SetVelocity(600, 300);
            ballMovement.Update(DeltaTime);

            ballMovement.SetPosition(200, 200);

            Assert.AreEqual(200, ballMovement.X);
            Assert.AreEqual(200, ballMovement.Y);
            Assert.AreEqual(100 + 600 * DeltaTime, ballMovement.PreviousX, Tolerance);
            Assert.AreEqual(100 + 300 * DeltaTime, ballMovement.PreviousY, Tolerance);
        }
    }
}