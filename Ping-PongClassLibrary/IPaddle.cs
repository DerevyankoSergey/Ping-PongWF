using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ping_PongClassLibrary
{
    public interface IPaddle
    {
        int X { get; }
        int Y { get; }
        double Width {  get; }
        double Height { get; }
    }
}
