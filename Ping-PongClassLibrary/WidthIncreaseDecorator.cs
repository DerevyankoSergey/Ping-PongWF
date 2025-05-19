using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_PongClassLibrary
{
    /// <summary>
    /// Декоратор, увеличивающий ширину ракетки.
    /// </summary>
    public class WidthIncreaseDecorator : PaddleDecorator
    {
        private readonly double _width;
        private readonly double _mass;

        /// <summary>
        /// Инициализирует новый экземпляр декоратора, увеличивающего ширину ракетки на 30% и изменяющего её массу.
        /// </summary>
        public WidthIncreaseDecorator(IPaddle paddle) : base(paddle)
        {
            _width = paddle.Width * 1.3;
            _mass = 0.19;
            if (paddle is Paddle basePaddle)
            {
                basePaddle.UpdateSize(_width, paddle.Height);
                basePaddle.UpdatePhysics(_mass, paddle.SpeedModifier, paddle.BounceModifier);
            }
        }

        public override double Width => _width;
        public override double Mass => _mass;
    }
}
