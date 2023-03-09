using Engine.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{

    public readonly struct Surface : IEquatable<Surface>
    {
        public Texture Texture { get; }
        public Framebuffer Framebuffer { get; }

        public Surface(Texture texture, Framebuffer framebuffer)
        {
            Texture = texture;
            Framebuffer = framebuffer;
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(Surface other)
        {
            return Texture == other.Texture && Framebuffer == other.Framebuffer;
        }

        public static bool operator ==(Surface a, Surface b) => a.Equals(b);
        public static bool operator !=(Surface a, Surface b) => !(a == b);

        public override int GetHashCode() => (Texture, Framebuffer).GetHashCode();
    }
}
