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
           //GL.Scissor(0, 0, Viewport.Width, Viewport.Height);
            GL.Viewport(Viewport);
            Framebuffer.Bind(target);
        }

        /// <summary>
        /// TODO: THIS.
        /// </summary>
        /// <param name="source">
        /// TODO: THIS.
        /// </param>
        /// <returns>The compiled ShaderProgram you can use in GraphicsApi.DrawSurface</returns>
        public static ShaderProgram CompileShader(string source)
        {
            string fragmentShaderSource = $@"
                #version 330 core

                out vec4 FragColor;

                in vec2 uv;

                uniform sampler2D input;
                uniform vec2 iResolution;

                {source}
                void main()
                {{
                    FragColor = surface();
                }}
            ";

            return new ShaderProgram(vertexShaderSource, fragmentShaderSource);
        }
        private static string vertexShaderSource = @"
                #version 330 core
                
                layout(location = 0) in vec3 aPos;
                layout(location = 1) in vec2 aTexCoord;                
                
                out vec2 uv;

                uniform mat4 transform;
                uniform vec2 ratio;

                void main()
                {
                    uv = aTexCoord * ratio;
                    gl_Position = vec4(aPos, 1.0) * transform;
                }
            ";
        public readonly static ShaderProgram DefaultShader = CompileShader("vec4 surface() { return texture(input, uv); }");
    }
}
