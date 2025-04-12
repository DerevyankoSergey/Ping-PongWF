using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_PongClassLibrary
{
    public class MaterialBonus : IBonus
    {
        public BonusType Type => BonusType.Material;
        public double X { get; set; }
        public double Y { get; set; }
        public double Width => 20;
        public double Height => 20;
    }
}
