using Engine.Core;
using Engine.Graphics;
using Engine.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing.Drawing2D;
using Engine.Attributes;

namespace Engine.Effects
{
    // TODO: time displacement with alpha map thingy

    public class NoChange : Effect
    {
        private static float[] textureVertices = {
             1f,  1f, 1.0f, 0.0f,   // bottom right
             1f,  0f, 1.0f, 1.0f,   // top right
             0f,  0f, 0.0f, 1.0f,   // top left
             0f,  1f, 0.0f, 0.0f    // bottom left
        };
        private static uint[] rectIndices = {
            0, 1, 3, // left triange
            2, 1, 3, // right triangle
        };
        private static ShaderProgram textureShader;
        private static VertexArray textureVao;
        private static Buffer<float> textureVbo;
        private static Buffer<uint> textureEbo;


        public override RenderResult Render(Surface mainSurface, Surface secondSurface, SizeF size)
        {
            secondSurface.Framebuffer.Bind(FramebufferTarget.Framebuffer);
            GraphicsApi.Clear(Color4.PapayaWhip);
            var matrix = MatrixBuilder.TopLeft;

            GL.ActiveTexture(TextureUnit.Texture0);
            mainSurface.Texture.Bind(TextureTarget.Texture2D);

            textureShader.Uniform1(textureShader.GetUniformLocation("tex"), 0);
            textureShader.UniformMatrix4(textureShader.GetUniformLocation("transform"), ref matrix);
            textureShader.Bind();
            textureEbo.Bind();
            textureVao.Bind();
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);


            return new RenderResult(true);
        }

        public NoChange()
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

                void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    float Pi = 6.28318530718; // Pi*2
    
    // GAUSSIAN BLUR SETTINGS {{{
    float Directions = 16.0; // BLUR DIRECTIONS (Default 16.0 - More is better but slower)
    float Quality = 3.0; // BLUR QUALITY (Default 4.0 - More is better but slower)
    float Size = 8.0; // BLUR SIZE (Radius)
    // GAUSSIAN BLUR SETTINGS }}}
   
    vec2 Radius = vec2(0.05);
    
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = fragCoord;
    // Pixel colour
    vec4 Color = texture(tex, uv);
    
    // Blur calculations
    for( float d=0.0; d<Pi; d+=Pi/Directions)
    {
		for(float i=1.0/Quality; i<=1.0; i+=1.0/Quality)
        {
			Color += texture( tex, uv+vec2(cos(d),sin(d))*Radius*i);		
        }
    }
    
    // Output to screen
    Color /= Quality * Directions - 15.0;
    fragColor =  Color;
}

                void main()
                {
                    mainImage(FragColor, texCoord);
                }
            "
            ;

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
    }
}
