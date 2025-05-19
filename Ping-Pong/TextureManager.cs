using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Ping_Pong
{
    /// <summary>
    /// Управляет загрузкой, хранением и удалением текстур для рендеринга в игре.
    /// </summary>
    public class TextureManager
    {
        public int[] Textures { get; private set; }
        public bool TexturesLoaded { get; private set; }

        /// <summary>
        /// Получает словарь, сопоставляющий символы с идентификаторами красных текстур букв.
        /// </summary>
        public Dictionary<char, TextureId> RedLetterTextures { get; } = new Dictionary<char, TextureId>
        {
            { 'R', TextureId.RedR }, { 'E', TextureId.RedE }, { 'D', TextureId.RedD },
            { 'W', TextureId.RedW }, { 'I', TextureId.RedI }, { 'N', TextureId.RedN }
        };

        /// <summary>
        /// Получает словарь, сопоставляющий символы с идентификаторами синих текстур букв.
        /// </summary>
        public Dictionary<char, TextureId> BlueLetterTextures { get; } = new Dictionary<char, TextureId>
        {
            { 'B', TextureId.BlueB }, { 'E', TextureId.BlueE }, { 'I', TextureId.BlueI },
            { 'L', TextureId.BlueL }, { 'N', TextureId.BlueN }, { 'U', TextureId.BlueU },
            { 'W', TextureId.BlueW }
        };

        private readonly GLControl glControl;
        private readonly string contentPath;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="TextureManager"/>.
        /// </summary>
        public TextureManager(GLControl glControl, string appPath)
        {
            this.glControl = glControl;
            contentPath = Path.Combine(appPath, "Content");
            Textures = new int[Enum.GetValues(typeof(TextureId)).Length];
            TexturesLoaded = false;
        }

        /// <summary>
        /// Загружает все текстуры из файлов в папке Content.
        /// </summary>
        public void LoadTextures()
        {
            try
            {
                if (!Directory.Exists(contentPath)) Directory.CreateDirectory(contentPath);
                GL.GenTextures(Textures.Length, Textures);

                var textureFiles = new Dictionary<TextureId, string>
                {
                    { TextureId.Table, "table.png" }, { TextureId.Paddle1, "paddle1.png" },
                    { TextureId.Paddle2, "paddle2.png" }, { TextureId.Ball, "ball.png" },
                    { TextureId.PrizeLength, "prize_length.png" }, { TextureId.PrizeWidth, "prize_width.png" },
                    { TextureId.PrizeMaterial, "prize_material.png" }, { TextureId.Scoreboard, "tablo.png" },
                    { TextureId.MenuBackground, "menu_background.png" }, { TextureId.IronPaddle1, "IronPaddle1.png" },
                    { TextureId.IronPaddle2, "IronPaddle2.png" }, { TextureId.GameOverBackground, "gameover_background.png" },
                    { TextureId.Exit, "exit.png" }, { TextureId.Play, "play.png" }, { TextureId.Pause, "pause.png" },
                    { TextureId.Continue, "continue.png" }, { TextureId.ExitMenu, "exit_menu.png" },
                    { TextureId.PauseMenu, "pause_menu.png" }, { TextureId.Restart, "restart.png" }
                };

                foreach (var pair in textureFiles)
                {
                    LoadTextureFromFile(Textures[(int)pair.Key], pair.Value, Color.White);
                }

                string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
                for (int i = 0; i < digits.Length; i++)
                {
                    LoadTextureFromFile(Textures[(int)TextureId.RedDigit0 + i], $"{digits[i]}.png", Color.White);
                    LoadTextureFromFile(Textures[(int)TextureId.BlueDigit0 + i], $"{digits[i]}Blue.png", Color.White);
                }

                var redLetters = new Dictionary<TextureId, string>
                {
                    { TextureId.RedR, "r.png" }, { TextureId.RedE, "e.png" }, { TextureId.RedD, "d.png" },
                    { TextureId.RedW, "w.png" }, { TextureId.RedI, "i.png" }, { TextureId.RedN, "n.png" }
                };
                foreach (var pair in redLetters)
                {
                    LoadTextureFromFile(Textures[(int)pair.Key], pair.Value, Color.White);
                }

                var blueLetters = new Dictionary<TextureId, string>
                {
                    { TextureId.BlueB, "BBlue.png" }, { TextureId.BlueE, "EBlue.png" }, { TextureId.BlueI, "IBlue.png" },
                    { TextureId.BlueL, "LBlue.png" }, { TextureId.BlueN, "NBlue.png" }, { TextureId.BlueU, "UBlue.png" },
                    { TextureId.BlueW, "WBlue.png" }
                };
                foreach (var pair in blueLetters)
                {
                    LoadTextureFromFile(Textures[(int)pair.Key], pair.Value, Color.White);
                }

                TexturesLoaded = true;
            }
            catch (Exception ex)
            {
                TexturesLoaded = false;
                Console.WriteLine($"Ошибка при загрузке текстур: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Загружает текстуру из файла или создаёт запасную текстуру указанного цвета.
        /// </summary>
        private void LoadTextureFromFile(int textureId, string filename, Color fallbackColor)
        {
            try
            {
                string filePath = Path.Combine(contentPath, filename);
                Bitmap bitmap = File.Exists(filePath) ? new Bitmap(filePath) : CreateColorTexture(fallbackColor, 64, 64);
                if (!File.Exists(filePath))
                {
                    bitmap.Save(filePath);
                }

                GL.BindTexture(TextureTarget.Texture2D, textureId);
                var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                bitmap.UnlockBits(data);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                bitmap.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки текстуры {filename}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Создаёт запасную текстуру заданного цвета с простым узором.
        /// </summary>
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
                        for (int x = 0; x < width; x += 10) g.DrawLine(pen, x, 0, x, height);
                        for (int y = 0; y < height; y += 10) g.DrawLine(pen, 0, y, width, y);
                    }
                    else if (color == Color.White || color == Color.Gray) g.DrawRectangle(pen, 2, 2, width - 4, height - 4);
                    else if (color == Color.Red) g.DrawEllipse(pen, 2, 2, width - 4, height - 4);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Освобождает ресурсы, удаляя загруженные текстуры из OpenGL.
        /// </summary>
        public void Cleanup()
        {
            if (TexturesLoaded)
            {
                GL.DeleteTextures(Textures.Length, Textures);
            }
        }
    }
}