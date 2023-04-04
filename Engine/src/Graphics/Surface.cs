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
using Engine.Core;

namespace Engine.Graphics
{

    public struct Surface
    {
        public Texture Texture { get; }
        public Framebuffer Framebuffer { get; }

        public Size Size { get; set; }
        public Size Viewport { get; set; }

        public OpenTK.Mathematics.Vector2 Ratio => new OpenTK.Mathematics.Vector2((float)Viewport.Width / (float)Size.Width, (float)Viewport.Height / (float)Size.Height);

        public Surface(Texture texture, Framebuffer framebuffer, Size size, Size viewport)
        {
            Texture = texture;
            Framebuffer = framebuffer;
            Size = size;
            Viewport = viewport;
        }

        public Surface(Texture texture, Framebuffer framebuffer, Size size)
        {
            Texture = texture;
            Framebuffer = framebuffer;
            Size = size;
            Viewport = size;
        }

        public void Bind(FramebufferTarget target)
        {
            GL.Viewport(Viewport);
            Framebuffer.Bind(target);
        }
    }
}
