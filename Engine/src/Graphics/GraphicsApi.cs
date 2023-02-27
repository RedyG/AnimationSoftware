using OpenTK.Graphics.OpenGL4;
using Engine.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTest
{
    public static class GraphicsApi
    {
        public static void Clear(Color color)
        {
            GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        private static float[] vertices =
        {
            -0.5f, -0.5f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f,// top left
             0.5f,  0.5f, 0.0f,// top right
             0.5f, -0.5f, 0.0f,// bottom right
        };
        private static uint[] indices =
        {
            0, 1, 3, // left triange
            2, 1, 3, // right triangle
        };
        private static ShaderProgram shader;
        private static VertexArray vao;
        private static Buffer<float> vbo;
        private static Buffer<uint> ebo;
        public static void DrawRect()
        {
            shader.Bind();
            ebo.Bind(BufferTarget.ElementArrayBuffer);
            vao.Bind();
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }
        private static void InitDrawRect()
        {
            string vertexShaderSource = @"
                #version 330 core
                
                layout(location = 0) in vec3 aPos;

                void main()
                {
                    gl_Position = vec4(aPos, 1.0);
                }
            ";
            string fragmentShaderSource = @"
                #version 330 core

                out vec4 FragColor;

                vec4 _0()
                {
                    return vec4(1.0, 1.0, 0.5, 0.0);
                }

                void main()
                {
                    FragColor = _0();
                }
            ";

            shader = new ShaderProgram(vertexShaderSource, fragmentShaderSource);

            vao = new VertexArray();
            vao.Bind();

            vbo = Buffer<float>.FromData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            ebo = Buffer<uint>.FromData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            VertexAttribPointer[] attribs = {
                new(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0)
            };

            vao.AttribPointers(vbo, attribs);
        }

        static GraphicsApi()
        {
            InitDrawRect();
        }
    }
}
