using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.UI
{
    public static class Colors
    {
        public static readonly Vector4 White = new Vector4(1f, 1f, 1f, 1f);
        public static readonly uint WhiteHex = ToHex(White);

        public static readonly Vector4 Transparent = new Vector4(0f, 0f, 0f, 0f);
        public static readonly Vector4 Red = new Vector4(1f, 0f, 0f, 1f);

        public static readonly Vector4 Blue = new Vector4(0.2313f, 0.5098f, 0.9647f, 1f);
        public static readonly uint BlueHex = ToHex(Blue);
        public static readonly Vector4 BlueHovered = ToHovered(Blue);

        public static readonly Vector4 Background = new Vector4(0.1372f, 0.1372f, 0.1372f, 1f);
        public static readonly uint BackgroundHex = ToHex(Background);
        public static readonly Vector4 ReallyDarkGray = new Vector4(0.0862f, 0.0862f, 0.0862f, 1f);

        public static readonly Vector4 DarkGray = new Vector4(0.1137f, 0.1137f, 0.1137f, 1f);
        public static readonly uint DarkGrayHex = ToHex(DarkGray);
        public static readonly Vector4 DarkGrayHovered = ToHovered(DarkGray);
        public static readonly uint DarkGrayHoveredHex = ToHex(DarkGrayHovered);

        public static readonly Vector4 MidGray = new Vector4(0.1882f, 0.1882f, 0.1882f, 1f);
        public static readonly uint MidGrayHex = ToHex(MidGray);
        public static readonly Vector4 MidGrayHovered = new Vector4(0.1882f + 0.05f, 0.1882f + 0.05f, 0.1882f + 0.05f, 1f);

        public static readonly Vector4 LightGray;
        public static readonly Vector4 Text = new Vector4(0.85f, 0.85f, 0.85f, 1f);
        public static readonly uint TextHex = ToHex(Text);

        private static Vector4 ToHovered(Vector4 color) => color + new Vector4(0.1f, 0.1f, 0.1f, 0f);
        private static uint ToHex(Vector4 color) => Color((byte)(color.X * 255f), (byte)(color.Y * 255f), (byte)(color.Z * 255f), (byte)(color.W * 255f));
        private static uint Color(byte r, byte g, byte b, byte a) { uint ret = a; ret <<= 8; ret += b; ret <<= 8; ret += g; ret <<= 8; ret += r; return ret; }
    }
}
