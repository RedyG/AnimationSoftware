using Engine.OpenGL;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTest
{

    public class ShaderEffectProgram 
    {
        public ShaderProgram ShaderProgram { get; set; }

        public ShaderEffectProgram(ShaderProgram shaderProgram)
        {
            ShaderProgram = shaderProgram;
        }

        /*public void SetUniform(ShaderEffect shaderEffect, string uniformName, )
        {
            int location = GetUniformLocation($"{uniformName}_{shaderEffect.GetHashCode()}");
            GL.Uniform1(Handle, 0);
        }*/
    }
}