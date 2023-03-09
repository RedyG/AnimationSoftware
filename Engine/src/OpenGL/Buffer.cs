using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.OpenGL
{
    public class Buffer<T>
        : GLObject
        where T : struct
    {
        private BufferTarget _target;

        public Buffer(BufferTarget target) : base(GL.GenBuffer())
        {
            _target = target;
        }

        public static Buffer<T> FromData(BufferTarget target, int size, T[] data, BufferUsageHint usageHint)
        {

            var buffer = new Buffer<T>(target);
            buffer.Bind();
            buffer.Data(size, data, usageHint);

            return buffer;
        }

        public void Data(int size, T[] data, BufferUsageHint usageHint)
        {
            Bind();

            GL.BufferData(_target, size, data, usageHint);
        }

        public void Bind()
        {
            GL.BindBuffer(_target, Handle);
        }

        public static void Unbind(BufferTarget target)
        {
            GL.BindBuffer(target, 0);
        }

        public void Unbind()
        {
            GL.BindBuffer(_target, 0);
        }

        protected override void Dispose(bool manual)
        {
            if (!manual) return;
            GL.DeleteBuffer(Handle);
        }
    }
}
