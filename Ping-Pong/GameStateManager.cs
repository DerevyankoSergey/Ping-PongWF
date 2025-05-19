using Ping_PongClassLibrary;
using System;
using System.Windows.Forms;

namespace Ping_Pong
{
    /// <summary>
    /// Управляет состояниями игры и координирует обновление логики и рендеринг.
    /// </summary>
    public class GameStateManager
    {
        private IGame game;
        private readonly GameRenderer renderer;
        private readonly UIManager uiManager;
        private readonly InputHandler inputHandler;
        private readonly Timer gameTimer;
        private GameState currentState;
        public GameState CurrentState => currentState;
        public IGame Game => game;
        public Timer GameTimer => gameTimer;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GameStateManager"/>.
        /// </summary>
        public GameStateManager(IGame game, GameRenderer renderer, UIManager uiManager, InputHandler inputHandler, Timer gameTimer)
        {
            this.game = game;
            this.renderer = renderer;
            this.uiManager = uiManager;
            this.inputHandler = inputHandler;
            this.gameTimer = gameTimer;
        }

        /// <summary>
        /// Заменяет текущий экземпляр игры на новый и обновляет связанные компоненты.
        /// </summary>
        public void ReplaceGame(IGame newGame)
        {
            game = newGame;
            renderer.UpdateGame(newGame);
            inputHandler.UpdateGame(newGame);
            uiManager.UpdateGame(newGame);
        }

        /// <summary>
        /// Устанавливает новое состояние игры и обновляет интерфейс и рендеринг.
        /// </summary>
        public void SetState(GameState newState)
        {
            currentState = newState;
            uiManager.UpdateControlVisibility(currentState);
            renderer.Render(currentState);
        }

        /// <summary>
        /// Обновляет игровую логику и рендеринг в зависимости от текущего состояния.
        /// </summary>
        public void Update(double deltaTime)
        {
            if (currentState == GameState.Playing)
            {
                uiManager.InputHandler.UpdatePaddlePositions(deltaTime);
                try
                {
                    game.Update(deltaTime);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка в game.Update: {ex.Message}");
                    throw;
                }
                var gameManager = game.GetGameManager();
                if (gameManager.GameOver)
                {
                    SetState(GameState.GameOver);
                }
            }
            renderer.Render(currentState);
        }
    }
}