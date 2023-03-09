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
            Bind();
            buffer.Bind();
            GL.VertexAttribPointer(index, size, type, normalized, stride, offset);
            GL.EnableVertexAttribArray(index);
        }


        public void AttribPointer<T>(Buffer<T> buffer, VertexAttribPointer attrib) where T : struct
        {
            AttribPointer(buffer, attrib.Index, attrib.Size, attrib.Type, attrib.Normalized, attrib.Stride, attrib.Offset);
        }

        // TODO: finish this proprely
       /* private static Dictionary<VertexAttribPointerType, int> _sizeOf = new Dictionary<VertexAttribPointerType, int>()
        {
            [VertexAttribPointerType.Byte] = sizeof(sbyte),
            [VertexAttribPointerType.Double] = sizeof(double),
            [VertexAttribPointerType.Float] = sizeof(float),
            [VertexAttribPointerType.HalfFloat] = sizeof(ushort), // same size as half
            [VertexAttribPointerType.Int] = sizeof(int),
            [VertexAttribPointerType.Short] = sizeof(short),
            [VertexAttribPointerType.UnsignedByte] = sizeof(byte),
            [VertexAttribPointerType.UnsignedInt] = sizeof(uint),
            [VertexAttribPointerType.UnsignedShort] = sizeof(ushort)
        };
        public static int GetSizeInBytes(SimpleVertexAttribPointer attrib) => attrib.Size * _sizeOf[attrib.Type];

        public void AttribPointers<T>(Buffer<T> buffer, SimpleVertexAttribPointer[] attribs) where T : struct
        {
            Bind();
            buffer.Bind();

            int stride = 0;
            foreach (var attrib in attribs)
                stride += GetSizeInBytes(attrib);

            int offset = 0;
            for (int i = 0; i < attribs.Length; i++)
            {
                var attrib = attribs[i];

                int sizeInBytes = GetSizeInBytes(attrib);
                GL.VertexAttribPointer(i, attrib.Size, attrib.Type, attrib.Normalized, stride, offset);
                offset += sizeInBytes;
            }
        }*/

        public void AttribPointers<T>(Buffer<T> buffer, VertexAttribPointer[] attribs) where T : struct
        {
            Bind();
            buffer.Bind();

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

        public static void Unbind()
        {
            GL.BindVertexArray(0);
        }


        protected override void Dispose(bool manual)
        {
            if (!manual) return;
            GL.DeleteVertexArray(Handle);
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

    public readonly struct SimpleVertexAttribPointer
    {
        public int Size { get; init; }
        public VertexAttribPointerType Type { get; init; }
        public bool Normalized { get; init; }

        public SimpleVertexAttribPointer(int size, VertexAttribPointerType type, bool normalized)
        {
            Size = size;
            Type = type;
            Normalized = normalized;
        }
    }
}
