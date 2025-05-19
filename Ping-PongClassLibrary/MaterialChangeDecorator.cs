using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_PongClassLibrary
{
    /// <summary>
    /// Декоратор, изменяющий материал ракетки, влияющий на её массу и параметры отскока.
    /// </summary>
    public class MaterialChangeDecorator : PaddleDecorator
    {
        private readonly double _mass;
        private readonly double _speedModifier;
        private readonly double _bounceModifier;

        /// <summary>
        /// Инициализирует новый экземпляр декоратора, изменяющего материал ракетки, увеличивая её массу и модификатор отскока.
        /// </summary>
        public MaterialChangeDecorator(IPaddle paddle) : base(paddle)
        {
            _mass = 0.5;
            _speedModifier = 1.0;
            _bounceModifier = 1.5;
            if (paddle is Paddle basePaddle)
            {
                basePaddle.UpdatePhysics(_mass, _speedModifier, _bounceModifier);
            }
        }

        public override double Mass => _mass;
        public override double SpeedModifier => _speedModifier;
        public override double BounceModifier => _bounceModifier;
    }
}
