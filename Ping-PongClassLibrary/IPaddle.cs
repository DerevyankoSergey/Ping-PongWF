namespace Ping_PongClassLibrary
{
    public interface IPaddle
    {
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        double Vy { get; set; }
        void UpdateVelocity(double newY, double deltaTime);
        void Strike();
    }
}