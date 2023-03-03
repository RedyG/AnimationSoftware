using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    public readonly struct Transform
    {
        public PointF Position { get; }
        public SizeF Size { get; }
        public PointF Origin { get; }
        public float Rotation { get; }

        public Transform()
        {
            Position = new(0f, 0f);
            Size = new(1f, 1f);
            Origin = new(0f, 0f);
            Rotation = 0f;
        }

        public Transform(PointF position, SizeF size)
        {
            Position = position;
            Size = size;
            Origin = new(0f, 0f);
            Rotation = 0f;
        }

        public Transform(PointF position, SizeF size, PointF origin, float rotation)
        {
            Position = position;
            Size = size;
            Origin = origin;
            Rotation = rotation;
        }

        public Transform(PointF position, SizeF size, SizeF scale, PointF origin, float rotation)
        {
            Position = position;
            Size = Mult(size, scale);
            Origin = origin;
            Rotation = rotation;
        }

        public static SizeF Mult(SizeF a, SizeF b)
        {
            return new SizeF(a.Width * b.Width, a.Height * b.Height);
        }

        public Matrix4 ToMatrix()
        {
            return Matrix4.CreateTranslation(-Origin.X, -Origin.Y, 0f) * Matrix4.CreateScale(Size.Width, Size.Height, 1f) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation)) * Matrix4.CreateTranslation(Position.X, Position.Y, 0f);
        }
    }
}
