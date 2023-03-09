using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.OpenGL
{
    public class Framebuffer : GLObject
    {
        public Framebuffer() : base(GL.GenFramebuffer())
        {
        }

        public static Framebuffer FromTexture(Texture texture)
        {
            var framebuffer = new Framebuffer();
            framebuffer.Bind(FramebufferTarget.Framebuffer);

            /*int depthrenderbuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, 225, 225);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthrenderbuffer);*/
            //Texture.Unbind(TextureTarget.Texture2D);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, texture.Handle, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                throw new Exception("Couldn't create framebuffer.");

            return framebuffer;
        }


        public int GetFramebufferAttachmentParameter(FramebufferTarget target, FramebufferAttachment attachment, FramebufferParameterName paramName)
        {
            Bind(target);
            GL.GetFramebufferAttachmentParameter(target, attachment, paramName, out int textureId);
            return textureId;
        }

        public void Bind(FramebufferTarget target)
        {
            GL.BindFramebuffer(target, Handle);
        }

        public static void Unbind(FramebufferTarget target)
        {
            GL.BindFramebuffer(target, 0);
        }

        protected override void Dispose(bool manual)
        {
            if (!manual) return;
            GL.DeleteFramebuffer(Handle);
        }
    }
}
