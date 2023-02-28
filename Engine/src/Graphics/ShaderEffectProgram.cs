using Engine.OpenGL;
using Engine.Utilities;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{

    public class ShaderEffectProgram 
    {
        public ShaderProgram ShaderProgram { get; set; }

        public ShaderEffectProgram(ShaderProgram shaderProgram)
        {
            ShaderProgram = shaderProgram;
        }

        public int GetUniformLocation(ShaderEffect shaderEffect, string uniformName)
        {
            return ShaderProgram.GetUniformLocation($"{uniformName}_{shaderEffect.Id.ToShaderFormat()}");
        }

        public void Uniform1(ShaderEffect shaderEffect, string uniformName, double v0) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), v0);
        public void Uniform1(ShaderEffect shaderEffect, string uniformName, int count, double[] value) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), count, value);
        public void Uniform1(ShaderEffect shaderEffect, string uniformName, int count, ref double value) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), count, ref value);
        public unsafe void Uniform1(ShaderEffect shaderEffect, string uniformName, int count, double* value) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), count, value);
        public void Uniform1(ShaderEffect shaderEffect, string uniformName, float v0) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), v0);
        public void Uniform1(ShaderEffect shaderEffect, string uniformName, int count, float[] value) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), count, value);
        public void Uniform1(ShaderEffect shaderEffect, string uniformName, int count, ref float value) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), count, ref value);
        public unsafe void Uniform1(ShaderEffect shaderEffect, string uniformName, int count, float* value) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), count, value);
        public void Uniform1(ShaderEffect shaderEffect, string uniformName, int v0) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), v0);
        public void Uniform1(ShaderEffect shaderEffect, string uniformName, int count, int[] value) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), count, value);
        public void Uniform1(ShaderEffect shaderEffect, string uniformName, int count, ref int value) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), count, ref value);
        public unsafe void Uniform1(ShaderEffect shaderEffect, string uniformName, int count, int* value) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), count, value);
        public void Uniform1(ShaderEffect shaderEffect, string uniformName, uint v0) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), v0);
        // TODO: test these
        public void Uniform1(ShaderEffect shaderEffect, string uniformName, int count, uint[] value) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), count, value);
        public void Uniform1(ShaderEffect shaderEffect, string uniformName, int count, ref uint value) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), count, ref value);
        public unsafe void Uniform1(ShaderEffect shaderEffect, string uniformName, int count, uint* value) => ShaderProgram.Uniform1(GetUniformLocation(shaderEffect, uniformName), count, value);
    }
}