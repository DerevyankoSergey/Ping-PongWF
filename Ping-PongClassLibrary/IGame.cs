using System.Collections.Generic;

namespace Ping_PongClassLibrary
{
    public interface IGame
    {
        Ball GetBall();
        IPaddle GetPlayer1Paddle();
        IPaddle GetPlayer2Paddle();
        Table GetTable();
        GameManager GetGameManager();
        IReadOnlyList<IPrize> GetPrizes();
        IReadOnlyList<(IPrize Prize, double ActivationTime, double Duration)> Player1ActivePrizes { get; }
        IReadOnlyList<(IPrize Prize, double ActivationTime, double Duration)> Player2ActivePrizes { get; }
        void MovePaddle1(double targetY);
        void MovePaddle2(double targetY);
        void StrikePaddle1();
        void StrikePaddle2();
        void TryServe(bool isPlayer1);
        void Update(double deltaTime);
    }
}