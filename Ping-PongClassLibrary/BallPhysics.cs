using System;

namespace Ping_PongClassLibrary
{
    public class BallPhysics
    {
        private readonly BallMovement movement;
        private readonly PaddleCollisionHandler collisionHandler;
        private readonly BallServeManager serveManager;
        private readonly BallStateManager stateManager;
        private IPaddle player1Paddle;
        private IPaddle player2Paddle;

        public double X => movement.X;
        public double Y => movement.Y;
        public double Vx => movement.Vx;
        public double Vy => movement.Vy;
        public double Radius => movement.Radius;
        public bool IsPaused => stateManager.IsPaused;
        public double GetPauseTime => stateManager.GetPauseTime;
        public bool IsPausedDueToOutOfBounds => stateManager.IsPausedDueToOutOfBounds;
        public bool HasCollidedWithPaddle => collisionHandler.HasCollidedWithPaddle;
        public bool HasTouchedOpponentTable => stateManager.HasTouchedOpponentTable;
        public int LastPaddleHit => collisionHandler.LastPaddleHit;
        public bool IsOverNet => stateManager.IsOverNet;

        public struct BallState
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Vx { get; set; }
            public double Vy { get; set; }
            public bool HasTouchedOpponentTable { get; set; }
            public int LastPaddleHit { get; set; }
            public double Radius { get; set; }
            public bool IsOverNet { get; set; } // Добавлено поле
        }

        public event Action<BallState> OnPotentialScore
        {
            add => stateManager.OnPotentialScore += value;
            remove => stateManager.OnPotentialScore -= value;
        }

        public BallPhysics(double x, double y, double radius, IPaddle player1Paddle, IPaddle player2Paddle, Table table)
        {
            this.player1Paddle = player1Paddle;
            this.player2Paddle = player2Paddle;
            movement = new BallMovement(x, y, radius);
            collisionHandler = new PaddleCollisionHandler(movement, player1Paddle, player2Paddle);
            serveManager = new BallServeManager(movement, collisionHandler);
            stateManager = new BallStateManager(movement, collisionHandler, table);
        }

        public void Update(double deltaTime, int screenWidth, int screenHeight,
                           int tableLeft, int tableRight, int tableTop, int tableBottom,
                           bool isPlayer1Serving)
        {
            if (stateManager.IsPaused)
            {
                stateManager.UpdateState(deltaTime, screenWidth, screenHeight, tableLeft, tableRight, tableTop, tableBottom);
                if (!stateManager.IsPaused)
                    serveManager.ResetForServe(screenWidth, screenHeight, tableLeft, tableRight, tableTop, tableBottom, isPlayer1Serving);
                return;
            }

            if (serveManager.IsWaitingForServe)
            {
                if (serveManager.ServeByPaddle(isPlayer1Serving ? player1Paddle : player2Paddle, isPlayer1Serving))
                {
                    // Подача выполнена
                }
                return;
            }

            if (movement.Vx == 0 && movement.Vy == 0) return;

            double previousX = movement.PreviousX;
            double previousY = movement.PreviousY;
            movement.Update(deltaTime);
            collisionHandler.CheckCollisions(previousX, previousY, isPlayer1Serving);
            stateManager.UpdateState(deltaTime, screenWidth, screenHeight, tableLeft, tableRight, tableTop, tableBottom);
        }

        public void UpdatePaddles(IPaddle newPlayer1Paddle, IPaddle newPlayer2Paddle)
        {
            player1Paddle = newPlayer1Paddle;
            player2Paddle = newPlayer2Paddle;
            collisionHandler.UpdatePaddles(newPlayer1Paddle, newPlayer2Paddle);
        }

        public bool ServeByPaddle(IPaddle paddle, bool isLeftPaddle)
        {
            return serveManager.ServeByPaddle(paddle, isLeftPaddle);
        }

        public void PauseBall()
        {
            stateManager.PauseBall();
        }

        public void ResetForServe(int screenWidth, int screenHeight,
                                  int tableLeft, int tableRight,
                                  int tableTop, int tableBottom,
                                  bool isPlayer1Serving)
        {
            serveManager.ResetForServe(screenWidth, screenHeight, tableLeft, tableRight, tableTop, tableBottom, isPlayer1Serving);
        }

        public void Serve(bool isLeft)
        {
            serveManager.Serve(isLeft);
        }

        public void InitiateServe(bool isPlayer1Serving)
        {
            serveManager.InitiateServe(isPlayer1Serving);
        }
    }
}