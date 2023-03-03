using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities
{
    public static class Extensions
    {
        public static string ToShaderFormat(this Guid guid)
        {
            return guid.ToString().Replace("-", "");
        }

        public static float GetAspectRatio(this SizeF size)
        {
            return size.Width / size.Height;
        }
    }
}
