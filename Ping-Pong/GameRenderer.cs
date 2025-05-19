using System;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Ping_PongClassLibrary;

namespace Ping_Pong
{
    /// <summary>
    /// Отвечает за рендеринг игровых элементов и интерфейса с использованием OpenGL.
    /// </summary>
    public class GameRenderer
    {
        private readonly GLControl glControl;
        private readonly TextureManager textureManager;
        private IGame game;
        private bool isSetupDone = false;
        private double _playTextureX, _playTextureY, _playTextureWidth = 150, _playTextureHeight = 60;
        private double _exitTextureX, _exitTextureY, _exitTextureWidth = 150, _exitTextureHeight = 60;
        private double _pauseTextureX, _pauseTextureY, _pauseTextureWidth = 40, _pauseTextureHeight = 40;
        private double _continueTextureX, _continueTextureY, _continueTextureWidth = 150, _continueTextureHeight = 60;
        private double _exitMenuTextureX, _exitMenuTextureY, _exitMenuTextureWidth = 150, _exitMenuTextureHeight = 60;
        private double _pauseMenuTextureX, _pauseMenuTextureY, _pauseMenuTextureWidth = 400, _pauseMenuTextureHeight = 300;
        private double _restartTextureX, _restartTextureY, _restartTextureWidth = 150, _restartTextureHeight = 60;
        private double _gameOverExitTextureX, _gameOverExitTextureY, _gameOverExitTextureWidth = 150, _gameOverExitTextureHeight = 60;
        public double ContinueTextureX => _continueTextureX;
        public double ContinueTextureY => _continueTextureY;
        public double ContinueTextureWidth => _continueTextureWidth;
        public double ContinueTextureHeight => _continueTextureHeight;
        public double ExitMenuTextureX => _exitMenuTextureX;
        public double ExitMenuTextureY => _exitMenuTextureY;
        public double ExitMenuTextureWidth => _exitMenuTextureWidth;
        public double ExitMenuTextureHeight => _exitMenuTextureHeight;
        public double RestartTextureX => _restartTextureX;
        public double RestartTextureY => _restartTextureY;
        public double RestartTextureWidth => _restartTextureWidth;
        public double RestartTextureHeight => _restartTextureHeight;
        public double GameOverExitTextureX => _gameOverExitTextureX;
        public double GameOverExitTextureY => _gameOverExitTextureY;
        public double GameOverExitTextureWidth => _gameOverExitTextureWidth;
        public double GameOverExitTextureHeight => _gameOverExitTextureHeight;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GameRenderer"/>.
        /// </summary>
        public GameRenderer(GLControl glControl, TextureManager textureManager, IGame game)
        {
            this.glControl = glControl;
            this.textureManager = textureManager;
            this.game = game;
        }

        /// <summary>
        /// Обновляет экземпляр игры, используемый для рендеринга.
        /// </summary>
        public void UpdateGame(IGame newGame)
        {
            game = newGame;
        }

