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
        public readonly static Matrix4 TopLeftMatrix = Matrix4.CreateScale(2f, -2f, 1f) * Matrix4.CreateTranslation(-1f, 1f, 0f);
        public static Matrix4 ToTopLeft(Matrix4 matrix) => matrix * TopLeftMatrix;

        public static Matrix4 CreateTransform(Layer layer)
        {
            PointF origin = Normalize(layer.Origin.Value);
            SizeF size = Normalize(layer.Size.Value);
            System.Numerics.Vector2 scale = layer.Scale.Value;
            PointF position = Normalize(layer.Position.Value);
            float rotation = layer.Rotation.Value;

            return Matrix4.CreateScale(size.Width, size.Height, 1f) * Matrix4.CreateTranslation(-origin.X, -origin.Y, 0f) * Matrix4.CreateScale(scale.X, scale.Y, 1f) *
                   Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation)) * Matrix4.CreateTranslation(position.X, position.Y, 0f) *
                   Matrix4.CreateScale(1f / App.Project.ActiveScene.AspectRatio, 1f, 1f);
        }

        public static Matrix4 CreateTransform(PointF position, SizeF size, Vector2 scale, PointF origin, float rotation, SizeF containerSize)
            => Matrix4.CreateScale(size.Width, size.Height, 1f) * Matrix4.CreateTranslation(-origin.X, -origin.Y, 0f) *
               Matrix4.CreateScale(scale.X, scale.Y, 1f) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation)) *
               Matrix4.CreateTranslation(position.X, position.Y, 0f) * Matrix4.CreateScale(1f / containerSize.GetAspectRatio(), 1f, 1f);

        public static SizeF Mult(SizeF a, System.Numerics.Vector2 b)
        {
            return new SizeF(a.Width * b.X, a.Height * b.Y);
        }
        private static PointF Normalize(PointF point) => new PointF(point.X / App.Project.ActiveScene.Size.Height, point.Y / App.Project.ActiveScene.Size.Height);
        private static PointF Normalize(Point point) => new PointF((float)point.X / (float)App.Project.ActiveScene.Size.Height, (float)point.Y / (float)App.Project.ActiveScene.Size.Height);

        private static SizeF Normalize(SizeF size) => new SizeF(size.Width / App.Project.ActiveScene.Size.Height, size.Height / App.Project.ActiveScene.Size.Height);
        private static SizeF Normalize(Size size) => new SizeF((float)size.Width / (float)App.Project.ActiveScene.Size.Height, (float)size.Height / (float)App.Project.ActiveScene.Size.Height);

    }
}
