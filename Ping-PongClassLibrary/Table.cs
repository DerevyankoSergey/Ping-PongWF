using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_PongClassLibrary
{
    public class Table
    {
        public double Width {  get;}
        public double Height { get;}
        public double Left {  get;}
        public double Right { get;}
        public double Top { get;}
        public double Bottom { get;}

        public Table(double screenWidth, double screenHeight, double margin = 50)
        {
            Width = screenWidth - 2 * margin;
            Height = screenHeight - 2 * margin;
            Left = margin;
            Right = screenWidth - margin;
            Top = margin;
            Bottom = screenHeight - margin;
        }
    }
}
