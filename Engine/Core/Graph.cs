using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public struct Graph
    {
        float P1 { get; set; }
        float P2 { get; set; }

        public Graph(float p1, float p2)
        {
            P1 = p1;
            P2 = p2;
        }
    }
}
