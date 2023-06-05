using OpenTK.Graphics.OpenGL4;
using Engine.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using System.Numerics;
using Engine.Core;

namespace Engine.Graphics
{
    public static class GraphicsApi
    {
        public static void Clear(Color4 color)
        {
            GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        private static float[] surfaceVertices = {
             1f,  1f, 1.0f, 0.0f,   // bottom right
             1f,  0f, 1.0f, 1.0f,   // top right
             0f,  0f, 0.0f, 1.0f,   // top left
             0f,  1f, 0.0f, 0.0f    // bottom left
        };
        private static VertexArray surfaceVao;
        private static Buffer<float> surfaceVbo;
        private static Buffer<uint> surfaceEbo;
        public static void DrawSurface(Matrix4 transform, Surface surface)
        {
            DrawSurface(transform, surface, Surface.DefaultShader);
        }
        public static void DrawSurface(Matrix4 transform, Surface surface, ShaderProgram shader)
        {
            Matrix4 matrix = MatrixBuilder.ToTopLeft(transform);
            GL.ActiveTexture(TextureUnit.Texture0);
            surface.Texture.Bind(TextureTarget.Texture2D);

            shader.Uniform1(shader.GetUniformLocation("input"), 0);
            shader.Uniform2(shader.GetUniformLocation("iResolution"), new OpenTK.Mathematics.Vector2(surface.Size.Width, surface.Size.Height));
            shader.Uniform2(shader.GetUniformLocation("ratio"), surface.Ratio);
            shader.UniformMatrix4(shader.GetUniformLocation("transform"), ref matrix);
            shader.Bind();
            surfaceEbo.Bind();
            surfaceVao.Bind();
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }
        public static void InitSurface()
        {
            surfaceVao = new VertexArray();
            surfaceVao.Bind();

            surfaceVbo = Buffer<float>.FromData(BufferTarget.ArrayBuffer, surfaceVertices.Length * sizeof(float), surfaceVertices, BufferUsageHint.StaticDraw);

            surfaceEbo = Buffer<uint>.FromData(BufferTarget.ElementArrayBuffer, rectIndices.Length * sizeof(uint), rectIndices, BufferUsageHint.StaticDraw);

            VertexAttribPointer[] attribs = {
                new(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0),
                new(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float))
            };

            surfaceVao.AttribPointers(surfaceVbo, attribs);
        }


        private static float[] textureVertices = {
             1f,  1f, 1.0f, 0.0f,   // bottom right
             1f,  0f, 1.0f, 1.0f,   // top right
             0f,  0f, 0.0f, 1.0f,   // top left
             0f,  1f, 0.0f, 0.0f    // bottom left
        };
        private static ShaderProgram textureShader;
        private static VertexArray textureVao;
        private static Buffer<float> textureVbo;
        private static Buffer<uint> textureEbo;
        public static void DrawTexture(Matrix4 transform, Texture texture)
        {
            Matrix4 matrix = MatrixBuilder.ToTopLeft(transform);
            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Bind(TextureTarget.Texture2D);

            textureShader.Uniform1(textureShader.GetUniformLocation("tex"), 0);
            textureShader.UniformMatrix4(textureShader.GetUniformLocation("transform"), ref matrix);
            textureShader.Bind();
            textureEbo.Bind();
            textureVao.Bind();
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }
        public static void InitTexture()
        {
            string vertexShaderSource = @"
                #version 330 core
                
                layout(location = 0) in vec3 aPos;
                layout(location = 1) in vec2 aTexCoord;                
                
                out vec2 texCoord;

                uniform mat4 transform;

                void main()
                {
                    texCoord = aTexCoord;
                    gl_Position = vec4(aPos, 1.0) * transform;
                }
            ";// - vec3(0.5, 0.0, 0.0)
            string fragmentShaderSource = @"
                #version 330 core

                out vec4 FragColor;

                in vec2 texCoord;

                uniform sampler2D tex;

                void main()
                {
                    FragColor = texture(tex, texCoord);
                }
            ";

            textureShader = new ShaderProgram(vertexShaderSource, fragmentShaderSource);

            textureVao = new VertexArray();
            textureVao.Bind();

            textureVbo = Buffer<float>.FromData(BufferTarget.ArrayBuffer, textureVertices.Length * sizeof(float), textureVertices, BufferUsageHint.StaticDraw);

            textureEbo = Buffer<uint>.FromData(BufferTarget.ElementArrayBuffer, rectIndices.Length * sizeof(uint), rectIndices, BufferUsageHint.StaticDraw);

            VertexAttribPointer[] attribs = {
                new(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0),
                new(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float))
            };

            textureVao.AttribPointers(textureVbo, attribs);
        }



        private static float[] rectVertices = {
            0f, 0f, // bottom left
            0f,  1f,// top left
             1f,  1f,// top right
             1f, 0f,// bottom right
        };
        private static uint[] rectIndices = {
            0, 1, 3, // left triange
            2, 1, 3, // right triangle
        };
        private static ShaderProgram rectShader;
        private static VertexArray rectVao;
        private static Buffer<float> rectVbo;
        private static Buffer<uint> rectEbo;
        public static void DrawRect(Matrix4 transform, Color4 color)
        {
            Matrix4 matrix = MatrixBuilder.ToTopLeft(transform);
            rectShader.UniformMatrix4(rectShader.GetUniformLocation("transform"), ref matrix);
            rectShader.Uniform4(rectShader.GetUniformLocation("color"), color);

            rectShader.Bind();
            rectEbo.Bind();
            rectVao.Bind();
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }
        private static void InitRect()
        {
            string vertexShaderSource = @"
                #version 330 core
                
                layout(location = 0) in vec3 aPos;

                uniform mat4 transform;

                void main()
                {
                    gl_Position = vec4(aPos, 1.0f) * transform;
                }
            ";
            string fragmentShaderSource = @"
                #version 330 core

                uniform vec4 color;

                out vec4 FragColor;

                void main()
                {
                    FragColor = color;
                }
            ";

            rectShader = new ShaderProgram(vertexShaderSource, fragmentShaderSource);

            rectVao = new VertexArray();
            rectVao.Bind();

            rectVbo = Buffer<float>.FromData(BufferTarget.ArrayBuffer, rectVertices.Length * sizeof(float), rectVertices, BufferUsageHint.StaticDraw);

            rectEbo = Buffer<uint>.FromData(BufferTarget.ElementArrayBuffer, rectIndices.Length * sizeof(uint), rectIndices, BufferUsageHint.StaticDraw);

            VertexAttribPointer attrib = new(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            rectVao.AttribPointer(rectVbo, attrib);
        }

        static GraphicsApi()
        {
            InitRect();
            InitTexture();
            InitSurface();
        }
    }
}
