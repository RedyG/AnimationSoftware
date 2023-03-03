using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.OpenGL
{
    public class ShaderProgram : GLObject
    {
        public ShaderProgram(string vertexShaderSource, string fragmentShaderSource) : base(GL.CreateProgram())
        {
            int vertexShader;
            int fragmentShader;

            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);

            GL.CompileShader(vertexShader);

            int succes;

            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out succes);
            if (succes == 0)
            {
                string infoLog = GL.GetShaderInfoLog(vertexShader);
                throw new Exception(infoLog);
            }

            GL.CompileShader(fragmentShader);

            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out succes);
            if (succes == 0)
            {
                string infoLog = GL.GetShaderInfoLog(fragmentShader);
                throw new Exception(infoLog);
            }

            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            GL.LinkProgram(Handle);
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out succes);
            if (succes == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                throw new Exception(infoLog);
            }

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);
        }

        public void Bind()
        {
            GL.UseProgram(Handle);
        }

        public static void Unbind()
        {
            GL.UseProgram(0);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        public int GetUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(Handle, uniformName);
        }

        public void Uniform1(int location, double v0) => GL.ProgramUniform1(Handle, location, v0);
        public void Uniform1(int location, int count, double[] value) => GL.ProgramUniform1(Handle, location, count, value);
        public void Uniform1(int location, int count, ref double value) => GL.ProgramUniform1(Handle, location, count, ref value);
        public unsafe void Uniform1(int location, int count, double* value) => GL.ProgramUniform1(Handle, location, count, value);
        public void Uniform1(int location, float v0) => GL.ProgramUniform1(Handle, location, v0);
        public void Uniform1(int location, int count, float[] value) => GL.ProgramUniform1(Handle, location, count, value);
        public void Uniform1(int location, int count, ref float value) => GL.ProgramUniform1(Handle, location, count, ref value);
        public unsafe void Uniform1(int location, int count, float* value) => GL.ProgramUniform1(Handle, location, count, value);
        public void Uniform1(int location, int v0) => GL.ProgramUniform1(Handle, location, v0);
        public void Uniform1(int location, int count, int[] value) => GL.ProgramUniform1(Handle, location, count, value);
        public void Uniform1(int location, int count, ref int value) => GL.ProgramUniform1(Handle, location, count, ref value);
        public unsafe void Uniform1(int location, int count, int* value) => GL.ProgramUniform1(Handle, location, count, value);
        public void Uniform1(int location, uint v0) => GL.ProgramUniform1(Handle, location, v0);
        // TODO: test these
        public void Uniform1(int location, int count, uint[] value) => GL.ProgramUniform1((uint)Handle, location, count, value);
        public void Uniform1(int location, int count, ref uint value) => GL.ProgramUniform1((uint)Handle, location, count, ref value);
        public unsafe void Uniform1(int location, int count, uint* value) => GL.ProgramUniform1((uint)Handle, location, count, value);


        public void Uniform2(int location, int v0, int v1)
        {
            GL.ProgramUniform2(Handle, location, v0, v1);
        }

        public void Uniform3(int location, int v0, int v1, int v2)
        {
            GL.ProgramUniform3(Handle, location, v0, v1, v2);
        }

        public void Uniform4(int location, int v0, int v1, int v2, int v3)
        {
            GL.ProgramUniform4(Handle, location, v0, v1, v2, v3);
        }

        public void Uniform4(int location, float v0, float v1, float v2, float v3)
        {
            GL.ProgramUniform4(Handle, location, v0, v1, v2, v3);
        }

        public void UniformMatrix4(int location, ref Matrix4 matrix)
        {
            GL.ProgramUniformMatrix4(Handle, location, true, ref matrix);
        }

        protected override void Dispose(bool manual)
        {
            if (!manual) return;
            GL.DeleteProgram(Handle);
        }
    }
}
