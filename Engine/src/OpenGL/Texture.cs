using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.OpenGL
{
    public class Texture : GLObject
    {
        static Texture()
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
        }
        public Texture() : base(GL.GenTexture())
        {
        }

        public static Texture FromImage(string path, TextureUnit unit, TextureTarget target,
            TextureMinFilter minFilter = TextureMinFilter.Linear, TextureMagFilter magFilter = TextureMagFilter.Linear,
            TextureWrapMode wrapModeS = TextureWrapMode.MirroredRepeat, TextureWrapMode wrapModeT = TextureWrapMode.MirroredRepeat,
            PixelInternalFormat pixelInternalFormat = PixelInternalFormat.Rgba, PixelFormat pixelFormat = PixelFormat.Rgba)
        {
            var texture = new Texture();

            GL.ActiveTexture(unit);
            texture.Bind(target);

            GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)magFilter);

            GL.TexParameter(target, TextureParameterName.TextureWrapS, (int)wrapModeS);
            GL.TexParameter(target, TextureParameterName.TextureWrapT, (int)wrapModeT);

            ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(target, 0, pixelInternalFormat, image.Width, image.Height, 0, pixelFormat, PixelType.UnsignedByte, image.Data);

            return texture;
        }

        public static Texture Create(int width, int height, TextureUnit unit)
        {
            return Create(
                width, height, unit,
                IntPtr.Zero, PixelType.UnsignedByte, TextureTarget.Texture2D,
                TextureMinFilter.Linear, TextureMagFilter.Linear,
                TextureWrapMode.MirroredRepeat, TextureWrapMode.MirroredRepeat,
                PixelInternalFormat.Rgba, PixelFormat.Rgba);
        }

        public static Texture Create<T>(
            int width, int height, TextureUnit unit, T[] data,
            PixelType pixelType, TextureTarget target,
            TextureMinFilter minFilter, TextureMagFilter magFilter,
            TextureWrapMode wrapModeS, TextureWrapMode wrapModeT,
            PixelInternalFormat pixelInternalFormat, PixelFormat pixelFormat) where T : struct
        {
            var texture = CreateBase(unit, target, minFilter, magFilter, wrapModeS, wrapModeT);

            GL.TexImage2D(target, 0, pixelInternalFormat, width, height, 0, pixelFormat, pixelType, data);

            return texture;
        }

        public static Texture Create(
            int width, int height, TextureUnit unit, IntPtr data,
            PixelType pixelType, TextureTarget target,
            TextureMinFilter minFilter, TextureMagFilter magFilter,
            TextureWrapMode wrapModeS, TextureWrapMode wrapModeT,
            PixelInternalFormat pixelInternalFormat, PixelFormat pixelFormat)
        {
            var texture = CreateBase(unit, target, minFilter, magFilter, wrapModeS, wrapModeT);

            GL.TexImage2D(target, 0, pixelInternalFormat, width, height, 0, pixelFormat, pixelType, data);

            return texture;
        }

        private static Texture CreateBase(
            TextureUnit unit, TextureTarget target,
            TextureMinFilter minFilter, TextureMagFilter magFilter,
            TextureWrapMode wrapModeS, TextureWrapMode wrapModeT)
        {
            var texture = new Texture();
            texture.Bind(target, unit);

            GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)magFilter);

            GL.TexParameter(target, TextureParameterName.TextureWrapS, (int)wrapModeS);
            GL.TexParameter(target, TextureParameterName.TextureWrapT, (int)wrapModeT);

            return texture;
        }


        public void Bind(TextureTarget textureTarget)
        {
            GL.BindTexture(textureTarget, Handle);
        }

        public void Bind(TextureTarget textureTarget, TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(textureTarget, Handle);
        }

        public static void Unbind(TextureTarget textureTarget)
        {
            GL.BindTexture(textureTarget, 0);
        }

        protected override void Dispose(bool manual)
        {
            if (!manual) return;
            GL.DeleteTexture(Handle);
        }
    }
}
