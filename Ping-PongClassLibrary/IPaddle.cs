namespace Ping_PongClassLibrary
{
    public interface IPaddle
    {
        double X { get; }
        double Y { get; set; }
        double Width { get; }
        double Height { get; }
        double BaseHeight { get; }
        double Vy { get; }
        double SpeedModifier { get; }
        double BounceModifier { get; }
        double Mass { get; }
        void UpdateVelocity(double newY, double deltaTime);
        void Strike();
    }
}