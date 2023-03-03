using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    public static class MatrixBuilder
    {
        public readonly static Matrix4 TopLeftMatrix = Matrix4.CreateScale(2f, -2f, 1f) * Matrix4.CreateTranslation(-1f, 1f, 0f);

        public static Matrix4 CreateTransform(PointF position, SizeF size, SizeF scale, PointF origin, float rotation)
            => CreateTransform(position, Mult(size, scale), origin, rotation);

        public static Matrix4 CreateTransform(PointF position, SizeF size)
            => CreateTransform(position, size, new PointF(0f, 0f), 0f);

        public static Matrix4 CreateTransform(PointF position, SizeF size, PointF origin, float rotation)
            => Matrix4.CreateTranslation(-origin.X, -origin.Y, 0f) * Matrix4.CreateScale(size.Width, size.Height, 1f) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation)) * Matrix4.CreateTranslation(position.X, position.Y, 0f);

        public static SizeF Mult(SizeF a, SizeF b)
        {
            return new SizeF(a.Width * b.Width, a.Height * b.Height);
        }
    }
}
