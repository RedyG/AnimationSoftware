using Engine.Attributes;
using Engine.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
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

        public static float GetAspectRatio(this Size size)
        {
            return (float)size.Width / (float)size.Height;
        }

        public static float GetAspectRatio(this Vector2 vector)
        {
            return vector.X / vector.Y;
        }
    }
}
