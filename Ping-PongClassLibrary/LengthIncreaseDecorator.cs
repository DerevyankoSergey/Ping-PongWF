
namespace Ping_PongClassLibrary
{
    /// <summary>
    /// Декоратор, увеличивающий высоту ракетки.
    /// </summary>
    public class LengthIncreaseDecorator : PaddleDecorator
    {
        private readonly double _height;
        private readonly double _mass;

        /// <summary>
        /// Инициализирует новый экземпляр декоратора, увеличивающего высоту ракетки на 50% и изменяющего её массу.
        /// </summary>
        public LengthIncreaseDecorator(IPaddle paddle) : base(paddle)
        {
            _height = paddle.Height * 1.5;
            _mass = 0.17;
            if (paddle is Paddle basePaddle)
            {
                basePaddle.UpdateSize(paddle.Width, _height);
                basePaddle.UpdatePhysics(_mass, paddle.SpeedModifier, paddle.BounceModifier);
            }
        }

        public override double Height => _height;
        public override double Mass => _mass;
    }
}
