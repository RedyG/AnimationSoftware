using Engine.OpenGL;
using OpenTK.Graphics.OpenGL4;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{

    public class Surface
    {
        public Texture Texture { get; }
        public Framebuffer Framebuffer { get; }

        public Size Size { get; }
        public Size Viewport { get; set; }

        public OpenTK.Mathematics.Vector2 ViewportRatio => new OpenTK.Mathematics.Vector2((float)Viewport.Width / (float)Size.Width, (float)Viewport.Height / (float)Size.Height);

        public Surface(Texture texture, Framebuffer framebuffer, Size size, Size viewport)
        {
            Texture = texture;
            Framebuffer = framebuffer;
            Size = size;
            Viewport = viewport;
        }

        public void Bind(FramebufferTarget target)
        {
            GL.Viewport(Viewport);
            Framebuffer.Bind(target);
        }
    }
}
