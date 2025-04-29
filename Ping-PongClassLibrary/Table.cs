using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_PongClassLibrary
{
    public class Table
    {
        public double Width { get; }
        public double Height { get; }
        public double Left { get; }
        public double Right { get; }
        public double Top { get; }
        public double Bottom { get; }

        public Table(double screenWidth, double screenHeight)
        {
            Width = screenWidth * 0.8;  // 80% ширины окна
            Height = screenHeight * 0.7; // 70% высоты окна

            // Центрирование:
            Left = (screenWidth - Width) / 2;
            Top = (screenHeight - Height) / 2;
            Right = Left + Width;
            Bottom = Top + Height;

        }
    }
}
