namespace Ping_PongClassLibrary
{
    /// <summary>
    /// Класс, управляющий движением мяча в игре "Пинг-понг".
    /// Отвечает за хранение и обновление позиции, скорости и радиуса мяча.
    /// </summary>
    public class BallMovement
    {
        private double x;
        private double y;
        private double previousX;
        private double previousY;
        private double vx;
        private double vy;
        private readonly double radius;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="BallMovement"/> с заданными координатами и радиусом.
        /// </summary>
        public BallMovement(double x, double y, double radius)
        {
            this.x = x;
            this.y = y;
            this.previousX = x;
            this.previousY = y;
            this.vx = 0;
            this.vy = 0;
            this.radius = radius;
        }

        /// <summary>
        /// Обновляет позицию мяча на основе его текущей скорости и времени, прошедшего с последнего обновления.
        /// Сохраняет текущие координаты как предыдущие перед обновлением.
        /// </summary>
        public void Update(double deltaTime)
        {
            previousX = x;
            previousY = y;
            x += vx * deltaTime;
            y += vy * deltaTime;
        }

        /// <summary>
        /// Обрабатывает отскок мяча от верхней и нижней границы стола.
        /// </summary>
        public void BounceOffTable(int tableTop, int tableBottom)
        {
        }

        public double X => x;
        public double Y => y;
        public double PreviousX => previousX;
        public double PreviousY => previousY;
        public double Vx { get => vx; set => vx = value; }
        public double Vy { get => vy; set => vy = value; }
        public double Radius => radius;

        /// <summary>
        /// Устанавливает новую позицию мяча, сохраняя текущие координаты как предыдущие.
        /// </summary>
        public void SetPosition(double newX, double newY)
        {
            previousX = x;
            previousY = y;
            x = newX;
            y = newY;
        }

        /// <summary>
        /// Устанавливает новую скорость мяча по осям X и Y.
        /// </summary>
        public void SetVelocity(double newVx, double newVy)
        {
            vx = newVx;
            vy = newVy;
        }
    }
}