using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_PongClassLibrary
{
    public enum BonusType
    {
        Size,
        Length,
        Material
    }
    public interface IBonus
    {
        BonusType Type { get; }
        double X {  get; set; }
        double Y { get; set; }
        double Width { get;}
        double Height { get;}
    }
}
