using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.OpenGL
{
    public class Buffer<T>
        : GLObject
        where T : struct
    {
        public Buffer() : base(GL.GenBuffer())
        {
        }

        public static Buffer<T> FromData(BufferTarget target, int size, T[] data, BufferUsageHint usageHint)
        {
            var buffer = new Buffer<T>();
            buffer.Bind(target);
            buffer.Data(target, size, data, usageHint);

            return buffer;
        }

        public void Data(BufferTarget target, int size, T[] data, BufferUsageHint usageHint)
        {
            AssertBound(target);

            GL.BufferData(target, size, data, usageHint);
        }

        public void Bind(BufferTarget target)
        {
            GL.BindBuffer(target, Handle);
        }

        public static void Unbind(BufferTarget target)
        {
            GL.BindBuffer(target, 0);
        }

        protected override void Dispose(bool manual)
        {
            if (!manual) return;
            GL.DeleteBuffer(Handle);
        }

        private static Dictionary<BufferTarget, GetPName> _assertDict = new()
        {
            // TODO: missing some
            [BufferTarget.ParameterBuffer] = GetPName.ParameterBufferBinding,
            [BufferTarget.ArrayBuffer] = GetPName.ArrayBufferBinding,
            [BufferTarget.ElementArrayBuffer] = GetPName.ElementArrayBufferBinding,
            [BufferTarget.PixelPackBuffer] = GetPName.PixelPackBufferBinding,
            [BufferTarget.PixelUnpackBuffer] = GetPName.PixelUnpackBufferBinding,
            [BufferTarget.TextureBuffer] = GetPName.TextureBindingBuffer,
            [BufferTarget.TransformFeedbackBuffer] = GetPName.TransformFeedbackBinding,
            [BufferTarget.DrawIndirectBuffer] = GetPName.DrawIndirectBufferBinding,
        };

        public void AssertBound(BufferTarget target)
        {
#if DEBUG
            int activeHandle;
            GL.GetInteger(_assertDict[target], out activeHandle);
            if (activeHandle != Handle) throw new Exception("Buffer is not bound.");
#endif
        }
    }
}
