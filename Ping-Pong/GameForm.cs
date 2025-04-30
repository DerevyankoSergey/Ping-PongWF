using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Ping_PongClassLibrary;

namespace Ping_Pong
{
    public partial class GameForm : Form
    {
        private GLControl glControl;
        private Game game;
        private Timer gameTimer;
        private int[] textures = new int[48]; // Увеличиваем до 48 для IronPaddle1 и IronPaddle2
        private readonly string contentPath = Path.Combine(Application.StartupPath, "Content");
        private Button restartButton;
        private Button exitButton;
        private bool isMenuVisible = true;
        private bool isGameOver = false;

        // Состояние клавиш для игрока 1 (W, S)
        private bool isWPressed = false;
        private bool isSPressed = false;

        // Состояние клавиш для игрока 2 (NumPad8, NumPad2)
        private bool isNumPad8Pressed = false;
        private bool isNumPad2Pressed = false;

        // Состояние клавиш для подачи (Q, K)
        private bool isQPressed = false;
        private bool isKPressed = false;

        // Текущие целевые позиции для ракеток
        private double player1TargetY;
        private double player2TargetY;

        private const double paddleSpeed = 600.0;

        // Размеры и позиция текстуры "Play"
        private double playTextureWidth = 100;
        private double playTextureHeight = 50;
        private double playTextureX;
        private double playTextureY;

        // Состояние материала ракеток
        private bool isPlayer1PaddleIron = false;
        private bool isPlayer2PaddleIron = false;

        // Словари для маппинга букв на индексы текстур
        private readonly Dictionary<char, int> redLetterTextures = new Dictionary<char, int>
        {
            { 'B', 29 }, { 'D', 30 }, { 'E', 31 }, { 'I', 32 },
            { 'L', 33 }, { 'N', 34 }, { 'R', 35 }, { 'W', 36 }
        };

        private readonly Dictionary<char, int> blueLetterTextures = new Dictionary<char, int>
        {
            { 'B', 37 }, { 'E', 38 }, { 'I', 39 }, { 'L', 40 },
            { 'N', 41 }, { 'U', 42 }, { 'W', 43 }
        };

        public GameForm()
        {
            this.Text = "Пинг-Понг";
            this.ClientSize = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.DarkBlue;

            InitializeMenu();
            InitializeGameOverPanel();
            InitializeGLControl();
            InitializeGame();
            SetupTimer();
            UpdateControlVisibility();
        }

        private void InitializeMenu()
        {
            // Кнопка "Play" реализована как текстура в OpenGL
        }

