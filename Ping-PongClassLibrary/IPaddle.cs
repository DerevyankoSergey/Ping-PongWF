using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_PongClassLibrary
{
    public interface IPaddle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Vy { get; set; }
        public double? Elasticity { get; set; }

        public void UpdateVelocity(double newY, double deltaTime)
        {
            Vy = (newY - Y) / deltaTime;
            Y = newY;
        }

    }
}
