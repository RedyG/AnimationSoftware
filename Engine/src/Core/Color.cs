using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public struct Color
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public uint Hex => RGBToHex((byte)(R * 255f), (byte)(G * 255f), (byte)(B * 255f), (byte)(A * 255f));

        public static explicit operator Vector4(Color color) => new Vector4(color.R, color.G, color.B, color.A);

        public static explicit operator Color(Vector4 vector) => new Color(vector.X, vector.Y, vector.Z, vector.W);


        public static uint RGBToHex(byte r, byte g, byte b, byte a) { uint ret = a; ret <<= 8; ret += b; ret <<= 8; ret += g; ret <<= 8; ret += r; return ret; }

        public Color(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}
