using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities
{
    public static class MathUtilities
    {
        public static float Map(float value, float start1, float stop1, float start2, float stop2)
        {
            return (value - start1) / (stop1 - start1) * (stop2 - start2) + start2;
        }
    }
}
