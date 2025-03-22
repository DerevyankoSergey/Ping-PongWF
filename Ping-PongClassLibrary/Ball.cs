using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_PongClassLibrary
{
    public class Ball
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Vx { get; private set; }
        public double Vy { get; private set; }
        public double Radius { get; private set; }
        public double Mass { get; private set; }

        public Ball(double x, double y,double radius, double mass)
        {
            X = x;
            Y = y;
            Radius = radius;
            Mass = mass;
        }
    }
}
