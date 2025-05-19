namespace Ping_PongClassLibrary
{
    /// <summary>
    /// Абстрактный базовый класс для декораторов ракетки, реализующий интерфейс IPaddle.
    /// </summary>
    public abstract class PaddleDecorator : IPaddle
    {
        protected IPaddle paddle;

        /// <summary>
        /// Инициализирует новый экземпляр декоратора ракетки.
        /// </summary>
        protected PaddleDecorator(IPaddle paddle)
        {
            this.paddle = paddle;
        }

        public virtual double X => paddle.X;
        public virtual double Y { get => paddle.Y; set => paddle.Y = value; }
        public virtual double Width => paddle.Width;
        public virtual double Height => paddle.Height;
        public virtual double BaseHeight => paddle.BaseHeight;
        public virtual double Vy => paddle.Vy;
        public virtual double SpeedModifier => paddle.SpeedModifier;
        public virtual double BounceModifier => paddle.BounceModifier;
        public virtual double Mass => paddle.Mass;

        /// <summary>
        /// Обновляет скорость и позицию ракетки.
        /// </summary>
        public virtual void UpdateVelocity(double newY, double deltaTime)
        {
            paddle.UpdateVelocity(newY, deltaTime);
        }

        /// <summary>
        /// Выполняет удар ракеткой.
        /// </summary>
        public virtual void Strike()
        {
            paddle.Strike();
        }
    }
}