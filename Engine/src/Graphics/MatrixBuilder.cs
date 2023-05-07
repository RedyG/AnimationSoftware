using Engine.Core;
using Engine.Utilities;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    public static class MatrixBuilder
    {
        public readonly static Matrix4 TopLeft = Matrix4.CreateScale(2f, -2f, 1f) * Matrix4.CreateTranslation(-1f, 1f, 0f);
        public readonly static Matrix4 Empty = Matrix4.CreateScale(1f, 1f, 1f);

        public static Matrix4 ToTopLeft(Matrix4 matrix) => matrix * TopLeft;

        public static Matrix4 CreateTransform(Timecode time, SizeF containerSize, Layer layer)
            => CreateTransform(containerSize, layer.Transform.Position.GetValueAtTime(time), layer.Transform.Size.GetValueAtTime(time), layer.Transform.Origin.GetValueAtTime(time), layer.Transform.Scale.GetValueAtTime(time), layer.Transform.Rotation.GetValueAtTime(time));

        public static Matrix4 CreateTransform(SizeF containerSize, PointF position, SizeF size, PointF origin, System.Numerics.Vector2 scale, float rotation)
        {
            PointF normalizedPosition = Normalize(position, containerSize);
            SizeF normalizedSize = Normalize(size, containerSize);
            PointF normalizedOrigin = Normalize(origin, containerSize);

            return Matrix4.CreateScale(normalizedSize.Width, normalizedSize.Height, 1f) * Matrix4.CreateTranslation(-normalizedOrigin.X, -normalizedOrigin.Y, 0f) *
                   Matrix4.CreateScale(scale.X, scale.Y, 1f) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation)) *
                   Matrix4.CreateTranslation(normalizedPosition.X, normalizedPosition.Y, 0f) * Matrix4.CreateScale(1f / containerSize.GetAspectRatio(), 1f, 1f);
        }

        private static PointF Normalize(PointF point, SizeF containerSize) => new PointF(point.X / containerSize.Height, point.Y / containerSize.Height);
        private static PointF Normalize(Point point, SizeF containerSize) => new PointF((float)point.X / (float)containerSize.Height, (float)point.Y / (float)containerSize.Height);

        private static SizeF Normalize(SizeF size, SizeF containerSize) => new SizeF(size.Width / containerSize.Height, size.Height / containerSize.Height);
        private static SizeF Normalize(Size size, SizeF containerSize) => new SizeF((float)size.Width / (float)containerSize.Height, (float)size.Height / (float)containerSize.Height);

    }
}
