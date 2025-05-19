namespace Ping_PongClassLibrary
{
    /// <summary>
    /// Абстрактный базовый класс для всех призов в игре, реализующий интерфейс IPrize.
    /// </summary>
    public abstract class Prize : IPrize
    {
        public double X { get; protected set; }
        public double Y { get; protected set; }
        public double Width { get; protected set; }
        public double Height { get; protected set; }
        public int TextureId { get; protected set; }
        public double SpawnTime { get; private set; }
        public double Lifetime { get; private set; } = 10.0;

        /// <summary>
        /// Инициализирует новый экземпляр приза с заданными параметрами.
        /// </summary>
        protected Prize(double x, double y, double width, double height, int textureId, double spawnTime)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            TextureId = textureId;
            SpawnTime = spawnTime;
        }

        /// <summary>
        /// Применяет эффект приза к ракетке, возвращая новую ракетку с измененными характеристиками.
        /// </summary>
        public abstract IPaddle Apply(IPaddle paddle);
    }
}