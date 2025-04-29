using System;

namespace Ping_PongClassLibrary
{
    public class IncreaseLengthPrize : Prize
    {
        public IncreaseLengthPrize(double x, double y, double width, double height, int textureId, double spawnTime)
            : base(x, y, width, height, textureId, spawnTime) { }

        public override IPaddle Apply(IPaddle paddle)
        {
            return new LengthIncreaseDecorator(paddle);
        }
    }
}