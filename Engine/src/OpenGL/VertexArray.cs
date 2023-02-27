using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.OpenGL
{
    public class VertexArray : GLObject
    {
        public VertexArray() : base(GL.GenVertexArray())
        {
        }

        public void AttribPointer<T>(Buffer<T> buffer, int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset) where T : struct
        {
            AssertBound();

            buffer.Bind(BufferTarget.ArrayBuffer);
            GL.VertexAttribPointer(index, size, type, normalized, stride, offset);
            GL.EnableVertexAttribArray(index);
        }

        public void AttribPointer<T>(Buffer<T> buffer, VertexAttribPointer attrib) where T : struct
        {
            AttribPointer(buffer, attrib.Index, attrib.Size, attrib.Type, attrib.Normalized, attrib.Stride, attrib.Offset);
        }

        public void AttribPointers<T>(Buffer<T> buffer, VertexAttribPointer[] attribs) where T : struct
        {
            AssertBound();

            buffer.Bind(BufferTarget.ArrayBuffer);
            foreach (var attrib in attribs)
            {
                GL.VertexAttribPointer(attrib.Index, attrib.Size, attrib.Type, attrib.Normalized, attrib.Stride, attrib.Offset);
                GL.EnableVertexAttribArray(attrib.Index);
            }
        }

        public void Bind()
        {
            GL.BindVertexArray(Handle);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }


        protected override void Dispose(bool manual)
        {
            if (!manual) return;
            GL.DeleteVertexArray(Handle);
        }

        public void AssertBound()
        {
#if DEBUG
            int activeHandle;
            GL.GetInteger(GetPName.VertexArrayBinding, out activeHandle);
            if (activeHandle != Handle) throw new Exception("Vertex array object is not bound.");
#endif
        }
    }

    public readonly struct VertexAttribPointer
    {
        public int Index { get; init; }
        public int Size { get; init; }
        public VertexAttribPointerType Type { get; init; }
        public bool Normalized { get; init; }
        public int Stride { get; init; }
        public int Offset { get; init; }

        public VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
        {
            Index = index;
            Size = size;
            Type = type;
            Normalized = normalized;
            Stride = stride;
            Offset = offset;
        }
    }
}
