namespace Ping_PongClassLibrary
{
    public interface IPrize
    {
        IPaddle Apply(IPaddle paddle);
        double X { get; }
        double Y { get; }
        double Width { get; }
        double Height { get; }
        int TextureId { get; }
        double SpawnTime { get; } 
        double Lifetime { get; } 
    }
}