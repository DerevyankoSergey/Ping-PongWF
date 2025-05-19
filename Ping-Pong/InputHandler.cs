using System;
using System.Windows.Forms;
using OpenTK;
using Ping_PongClassLibrary;

namespace Ping_Pong
{
    /// <summary>
    /// Обрабатывает ввод пользователя для управления ракетками и подачей мяча в игре.
    /// </summary>
    public class InputHandler
    {
        private readonly GLControl glControl;
        private IGame game;
        public bool IsWPressed { get; set; }
        public bool IsSPressed { get; set; }
        public bool IsNumPad8Pressed { get; set; }
        public bool IsNumPad2Pressed { get; set; }
        public bool IsQPressed { get; set; }
        public bool IsKPressed { get; set; }
        public double Player1TargetY { get; set; }
        public double Player2TargetY { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="InputHandler"/>.
        /// </summary>
        public InputHandler(GLControl glControl, IGame game)
        {
            this.glControl = glControl;
            this.game = game;
            Player1TargetY = game.GetPlayer1Paddle().Y;
            Player2TargetY = game.GetPlayer2Paddle().Y;

            glControl.KeyDown += HandleKeyDown;
            glControl.KeyUp += HandleKeyUp;
        }

        /// <summary>
        /// Обновляет экземпляр игры и сбрасывает целевые координаты ракеток.
        /// </summary>
        public void UpdateGame(IGame newGame)
        {
            game = newGame;
            Player1TargetY = game.GetPlayer1Paddle().Y;
            Player2TargetY = game.GetPlayer2Paddle().Y;
        }

        /// <summary>
        /// Обрабатывает событие нажатия клавиши.
        /// </summary>
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    IsWPressed = true;
                    break;
                case Keys.S:
                    IsSPressed = true;
                    break;
                case Keys.NumPad8:
                    IsNumPad8Pressed = true;
                    break;
                case Keys.NumPad2:
                    IsNumPad2Pressed = true;
                    break;
                case Keys.Q:
                    if (!IsQPressed && game.GetGameManager().IsPlayer1Turn)
                    {
                        try
                        {
                            var ball = game.GetBall();
                            if (ball.Vx == 0 && ball.Vy == 0)
                            {
                                game.StrikePaddle1();
                                game.TryServe(true);
                                game.GetBall().TriggerServeAnimation();
                            }
                            IsQPressed = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при ударе (игрок 1): {ex.Message}");
                            throw;
                        }
                    }
                    break;
                case Keys.NumPad5:
                    if (!IsKPressed && !game.GetGameManager().IsPlayer1Turn)
                    {
                        try
                        {
                            var ball = game.GetBall();
                            if (ball.Vx == 0 && ball.Vy == 0)
                            {
                                game.StrikePaddle2();
                                game.TryServe(false);
                                game.GetBall().TriggerServeAnimation();
                            }
                            IsKPressed = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при ударе (игрок 2): {ex.Message}");
                            throw;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Обрабатывает событие отпускания клавиши.
        /// </summary>
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    IsWPressed = false;
                    break;
                case Keys.S:
                    IsSPressed = false;
                    break;
                case Keys.NumPad8:
                    IsNumPad8Pressed = false;
                    break;
                case Keys.NumPad2:
                    IsNumPad2Pressed = false;
                    break;
                case Keys.Q:
                    IsQPressed = false;
                    break;
                case Keys.NumPad5:
                    IsKPressed = false;
                    break;
            }
        }

        /// <summary>
        /// Обновляет позиции ракеток на основе ввода и времени.
        /// </summary>
        public void UpdatePaddlePositions(double dt)
        {
            Player1TargetY = IsWPressed && !IsSPressed ? Player1TargetY - 600 * dt :
                            IsSPressed && !IsWPressed ? Player1TargetY + 600 * dt : game.GetPlayer1Paddle().Y;
            Player2TargetY = IsNumPad8Pressed && !IsNumPad2Pressed ? Player2TargetY - 600 * dt :
                            IsNumPad2Pressed && !IsNumPad8Pressed ? Player2TargetY + 600 * dt : game.GetPlayer2Paddle().Y;
            var p1 = game.GetPlayer1Paddle();
            var p2 = game.GetPlayer2Paddle();
            Player1TargetY = Math.Max(p1.Height / 2, Math.Min(glControl.Height - p1.Height / 2, Player1TargetY));
            Player2TargetY = Math.Max(p2.Height / 2, Math.Min(glControl.Height - p2.Height / 2, Player2TargetY));
            game.MovePaddle1(Player1TargetY);
            game.MovePaddle2(Player2TargetY);
        }
    }
}