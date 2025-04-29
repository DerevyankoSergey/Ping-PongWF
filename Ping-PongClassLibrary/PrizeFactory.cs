using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_PongClassLibrary
{
    public abstract class PrizeFactory
    {
        public abstract IPrize CreatePrize(double x, double y, int textureId, double spawnTime);
    }
}
