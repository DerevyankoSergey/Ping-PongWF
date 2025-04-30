namespace Ping_PongClassLibrary
{
    public abstract class PaddleDecorator : IPaddle
    {
        protected IPaddle paddle;

        protected PaddleDecorator(IPaddle paddle)
        {
            this.paddle = paddle;
        }

        public double X { get => paddle.X; set => paddle.X = value; }
        public double Y { get => paddle.Y; set => paddle.Y = value; }
        public double Width { get => paddle.Width; set => paddle.Width = value; }
        public double Height { get => paddle.Height; set => paddle.Height = value; }
        public double Vy { get => paddle.Vy; set => paddle.Vy = value; }

        public virtual void UpdateVelocity(double newY, double deltaTime)
        {
            paddle.UpdateVelocity(newY, deltaTime);
        }

        public virtual void Strike()
        {
            paddle.Strike(); // Делегируем вызов внутренней ракетке
        }
    }

    public class LengthIncreaseDecorator : PaddleDecorator
    {
        public LengthIncreaseDecorator(IPaddle paddle) : base(paddle)
        {
            Height *= 1.5; // Увеличиваем длину
        }
    }

    public class WidthIncreaseDecorator : PaddleDecorator
    {
        public WidthIncreaseDecorator(IPaddle paddle) : base(paddle)
        {
            Width *= 1.3; // Увеличиваем ширину
        }
    }

    public class MaterialChangeDecorator : PaddleDecorator
    {
        public MaterialChangeDecorator(IPaddle paddle) : base(paddle)
        {
            if (paddle is Paddle p)
            {
                p.SpeedModifier = 2.0; // Увеличиваем скорость
            }
        }
    }
}
