using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.OpenGL;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Engine.Utilities;

namespace Engine.Graphics
{
    public class ShaderEffect
    {
        public Dictionary<string, ShaderEffect> Children { get; set; } = new();
        public string Content { get; set; }
        public Guid Id { get; } = Guid.NewGuid();

        public ShaderEffect(string content)
        {
            Content = content;
        }

        private const string VertexShader = @"
            #version 330 core
            
            layout(location = 0) in vec3 aPos;
     
            layout(location = 1) in vec2 aTex;
     
            out vec2 texCoord;
     
            void main()
            {
                texCoord = aTex;
                gl_Position = vec4(aPos, 1.0);
            }
        ";

        private const string TextureShaderEffect = @"
            uniform sampler2D s            

            vec4 main(in vec2 coord)
            {
                return texture(inputTexture, coord);
            }
        ";

        public static ShaderEffect TextureEffect = new ShaderEffect(TextureShaderEffect);

        public ShaderEffectProgram Compile()
        {
            // TODO: Resolution instead of TexCoord
            // TODO: remove unused code if ShaderEffect has no child ( so all previous shader effects can be ignored ) or if ShaderEffect isn't used

            string result = @"
                #version 330 core

                uniform vec4 Resolution;

                in vec2 texCoord;

                out vec4 FragColor;
            ";

            result += ConcactChildren();

            result += $@"
                void main()
                {{
                    FragColor = _{Id.ToShaderFormat()}(texCoord);
                }}
            ";

            Console.WriteLine(result);


            var shaderProgram = new ShaderProgram(VertexShader, result);
            var shaderEffectProgram = new ShaderEffectProgram(shaderProgram);

              
            //var resolutionLocation = shaderProgram.GetUniformLocation("resolution");
            //shaderProgram.Uniform2(resolutionLocation, )
            return shaderEffectProgram;

        }

        private string ConcactChildren()
        {
            string result = "";
            foreach (var child in Children.Values)
                result += child.ConcactChildren();

            // remove the shader "uniforms"
            string parsedCode = Regex.Replace(Content, "uniform\\s+shader[^;]+;", "");

            // replace "main" for the Hash
            parsedCode = Regex.Replace(parsedCode, "\\smain\\s+\\(", $" _{Id.ToShaderFormat()}(");

            foreach (var child in Children)
            {
                // replace {shader}.eval for the hash
                parsedCode = Regex.Replace(parsedCode, $"sample\\s*\\(\\s*{child.Key}\\s*,", $"_{child.Value.Id.ToShaderFormat()}(");
            }

            result += parsedCode;
            return result;
        }
    }
}