        private void InitializeGameOverPanel()
        {
            restartButton = new Button
            {
                Text = "Рестарт",
                Size = new Size(150, 50),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Blue,
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            restartButton.FlatAppearance.BorderSize = 0;
            restartButton.Location = new Point(
                (this.ClientSize.Width - restartButton.Width) / 2,
                this.ClientSize.Height / 2 - 30
            );
            restartButton.Click += RestartButton_Click;

            exitButton = new Button
            {
                Text = "Выход",
                Size = new Size(100, 50),
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Red,
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Location = new Point(
                (this.ClientSize.Width - exitButton.Width) / 2,
                this.ClientSize.Height / 2 + 30
            );
            exitButton.Click += ExitButton_Click;

            this.Controls.Add(restartButton);
            this.Controls.Add(exitButton);
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            game.GetGameManager().ResetGame();
            game = new Game(glControl.Width, glControl.Height);
            player1TargetY = game.GetPlayer1Paddle().Y;
            player2TargetY = game.GetPlayer2Paddle().Y;
            isGameOver = false;
            isMenuVisible = false;
            // Сбрасываем состояние материала ракеток
            isPlayer1PaddleIron = false;
            isPlayer2PaddleIron = false;
            UpdateControlVisibility();
            glControl.Focus();
            gameTimer.Start();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            isGameOver = false;
            isMenuVisible = true;
            game.GetGameManager().ResetGame();
            game = new Game(glControl.Width, glControl.Height);
            player1TargetY = game.GetPlayer1Paddle().Y;
            player2TargetY = game.GetPlayer2Paddle().Y;
            // Сбрасываем состояние материала ракеток
            isPlayer1PaddleIron = false;
            isPlayer2PaddleIron = false;
            UpdateControlVisibility();
            gameTimer.Stop();
        }

        private void InitializeGLControl()
        {
            glControl = new GLControl(new GraphicsMode(32, 24, 0, 8));
            glControl.Dock = DockStyle.Fill;
            glControl.Paint += RenderScene;
            glControl.KeyDown += HandleInput;
            glControl.KeyUp += HandleInputRelease;
            glControl.MouseClick += HandleMouseClick;
            glControl.Load += (sender, e) =>
            {
                glControl.MakeCurrent();
                SetupViewport();
                LoadTextures();
            };
            this.Controls.Add(glControl);
            glControl.SendToBack();
            restartButton.BringToFront();
            exitButton.BringToFront();
        }

        private void InitializeGame()
        {
            game = new Game(glControl.Width, glControl.Height);
            player1TargetY = game.GetPlayer1Paddle().Y;
            player2TargetY = game.GetPlayer2Paddle().Y;
        }

        private void SetupTimer()
        {
            gameTimer = new Timer { Interval = 16 };
            gameTimer.Tick += (s, e) =>
            {
                if (!isMenuVisible)
                {
                    double deltaTime = 0.016;

                    if (isWPressed && !isSPressed)
                    {
                        player1TargetY -= paddleSpeed * deltaTime;
                    }
                    else if (isSPressed && !isWPressed)
                    {
                        player1TargetY += paddleSpeed * deltaTime;
                    }
                    else
                    {
                        player1TargetY = game.GetPlayer1Paddle().Y;
                    }

                    if (isNumPad8Pressed && !isNumPad2Pressed)
                    {
                        player2TargetY -= paddleSpeed * deltaTime;
                    }
                    else if (isNumPad2Pressed && !isNumPad8Pressed)
                    {
                        player2TargetY += paddleSpeed * deltaTime;
                    }
                    else
                    {
                        player2TargetY = game.GetPlayer2Paddle().Y;
                    }

                    player1TargetY = Math.Max(game.GetPlayer1Paddle().Height / 2,
                                             Math.Min(glControl.Height - game.GetPlayer1Paddle().Height / 2, player1TargetY));
                    player2TargetY = Math.Max(game.GetPlayer2Paddle().Height / 2,
                                             Math.Min(glControl.Height - game.GetPlayer2Paddle().Height / 2, player2TargetY));

                    game.MovePaddle1(player1TargetY);
                    game.MovePaddle2(player2TargetY);

                    if (!isGameOver)
                    {
                        // Сохраняем список призов до обновления
                        var prizesBefore = new List<IPrize>(game.GetPrizes());

                        // Обновляем состояние игры
                        game.Update(deltaTime);

                        // Получаем список призов после обновления
                        var prizesAfter = game.GetPrizes();

                        // Проверяем, был ли подобран приз материала
                        var paddle1 = game.GetPlayer1Paddle();
                        var paddle2 = game.GetPlayer2Paddle();
                        foreach (var prize in prizesBefore)
                        {
                            // Проверяем, если приз больше не существует в списке после обновления
                            if (!prizesAfter.Contains(prize) && prize.TextureId == 6) // Приз материала
                            {
                                // Проверяем столкновение с ракеткой игрока 1
                                if (!isPlayer1PaddleIron &&
                                    prize.X < paddle1.X + paddle1.Width &&
                                    prize.X + prize.Width > paddle1.X &&
                                    prize.Y < paddle1.Y + paddle1.Height / 2 &&
                                    prize.Y + prize.Height > paddle1.Y - paddle1.Height / 2)
                                {
                                    isPlayer1PaddleIron = true;
                                }
                                // Проверяем столкновение с ракеткой игрока 2
                                else if (!isPlayer2PaddleIron &&
                                         prize.X < paddle2.X + paddle2.Width &&
                                         prize.X + prize.Width > paddle2.X &&
                                         prize.Y < paddle2.Y + paddle2.Height / 2 &&
                                         prize.Y + prize.Height > paddle2.Y - paddle2.Height / 2)
                                {
                                    isPlayer2PaddleIron = true;
                                }
                            }
                        }

                        // Проверяем, истекло ли действие приза материала
                        bool wasPlayer1Iron = isPlayer1PaddleIron;
                        bool wasPlayer2Iron = isPlayer2PaddleIron;
                        isPlayer1PaddleIron = false;
                        isPlayer2PaddleIron = false;

                        foreach (var (prize, _, _) in game.GetType().GetField("player1ActivePrizes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(game) as List<(IPrize Prize, double ActivationTime, double Duration)> ?? new List<(IPrize, double, double)>())
                        {
                            if (prize is ChangeMaterialPrize)
                            {
                                isPlayer1PaddleIron = true;
                                break;
                            }
                        }

                        foreach (var (prize, _, _) in game.GetType().GetField("player2ActivePrizes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(game) as List<(IPrize Prize, double ActivationTime, double Duration)> ?? new List<(IPrize, double, double)>())
                        {
                            if (prize is ChangeMaterialPrize)
                            {
                                isPlayer2PaddleIron = true;
                                break;
                            }
                        }

                        glControl.Invalidate();
                    }

                    if (game.GetGameManager().GameOver && !isGameOver)
                    {
                        isGameOver = true;
                        UpdateControlVisibility();
                        glControl.Invalidate();
                    }
                }
            };
        }

        private void UpdateControlVisibility()
        {
            glControl.Visible = true;
            restartButton.Visible = isGameOver;
            exitButton.Visible = isGameOver;
            glControl.Invalidate();
        }

        private void SetupViewport()
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.ClearColor(0.0f, 0.0f, 0.5f, 1.0f);
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, glControl.Width, glControl.Height, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            CheckGLError("SetupViewport");
        }

        private void LoadTextures()
        {
            if (!Directory.Exists(contentPath))
                Directory.CreateDirectory(contentPath);

            GL.GenTextures(48, textures);
            LoadTextureFromFile(textures[0], "table.png", Color.DarkGreen);
            LoadTextureFromFile(textures[1], "paddle1.png", Color.White);
            LoadTextureFromFile(textures[2], "paddle2.png", Color.White);
            LoadTextureFromFile(textures[3], "ball.png", Color.Red);
            LoadTextureFromFile(textures[4], "prize_length.png", Color.Blue);
            LoadTextureFromFile(textures[5], "prize_width.png", Color.Green);
            LoadTextureFromFile(textures[6], "prize_material.png", Color.Yellow);
            LoadTextureFromFile(textures[7], "tablo.png", Color.Black);
            LoadTextureFromFile(textures[8], "menu_background.png", Color.Yellow);

            string[] redDigitNames = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            for (int i = 0; i < 10; i++)
                LoadTextureFromFile(textures[9 + i], $"{redDigitNames[i]}.png", Color.White);

            string[] blueDigitNames = { "zeroBlue", "oneBlue", "twoBlue", "threeBlue", "fourBlue", "fiveBlue", "sixBlue", "sevenBlue", "eightBlue", "nineBlue" };
            for (int i = 0; i < 10; i++)
                LoadTextureFromFile(textures[19 + i], $"{blueDigitNames[i]}.png", Color.White);

            string[] redLetters = { "B", "D", "E", "I", "L", "N", "R", "W" };
            for (int i = 0; i < redLetters.Length; i++)
                LoadTextureFromFile(textures[29 + i], $"{redLetters[i]}.png", Color.White);

            string[] blueLetters = { "B", "E", "I", "L", "N", "U", "W" };
            for (int i = 0; i < blueLetters.Length; i++)
                LoadTextureFromFile(textures[37 + i], $"{blueLetters[i]}Blue.png", Color.White);

            LoadTextureFromFile(textures[44], "gameover_background.png", Color.Gray);
            LoadTextureFromFile(textures[45], "play.png", Color.Green);

            // Загружаем текстуры для "железных" ракеток
            LoadTextureFromFile(textures[46], "IronPaddle1.png", Color.Gray);
            LoadTextureFromFile(textures[47], "IronPaddle2.png", Color.Gray);
        }

        private void LoadTextureFromFile(int textureId, string filename, Color fallbackColor)
        {
            string filePath = Path.Combine(contentPath, filename);
            Bitmap bitmap = null;

            if (File.Exists(filePath))
            {
                try
                {
                    bitmap = new Bitmap(filePath);
                }
                catch (Exception ex)
                {
                    bitmap = CreateColorTexture(fallbackColor, 64, 64);
                }
            }
            else
            {
                bitmap = CreateColorTexture(fallbackColor, 64, 64);
                bitmap.Save(filePath);
            }

            if (bitmap != null)
            {
                LoadTexture(textureId, bitmap);
                bitmap.Dispose();
            }
        }

        private Bitmap CreateColorTexture(Color color, int width, int height)
        {
            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(color);
                using (var pen = new Pen(Color.FromArgb(128, Color.Black), 2))
                {
                    if (color == Color.DarkGreen)
                    {
                        for (int x = 0; x < width; x += 10)
                            g.DrawLine(pen, x, 0, x, height);
                        for (int y = 0; y < height; y += 10)
                            g.DrawLine(pen, 0, y, width, y);
                    }
                    else if (color == Color.White || color == Color.Gray)
                    {
                        g.DrawRectangle(pen, 2, 2, width - 4, height - 4);
                    }
                    else if (color == Color.Red)
                    {
                        g.DrawEllipse(pen, 2, 2, width - 4, height - 4);
                    }
                }
            }
            return bmp;
        }

        private void LoadTexture(int textureId, Bitmap bitmap)
        {
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                         data.Width, data.Height, 0,
                         OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                         PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            int width, height;
            GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureWidth, out width);
            GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureHeight, out height);
            CheckGLError($"LoadTexture ID={textureId}");
        }

        private void CheckGLError(string context)
        {
            ErrorCode error = GL.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception($"Ошибка OpenGL в {context}: {(int)error} ({error})");
        }

        private void DrawTexturedQuad(double x, double y, double width, double height, int textureId)
        {
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            int boundTexture;
            GL.GetInteger(GetPName.TextureBinding2D, out boundTexture);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(x, y);
            GL.TexCoord2(1, 0); GL.Vertex2(x + width, y);
            GL.TexCoord2(1, 1); GL.Vertex2(x + width, y + height);
            GL.TexCoord2(0, 1); GL.Vertex2(x, y + height);
            GL.End();
            CheckGLError("DrawTexturedQuad");
        }

        private void DrawText(string text, double x, double y, bool isRed, double letterWidth, double letterHeight)
        {
            double currentX = x;
            var letterTextures = isRed ? redLetterTextures : blueLetterTextures;

            foreach (char c in text.ToUpper())
            {
                if (c == ' ')
                {
                    currentX += letterWidth;
                    continue;
                }

                if (letterTextures.TryGetValue(c, out int textureId))
                {
                    DrawTexturedQuad(currentX, y, letterWidth, letterHeight, textures[textureId]);
                    currentX += letterWidth;
                }
            }
        }

        private void RenderScene(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (isMenuVisible)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.ClearColor(0.0f, 0.0f, 0.5f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                DrawTexturedQuad(0, 0, glControl.Width, glControl.Height, textures[8]);

                playTextureX = (glControl.Width - playTextureWidth) / 2;
                playTextureY = (glControl.Height / 2) + 20;
                DrawTexturedQuad(playTextureX, playTextureY,
                                 playTextureWidth, playTextureHeight, textures[45]);

                CheckGLError("RenderScene Menu");
            }
            else if (isGameOver)
            {
                GL.ClearColor(0.0f, 0.0f, 0.5f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                DrawTexturedQuad(0, 0, glControl.Width, glControl.Height, textures[44]);

                string winnerText = game.GetGameManager().Winner == 1 ? "RED WIN" : "BLUE WIN";
                bool isRed = game.GetGameManager().Winner == 1;
                double letterWidth = 50;
                double letterHeight = 100;
                double textX = (glControl.Width - (winnerText.Length * letterWidth)) / 2;
                double textY = glControl.Height / 4;
                DrawText(winnerText, textX, textY, isRed, letterWidth, letterHeight);

                CheckGLError("RenderScene GameOver");
            }
            else
            {
                GL.ClearColor(0.133f, 0.125f, 0.204f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                DrawTexturedQuad(game.GetTable().Left, game.GetTable().Top,
                                 game.GetTable().Width, game.GetTable().Height,
                                 textures[0]);

                var paddle1 = game.GetPlayer1Paddle();
                DrawTexturedQuad(paddle1.X, paddle1.Y - paddle1.Height / 2,
                                 paddle1.Width, paddle1.Height,
                                 isPlayer1PaddleIron ? textures[46] : textures[1]); // Используем IronPaddle1, если активен приз

                var paddle2 = game.GetPlayer2Paddle();
                DrawTexturedQuad(paddle2.X, paddle2.Y - paddle2.Height / 2,
                                 paddle2.Width, paddle2.Height,
                                 isPlayer2PaddleIron ? textures[47] : textures[2]); // Используем IronPaddle2, если активен приз

                var ball = game.GetBall();
                if (!ball.IsPaused)
                {
                    DrawTexturedQuad(ball.X - ball.Radius, ball.Y - ball.Radius,
                                     ball.Radius * 2, ball.Radius * 2,
                                     textures[3]);
                }

                foreach (var prize in game.GetPrizes())
                {
                    DrawTexturedQuad(prize.X, prize.Y,
                                     prize.Width, prize.Height,
                                     textures[prize.TextureId]);
                }

                double tabloWidth = 100;
                double tabloHeight = 80;
                double tabloX = (glControl.Width - tabloWidth) / 2;
                double tabloY = 0;
                DrawTexturedQuad(tabloX, tabloY, tabloWidth, tabloHeight, textures[7]);

                var score1 = game.GetGameManager().Score1;
                var score2 = game.GetGameManager().Score2;
                double digitWidth = 15;
                double digitHeight = 30;
                double digitY = 3;
                double leftMargin = 12;
                double rightMargin = 27;
                double digitSpacing = 0;

                if (score1 >= 10)
                {
                    int firstDigit = score1 / 10;
                    int secondDigit = score1 % 10;
                    double score1X1 = tabloX + leftMargin;
                    double score1X2 = score1X1 + digitWidth + digitSpacing;
                    DrawTexturedQuad(score1X1, digitY, digitWidth, digitHeight, textures[9 + firstDigit]);
                    DrawTexturedQuad(score1X2, digitY, digitWidth, digitHeight, textures[9 + secondDigit]);
                }
                else
                {
                    double score1X = tabloX + leftMargin + (digitWidth / 2);
                    DrawTexturedQuad(score1X, digitY, digitWidth, digitHeight, textures[9 + score1]);
                }

                if (score2 >= 10)
                {
                    int firstDigit = score2 / 10;
                    int secondDigit = score2 % 10;
                    double score2X2 = tabloX + tabloWidth - rightMargin - digitWidth;
                    double score2X1 = score2X2 - digitWidth - digitSpacing;
                    DrawTexturedQuad(score2X1, digitY, digitWidth, digitHeight, textures[19 + firstDigit]);
                    DrawTexturedQuad(score2X2, digitY, digitWidth, digitHeight, textures[19 + secondDigit]);
                }
                else
                {
                    double score2X = tabloX + tabloWidth - rightMargin - (digitWidth / 2);
                    DrawTexturedQuad(score2X, digitY, digitWidth, digitHeight, textures[19 + score2]);
                }

                using (Graphics g = e.Graphics)
                {
                    if (ball.IsPaused && ball.GetPauseTime > 0 && !ball.IsPausedDueToOutOfBounds)
                    {
                        var font = new Font("Arial", 48, FontStyle.Bold);
                        var brush = Brushes.White;
                        var shadowBrush = Brushes.Black;
                        string timerText = Math.Ceiling(ball.GetPauseTime).ToString();
                        SizeF timerSize = g.MeasureString(timerText, font);
                        float timerX = (glControl.Width - timerSize.Width) / 2;
                        float timerY = (glControl.Height - timerSize.Height) / 2;
                        g.DrawString(timerText, font, shadowBrush, timerX + 2, timerY + 2);
                        g.DrawString(timerText, font, brush, timerX, timerY);
                    }
                }

                CheckGLError("RenderScene Game");
            }

            glControl.SwapBuffers();
        }

        private void HandleMouseClick(object sender, MouseEventArgs e)
        {
            if (isMenuVisible)
            {
                if (e.X >= playTextureX && e.X <= playTextureX + playTextureWidth &&
                    e.Y >= playTextureY && e.Y <= playTextureY + playTextureHeight)
                {
                    isMenuVisible = false;
                    isGameOver = false;
                    UpdateControlVisibility();
                    glControl.Focus();
                    gameTimer.Start();
                }
            }
        }

        private void HandleInput(object sender, KeyEventArgs e)
        {
            if (isMenuVisible || isGameOver)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.W:
                    isWPressed = true;
                    break;
                case Keys.S:
                    isSPressed = true;
                    break;
                case Keys.NumPad8:
                    isNumPad8Pressed = true;
                    break;
                case Keys.NumPad2:
                    isNumPad2Pressed = true;
                    break;
                case Keys.Q:
                    if (!isQPressed && game.GetGameManager().IsPlayer1Turn)
                    {
                        game.StrikePaddle1();
                        game.TryServe(true);
                        isQPressed = true;
                    }
                    break;
                case Keys.NumPad5:
                    if (!isKPressed && !game.GetGameManager().IsPlayer1Turn)
                    {
                        game.StrikePaddle2();
                        game.TryServe(false);
                        isKPressed = true;
                    }
                    break;
            }
        }

        private void HandleInputRelease(object sender, KeyEventArgs e)
        {
            if (isMenuVisible || isGameOver) return;

            switch (e.KeyCode)
            {
                case Keys.W:
                    isWPressed = false;
                    break;
                case Keys.S:
                    isSPressed = false;
                    break;
                case Keys.NumPad8:
                    isNumPad8Pressed = false;
                    break;
                case Keys.NumPad2:
                    isNumPad2Pressed = false;
                    break;
                case Keys.Q:
                    isQPressed = false;
                    break;
                case Keys.NumPad5:
                    isKPressed = false;
                    break;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            gameTimer.Stop();
            GL.DeleteTextures(48, textures);
        }
    }
}