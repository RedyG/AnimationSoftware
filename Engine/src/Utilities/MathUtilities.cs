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

        public static float Lerp(float a, float b, float t) => a * (1f - t) + b * t;
        public static int Lerp(int a, int b, float t) => (int)MathF.Round(Lerp((float)a, (float)b, t));
    }
}
