using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ping_PongClassLibrary;

namespace Ping_PongClassLibrary.Tests
{
    [TestClass]
    public class GameManagerTests
    {
        [TestMethod]
        public void AwardPoint_WhenPlayer1Misses_IncrementsScore2()
        {
            var gameManager = new GameManager();

            gameManager.AwardPoint(true);

            Assert.AreEqual(0, gameManager.Score1);
            Assert.AreEqual(1, gameManager.Score2);
        }

        [TestMethod]
        public void UpdateServe_WhenServeCountReaches2_SwitchesTurn()
        {
            var gameManager = new GameManager();
            gameManager.ServeCount = 1;
            gameManager.IsPlayer1Turn = true;

            gameManager.AwardPoint(false);

            Assert.IsFalse(gameManager.IsPlayer1Turn);
            Assert.AreEqual(0, gameManager.ServeCount);
        }

        [TestMethod]
        public void CheckWinner_WhenScore1Wins_SetsGameOverAndWinner()
        {
            var gameManager = new GameManager();
            gameManager.Score1 = 11;
            gameManager.Score2 = 8;

            gameManager.CheckWinner();

            Assert.IsTrue(gameManager.GameOver);
            Assert.AreEqual(1, gameManager.Winner);
        }
    }
}