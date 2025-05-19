using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Ping_PongClassLibrary;

namespace Ping_Pong
{
    /// <summary>
    /// Основная форма приложения для игры Пинг-Понг, управляющая окном и его компонентами.
    /// </summary>
    public partial class GameForm : Form
    {
        private readonly IGame game;
        private readonly GLControl glControl;
        private readonly GameRenderer renderer;
        private readonly InputHandler inputHandler;
        private readonly GameStateManager stateManager;
        private readonly TextureManager textureManager;
        private readonly UIManager uiManager;
        private readonly Timer gameTimer = new Timer { Interval = 16 };

        /// <summary>
        /// Инициализирует новый экземпляр класса GameForm с настройкой окна и его компонентов.
        /// </summary>
        public GameForm()
        {
            Text = "Пинг-Понг";
            ClientSize = new Size(800, 600);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.DarkBlue;

            glControl = new GLControl { Dock = DockStyle.Fill };
            Controls.Add(glControl);

            textureManager = new TextureManager(glControl, Application.StartupPath);
            game = new Game(glControl.Width, glControl.Height);
            renderer = new GameRenderer(glControl, textureManager, game);
            inputHandler = new InputHandler(glControl, game);
            uiManager = new UIManager(glControl, textureManager, inputHandler, renderer, game); // Передаем game
            stateManager = new GameStateManager(game, renderer, uiManager, inputHandler, gameTimer);

            uiManager.SetStateManager(stateManager);
            InitializeEvents();
            Shown += (s, e) => glControl.Refresh();
        }

        /// <summary>
        /// Инициализирует события для управления формой и её компонентами.
        /// </summary>
        private void InitializeEvents()
        {
            glControl.Load += (s, e) =>
            {
                if (glControl.IsHandleCreated)
                {
                    glControl.MakeCurrent();
                    renderer.SetupViewport();
                    textureManager.LoadTextures();
                    stateManager.SetState(GameState.Menu);
                    gameTimer.Start();
                }
            };
            gameTimer.Tick += (s, e) => stateManager.Update(0.016);
            FormClosing += (s, e) => textureManager.Cleanup();
        }
    }
}