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

namespace Engine.Graphics
{
    public static class GraphicsApi
    {

        public static void Clear(Color color)
        {
            GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        private static float[] textureVertices = {
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f,   // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f,   // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,   // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f    // top left
        };
        private static uint[] textureIndices =
        {
            0, 1, 3, // right triange
            2, 1, 3, // left triangle
        };
        private static ShaderProgram textureShader;
        private static VertexArray textureVao;
        private static Buffer<float> textureVbo;
        private static Buffer<uint> textureEbo;

        public static void DrawTexture(Texture texture)
        {
            textureShader.Bind();
            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Bind(TextureTarget.Texture2D);

            int textureLocation = textureShader.GetUniformLocation("tex");
            textureShader.Uniform1(textureLocation, 0);

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

                void main()
                {
                    texCoord = aTexCoord;
                    gl_Position = vec4(aPos, 1.0);
                }
            ";
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
                new(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0),
                new(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float))
            };

            textureVao.AttribPointers(textureVbo, attribs);
        }


        /*private static float[] rectVertices = {
            -1f, -1f, 0.0f, // bottom left
            -1f,  1f, 0.0f,// top left
             1f,  1f, 0.0f,// top right
             1f, -0.8f, 0.0f,// bottom right
        };*/
        private static float[] rectVertices = {
            0f, 0f, 0.0f, // bottom left
            0f,  1f, 0.0f,// top left
             1f,  1f, 0.0f,// top right
             1f, 0f, 0.0f,// bottom right
        };
        private static uint[] rectIndices =
        {
            0, 1, 3, // left triange
            2, 1, 3, // right triangle
        };
        private static ShaderProgram rectShader;
        private static VertexArray rectVao;
        private static Buffer<float> rectVbo;
        private static Buffer<uint> rectEbo;
        public static void DrawRect(Matrix4 transform, float aspectRatio, Color color)
        {
            Matrix4 matrix = transform * Matrix4.CreateScale(1f / aspectRatio, 1f, 1f) * MatrixBuilder.TopLeftMatrix;
            rectShader.Bind();
            rectShader.UniformMatrix4(rectShader.GetUniformLocation("transform"), ref matrix);
            rectShader.Uniform4(rectShader.GetUniformLocation("color"), 1f, 0f, 0f, 1f);
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

            VertexAttribPointer attrib = new(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            rectVao.AttribPointer(rectVbo, attrib);
        }

        static GraphicsApi()
        {
            InitRect();
            InitTexture();
        }
    }
}
