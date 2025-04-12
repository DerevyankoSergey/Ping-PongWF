using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_PongClassLibrary
{
    public class Paddle : IPaddle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width {  get; set; }
        public double Height { get; set; }
        public double Vy {  get; set; }
        public double? Elasticity { get; set; }

        private readonly double tableTop;
        private readonly double tableBottom;

        public Paddle(double x, double y, double width, double height, double tableTop, double tableBottom, double elasticity = 1.0)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Vy = 0;
            Elasticity = elasticity;
            this.tableTop = tableTop;
            this.tableBottom = tableBottom;
        }
        //Обновление скорости и положения ракетки
        public void UpdateVelocity(double newY, double deltaTime)
        {
            Vy = (newY - Y) / deltaTime;

            if(newY - Height / 2 < tableTop)
                newY = tableTop + Height / 2;
            else if(newY + Height / 2 > tableBottom)
                newY = tableBottom - Height / 2;

            Y = newY;
        }
    }
}
