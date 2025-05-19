using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using Ping_PongClassLibrary;

namespace Ping_Pong
{
    /// <summary>
    /// Управляет пользовательским интерфейсом и обработкой взаимодействий с элементами управления.
    /// </summary>
    public class UIManager
    {
        private readonly GLControl glControl;
        private readonly TextureManager textureManager;
        private readonly InputHandler inputHandler;
        private readonly GameRenderer renderer;
        private IGame game;
        private GameStateManager stateManager;

        public InputHandler InputHandler => inputHandler;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="UIManager"/>.
        /// </summary>
        public UIManager(GLControl glControl, TextureManager textureManager, InputHandler inputHandler, GameRenderer renderer, IGame game)
        {
            this.glControl = glControl;
            this.textureManager = textureManager;
            this.inputHandler = inputHandler;
            this.renderer = renderer;
            this.game = game;

            InitializeControls();
            glControl.MouseClick += HandleMouseClick;
        }

        /// <summary>
        /// Устанавливает менеджер состояний игры для координации действий.
        /// </summary>
        public void SetStateManager(GameStateManager manager)
        {
            stateManager = manager;
        }

        /// <summary>
        /// Обновляет экземпляр игры, используемый для взаимодействия.
        /// </summary>
        public void UpdateGame(IGame newGame)
        {
            game = newGame;
        }

        /// <summary>
        /// Инициализирует элементы управления, устанавливая их начальные параметры.
        /// </summary>
        private void InitializeControls()
        {
            glControl.SendToBack();
        }

        /// <summary>
        /// Обрабатывает действие кнопки перезапуска, сбрасывая игру и переходя в состояние игры.
        /// </summary>
        private void RestartButton_Click(object sender, EventArgs e)
        {
            stateManager.Game.GetGameManager().ResetGame();
            var newGame = new Game(glControl.Width, glControl.Height);
            inputHandler.Player1TargetY = newGame.GetPlayer1Paddle().Y;
            inputHandler.Player2TargetY = newGame.GetPlayer2Paddle().Y;
            stateManager.ReplaceGame(newGame);
            UpdateControlVisibility(GameState.Playing);
            glControl.Focus();
            stateManager.SetState(GameState.Playing);
            stateManager.GameTimer.Start();
        }

        /// <summary>
        /// Обрабатывает действие кнопки выхода, сбрасывая игру и возвращая в главное меню.
        /// </summary>
        private void ExitButton_Click(object sender, EventArgs e)
        {
            stateManager.Game.GetGameManager().ResetGame();
            var newGame = new Game(glControl.Width, glControl.Height);
            inputHandler.Player1TargetY = newGame.GetPlayer1Paddle().Y;
            inputHandler.Player2TargetY = newGame.GetPlayer2Paddle().Y;
            stateManager.ReplaceGame(newGame);
            UpdateControlVisibility(GameState.Menu);
            stateManager.SetState(GameState.Menu);
            stateManager.GameTimer.Stop();
        }

        /// <summary>
        /// Обновляет видимость элементов управления в зависимости от состояния игры.
        /// </summary>
        public void UpdateControlVisibility(GameState state)
        {
            glControl.Visible = true;
        }

        /// <summary>
        /// Обрабатывает событие клика мыши, взаимодействуя с элементами интерфейса.
        /// </summary>
        private void HandleMouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                double playTextureX = (glControl.Width - 150) / 2;
                double playTextureY = glControl.Height / 2 + 30;
                double playTextureWidth = 150;
                double playTextureHeight = 60;

                double exitTextureX = (glControl.Width - 150) / 2;
                double exitTextureY = glControl.Height / 2 + 120;
                double exitTextureWidth = 150;
                double exitTextureHeight = 60;

                double pauseTextureX = glControl.Width - 50 - 10;
                double pauseTextureY = 10;
                double pauseTextureWidth = 40;
                double pauseTextureHeight = 40;

                double continueTextureX = (glControl.Width - 150) / 2;
                double continueTextureY = glControl.Height / 2 - 60;
                double continueTextureWidth = 150;
                double continueTextureHeight = 60;

                double exitMenuTextureX = (glControl.Width - 150) / 2;
                double exitMenuTextureY = glControl.Height / 2 + 10;
                double exitMenuTextureWidth = 150;
                double exitMenuTextureHeight = 60;

                double restartTextureX = renderer.RestartTextureX;
                double restartTextureY = renderer.RestartTextureY;
                double restartTextureWidth = renderer.RestartTextureWidth;
                double restartTextureHeight = renderer.RestartTextureHeight;

                double gameOverExitTextureX = renderer.GameOverExitTextureX;
                double gameOverExitTextureY = renderer.GameOverExitTextureY;
                double gameOverExitTextureWidth = renderer.GameOverExitTextureWidth;
                double gameOverExitTextureHeight = renderer.GameOverExitTextureHeight;

                switch (stateManager.CurrentState)
                {
                    case GameState.Menu:
                        if (e.X >= playTextureX && e.X <= playTextureX + playTextureWidth &&
                            e.Y >= playTextureY && e.Y <= playTextureY + playTextureHeight)
                        {
                            stateManager.SetState(GameState.Playing);
                            stateManager.GameTimer.Start();
                        }
                        else if (e.X >= exitTextureX && e.X <= exitTextureX + exitTextureWidth &&
                                 e.Y >= exitTextureY && e.Y <= exitTextureY + exitTextureHeight)
                        {
                            if (glControl.Parent is Form form)
                            {
                                form.Close();
                            }
                        }
                        break;

                    case GameState.Playing:
                        if (e.X >= pauseTextureX && e.X <= pauseTextureX + pauseTextureWidth &&
                            e.Y >= pauseTextureY && e.Y <= pauseTextureY + pauseTextureHeight)
                        {
                            stateManager.SetState(GameState.Paused);
                            stateManager.GameTimer.Stop();
                        }
                        break;

                    case GameState.Paused:
                        if (e.X >= continueTextureX && e.X <= continueTextureX + continueTextureWidth &&
                            e.Y >= continueTextureY && e.Y <= continueTextureY + continueTextureHeight)
                        {
                            stateManager.SetState(GameState.Playing);
                            stateManager.GameTimer.Start();
                        }
                        else if (e.X >= exitMenuTextureX && e.X <= exitMenuTextureX + exitMenuTextureWidth &&
                                 e.Y >= exitMenuTextureY && e.Y <= exitMenuTextureY + exitMenuTextureHeight)
                        {
                            var newGame = new Game(glControl.Width, glControl.Height);
                            stateManager.ReplaceGame(newGame);
                            stateManager.Game.GetGameManager().ResetGame();
                            stateManager.SetState(GameState.Menu);
                            UpdateControlVisibility(GameState.Menu);
                            stateManager.GameTimer.Stop();
                        }
                        break;

                    case GameState.GameOver:
                        if (e.X >= restartTextureX && e.X <= restartTextureX + restartTextureWidth &&
                            e.Y >= restartTextureY && e.Y <= restartTextureY + restartTextureHeight)
                        {
                            RestartButton_Click(null, EventArgs.Empty);
                        }
                        else if (e.X >= gameOverExitTextureX && e.X <= gameOverExitTextureX + gameOverExitTextureWidth &&
                                 e.Y >= gameOverExitTextureY && e.Y <= gameOverExitTextureY + gameOverExitTextureHeight)
                        {
                            ExitButton_Click(null, EventArgs.Empty);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке клика мыши: {ex.Message}");
                throw;
            }
        }
    }
}