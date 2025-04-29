using System;

namespace Ping_PongClassLibrary
{
    public class ChangeMaterialPrize : Prize
    {
        public ChangeMaterialPrize(double x, double y, double width, double height, int textureId, double spawnTime)
            : base(x, y, width, height, textureId, spawnTime) { }

        public override IPaddle Apply(IPaddle paddle)
        {
            return new MaterialChangeDecorator(paddle);
        }
    }
}