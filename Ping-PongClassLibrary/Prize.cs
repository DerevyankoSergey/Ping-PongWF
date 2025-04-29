namespace Ping_PongClassLibrary
{
    public abstract class Prize : IPrize
    {
        public double X { get; protected set; }
        public double Y { get; protected set; }
        public double Width { get; protected set; }
        public double Height { get; protected set; }
        public int TextureId { get; protected set; }
        public double SpawnTime { get; private set; }
        public double Lifetime { get; private set; } = 10.0; // Приз исчезает через 10 секунд

        protected Prize(double x, double y, double width, double height, int textureId, double spawnTime)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            TextureId = textureId;
            SpawnTime = spawnTime;
        }

        public abstract IPaddle Apply(IPaddle paddle);
    }
}