        /// <summary>
        /// Настраивает область просмотра и параметры проекции OpenGL.
        /// </summary>
        public void SetupViewport()
        {
            if (!glControl.IsHandleCreated)
            {
                return;
            }
            try
            {
                glControl.MakeCurrent();
                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.ClearColor(0.0f, 0.0f, 0.5f, 1.0f);
                GL.Viewport(0, 0, glControl.Width, glControl.Height);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, glControl.Width, glControl.Height, 0, -1, 1);
                GL.MatrixMode(MatrixMode.Modelview);
                isSetupDone = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в SetupViewport: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Отрисовывает игровую сцену в зависимости от текущего состояния игры.
        /// </summary>
        public void Render(GameState state)
        {
            if (!isSetupDone || !textureManager.TexturesLoaded)
            {
                glControl.MakeCurrent();
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.Color3(1.0f, 0.0f, 0.0f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(0, 0);
                GL.Vertex2(glControl.Width, 0);
                GL.Vertex2(glControl.Width, glControl.Height);
                GL.Vertex2(0, glControl.Height);
                GL.End();
                glControl.SwapBuffers();
                return;
            }

            try
            {
                glControl.MakeCurrent();
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                switch (state)
                {
                    case GameState.Menu:
                        DrawMenu();
                        break;
                    case GameState.Playing:
                        DrawGameElements();
                        DrawPauseButton();
                        break;
                    case GameState.Paused:
                        DrawGameElements();
                        DrawSemiTransparentOverlay();
                        DrawPausedMenu();
                        break;
                    case GameState.GameOver:
                        DrawTexturedQuad(0, 0, glControl.Width, glControl.Height, textureManager.Textures[(int)TextureId.GameOverBackground]);
                        var gameManager = game.GetGameManager();
                        string winner;
                        bool isRed;
                        if (gameManager.Winner == 1)
                        {
                            winner = "RED WIN";
                            isRed = true;
                        }
                        else if (gameManager.Winner == 2)
                        {
                            winner = "BLUE WIN";
                            isRed = false;
                        }
                        else
                        {
                            winner = gameManager.Score1 > gameManager.Score2 ? "RED WIN" : "BLUE WIN";
                            isRed = gameManager.Score1 > gameManager.Score2;
                        }
                        DrawText(winner, (glControl.Width - winner.Length * 50) / 2, glControl.Height / 4, isRed, 50, 100);
                        DrawGameOverMenu();
                        break;
                }

                glControl.SwapBuffers();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в Render: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Отрисовывает главное меню с кнопками "Играть" и "Выход".
        /// </summary>
        private void DrawMenu()
        {
            DrawTexturedQuad(0, 0, glControl.Width, glControl.Height, textureManager.Textures[(int)TextureId.MenuBackground]);
            _playTextureX = (glControl.Width - _playTextureWidth) / 2;
            _playTextureY = glControl.Height / 2 - 30 + 60;
            _exitTextureX = (glControl.Width - _exitTextureWidth) / 2;
            _exitTextureY = glControl.Height / 2 + 60 + 60;
            DrawTexturedQuad(_playTextureX, _playTextureY, _playTextureWidth, _playTextureHeight, textureManager.Textures[(int)TextureId.Play]);
            DrawTexturedQuad(_exitTextureX, _exitTextureY, _exitTextureWidth, _exitTextureHeight, textureManager.Textures[(int)TextureId.Exit]);
        }

        /// <summary>
        /// Отрисовывает кнопку паузы в правом верхнем углу во время игры.
        /// </summary>
        private void DrawPauseButton()
        {
            _pauseTextureX = glControl.Width - _pauseTextureWidth - 10;
            _pauseTextureY = 10;
            int pauseTextureId = textureManager.Textures[(int)TextureId.Pause];
            if (pauseTextureId > 0)
            {
                DrawTexturedQuad(_pauseTextureX, _pauseTextureY, _pauseTextureWidth, _pauseTextureHeight, pauseTextureId);
            }
        }

        /// <summary>
        /// Отрисовывает меню паузы с кнопками "Продолжить" и "Выйти в меню".
        /// </summary>
        private void DrawPausedMenu()
        {
            _pauseMenuTextureX = (glControl.Width - _pauseMenuTextureWidth) / 2;
            _pauseMenuTextureY = (glControl.Height - _pauseMenuTextureHeight) / 2;
            DrawTexturedQuad(_pauseMenuTextureX, _pauseMenuTextureY, _pauseMenuTextureWidth, _pauseMenuTextureHeight, textureManager.Textures[(int)TextureId.PauseMenu]);
            _continueTextureX = (glControl.Width - _continueTextureWidth) / 2;
            _continueTextureY = glControl.Height / 2 - 60;
            _exitMenuTextureX = (glControl.Width - _exitMenuTextureWidth) / 2;
            _exitMenuTextureY = glControl.Height / 2 + 10;
            DrawTexturedQuad(_continueTextureX, _continueTextureY, _continueTextureWidth, _continueTextureHeight, textureManager.Textures[(int)TextureId.Continue]);
            DrawTexturedQuad(_exitMenuTextureX, _exitMenuTextureY, _exitMenuTextureWidth, _exitMenuTextureHeight, textureManager.Textures[(int)TextureId.ExitMenu]);
        }

        /// <summary>
        /// Отрисовывает меню окончания игры с кнопками "Рестарт" и "Выход".
        /// </summary>
        private void DrawGameOverMenu()
        {
            _restartTextureX = (glControl.Width - _restartTextureWidth) / 2;
            _restartTextureY = glControl.Height / 2 - 10;
            _gameOverExitTextureX = (glControl.Width - _gameOverExitTextureWidth) / 2;
            _gameOverExitTextureY = _restartTextureY + _restartTextureHeight + 10;

            int restartTextureId = textureManager.Textures[(int)TextureId.Restart];
            if (restartTextureId <= 0)
            {
                GL.Disable(EnableCap.Texture2D);
                GL.Color3(1.0f, 0.0f, 0.0f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(_restartTextureX, _restartTextureY);
                GL.Vertex2(_restartTextureX + _restartTextureWidth, _restartTextureY);
                GL.Vertex2(_restartTextureX + _restartTextureWidth, _restartTextureY + _restartTextureHeight);
                GL.Vertex2(_restartTextureX, _restartTextureY + _restartTextureHeight);
                GL.End();
                GL.Color3(1.0f, 1.0f, 1.0f);
                GL.Enable(EnableCap.Texture2D);
            }
            else
            {
                DrawTexturedQuad(_restartTextureX, _restartTextureY, _restartTextureWidth, _restartTextureHeight, restartTextureId);
            }

            int exitTextureId = textureManager.Textures[(int)TextureId.Exit];
            if (exitTextureId > 0)
            {
                DrawTexturedQuad(_gameOverExitTextureX, _gameOverExitTextureY, _gameOverExitTextureWidth, _gameOverExitTextureHeight, exitTextureId);
            }
        }

        /// <summary>
        /// Отрисовывает все игровые элементы, включая стол, ракетки, мяч, призы и табло.
        /// </summary>
        public void DrawGameElements()
        {
            var table = game.GetTable();
            DrawTexturedQuad(table.Left, table.Top, table.Width, table.Height, textureManager.Textures[(int)TextureId.Table]);
            var p1 = game.GetPlayer1Paddle();
            int p1TextureId = game.Player1ActivePrizes.Any(p => p.Prize is ChangeMaterialPrize) ?
                textureManager.Textures[(int)TextureId.IronPaddle1] : textureManager.Textures[(int)TextureId.Paddle1];
            DrawTexturedQuad(p1.X, p1.Y - p1.Height / 2, p1.Width, p1.Height, p1TextureId);
            var p2 = game.GetPlayer2Paddle();
            int p2TextureId = game.Player2ActivePrizes.Any(p => p.Prize is ChangeMaterialPrize) ?
                textureManager.Textures[(int)TextureId.IronPaddle2] : textureManager.Textures[(int)TextureId.Paddle2];
            DrawTexturedQuad(p2.X, p2.Y - p2.Height / 2, p2.Width, p2.Height, p2TextureId);
            var ball = game.GetBall();
            double scaledWidth = ball.Radius * 2 * ball.ScaleX;
            double scaledHeight = ball.Radius * 2 * ball.ScaleY;
            DrawTexturedQuad(ball.X - ball.Radius * ball.ScaleX, ball.Y - ball.Radius * ball.ScaleY,
                scaledWidth, scaledHeight, textureManager.Textures[(int)TextureId.Ball]);
            foreach (var prize in game.GetPrizes())
                DrawTexturedQuad(prize.X, prize.Y, prize.Width, prize.Height, textureManager.Textures[prize.TextureId]);
            DrawScoreBoard();
        }

        /// <summary>
        /// Отрисовывает табло с очками игроков.
        /// </summary>
        private void DrawScoreBoard()
        {
            double w = 100, h = 80;
            DrawTexturedQuad((glControl.Width - w) / 2, 0, w, h, textureManager.Textures[(int)TextureId.Scoreboard]);
            var s1 = game.GetGameManager().Score1;
            var s2 = game.GetGameManager().Score2;
            double dw = 15, dh = 30, y = 3;
            double centerX = glControl.Width / 2;
            double halfScoreboardWidth = w / 2;
            if (s1 >= 10)
            {
                double x1 = centerX - halfScoreboardWidth + 12;
                DrawTexturedQuad(x1, y, dw, dh, textureManager.Textures[(int)TextureId.RedDigit0 + s1 / 10]);
                DrawTexturedQuad(x1 + dw, y, dw, dh, textureManager.Textures[(int)TextureId.RedDigit0 + s1 % 10]);
            }
            else
            {
                double x1 = centerX - halfScoreboardWidth + 12 + dw / 2;
                DrawTexturedQuad(x1, y, dw, dh, textureManager.Textures[(int)TextureId.RedDigit0 + s1]);
            }
            if (s2 >= 10)
            {
                double x2 = centerX + 8;
                DrawTexturedQuad(x2, y, dw, dh, textureManager.Textures[(int)TextureId.BlueDigit0 + s2 / 10]);
                DrawTexturedQuad(x2 + dw, y, dw, dh, textureManager.Textures[(int)TextureId.BlueDigit0 + s2 % 10]);
            }
            else
            {
                double x2 = centerX + 8 + dw / 2;
                DrawTexturedQuad(x2, y, dw, dh, textureManager.Textures[(int)TextureId.BlueDigit0 + s2]);
            }
        }

        /// <summary>
        /// Отрисовывает текстурированный прямоугольник в указанной позиции и размере.
        /// </summary>
        public void DrawTexturedQuad(double x, double y, double w, double h, int texId)
        {
            if (!isSetupDone || !textureManager.TexturesLoaded)
            {
                return;
            }

            try
            {
                if (texId <= 0)
                {
                    return;
                }

                GL.BindTexture(TextureTarget.Texture2D, texId);
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 0); GL.Vertex2(x, y);
                GL.TexCoord2(1, 0); GL.Vertex2(x + w, y);
                GL.TexCoord2(1, 1); GL.Vertex2(x + w, y + h);
                GL.TexCoord2(0, 1); GL.Vertex2(x, y + h);
                GL.End();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в DrawTexturedQuad (texId={texId}): {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Отрисовывает текст с использованием текстур букв.
        /// </summary>
        public void DrawText(string text, double x, double y, bool isRed, double w, double h)
        {
            var dict = isRed ? textureManager.RedLetterTextures : textureManager.BlueLetterTextures;
            foreach (char c in text.ToUpper())
            {
                if (c == ' ') { x += w; continue; }
                if (dict.TryGetValue(c, out TextureId id))
                {
                    DrawTexturedQuad(x, y, w, h, textureManager.Textures[(int)id]);
                    x += w;
                }
            }
        }

        /// <summary>
        /// Отрисовывает полупрозрачный оверлей для меню паузы.
        /// </summary>
        /// <exception cref="Exception">Выбрасывается при сбое рендеринга OpenGL.</exception>
        public void DrawSemiTransparentOverlay()
        {
            if (!isSetupDone)
            {
                return;
            }

            try
            {
                GL.Disable(EnableCap.Texture2D);
                GL.Color4(0.0f, 0.0f, 0.0f, 0.7f);
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(0, 0);
                GL.Vertex2(glControl.Width, 0);
                GL.Vertex2(glControl.Width, glControl.Height);
                GL.Vertex2(0, glControl.Height);
                GL.End();
                GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);
                GL.Enable(EnableCap.Texture2D);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в DrawSemiTransparentOverlay: {ex.Message}");
                throw;
            }
        }
    }
}