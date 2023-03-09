﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Engine.Effects;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;
using Engine.OpenGL;
using System.Numerics;
using Engine.Graphics;
using Engine.Core;

namespace Editor
{
    public class Window : GameWindow
    {
        ImGuiController _controller;

        public Window(string title, int width, int height) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = new Vector2i(width, height), APIVersion = new Version(3, 3), Title = title, NumberOfSamples = 4 })
        {
            VSync = VSyncMode.On;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

            int a = GL.GetInteger(GetPName.MaxTextureImageUnits);
            Console.WriteLine(a);
            //GL.Enable(EnableCap.Multisample);
            App.Project = new Project("a");
            scene = new Scene(60f, new Size(1920, 1080), Timecode.FromSeconds(100f));
            App.Project.ActiveScene = scene;
            var layer = new Layer(new PointF(250f, 250f), new Size(500, 500));
            layer.Rotation.Keyframes!.Add(new Keyframe<float>(Timecode.FromSeconds(0f), 0f, new BezierEasing(0.8f, 0, 0, 0.8f)));
            layer.Rotation.Keyframes.Add(new Keyframe<float>(Timecode.FromSeconds(4f), 360f, new OutElastic()));
            layer.Rotation.Keyframes.Add(new Keyframe<float>(Timecode.FromSeconds(7f), 315f, new OutElastic()));


            //layer.Position.Keyframes!.Add(new Keyframe<PointF>(Timecode.FromSeconds(0f), new PointF(0f, 0f), new BezierEasing(0, 1, 0, 1)));
            layer.Position.Keyframes.Add(new Keyframe<PointF>(Timecode.FromSeconds(4f), new PointF(600f, 600f), new BezierEasing(0, 1, 0, 1)));

            layer.Origin.Value = new PointF(250f, 250f);

            layer.Scale.Keyframes!.Add(new Keyframe<System.Numerics.Vector2>(Timecode.FromSeconds(4f), new System.Numerics.Vector2(1f, 1f), new OutElastic()));
            layer.Scale.Keyframes!.Add(new Keyframe<System.Numerics.Vector2>(Timecode.FromSeconds(7f), new System.Numerics.Vector2(1f, 2f), new BezierEasing(0, 1, 0, 1)));
            var rect = new Engine.Effects.Rectangle();
            rect.Color.Value = Color.Red;
            rect.FitToLayer.Value = true;
            layer.Effects.Add(rect);
            scene.Layers.Add(layer);


            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            framebuffer = new Framebuffer();
            texture = Texture.Create(1920, 1080, IntPtr.Zero, PixelType.UnsignedByte, TextureTarget.Texture2D, TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.Repeat, TextureWrapMode.Repeat, PixelInternalFormat.Rgba, PixelFormat.Rgba);
            Texture.Unbind(TextureTarget.Texture2D);
            framebuffer.Bind(FramebufferTarget.Framebuffer);

            textureImage = Texture.FromImage("Z:\\1.jpg", TextureTarget.Texture2D);

            /*int depthrenderbuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthrenderbuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, 225, 225);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthrenderbuffer);*/
            //Texture.Unbind(TextureTarget.Texture2D);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, texture.Handle, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            rectangle = new();
            Texture.Unbind(TextureTarget.Texture2D);
            Texture.Unbind(TextureTarget.Texture2DMultisample);
            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            // Update the opengl viewport
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

            // Tell ImGui of the new size
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        Scene scene;
        Texture textureImage;
        Texture texture;
        Framebuffer framebuffer;
        Engine.Effects.Rectangle rectangle;

        float time = 0f;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            //Texture.Unbind(TextureTarget.Texture2D);
            //framebuffer.Bind(FramebufferTarget.Framebuffer);
            //GL.ClearColor(new Color4(255, 255, 48, 255));
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            //GraphicsApi.DrawRect(0, 0, 0, 0);
            //Framebuffer.Unbind(FramebufferTarget.Framebuffer);
            GraphicsApi.Time += 1f;
            _controller.Update(this, (float)e.Time);
            Texture.Unbind(TextureTarget.Texture2D);
            Texture.Unbind(TextureTarget.Texture2DMultisample);
            framebuffer.Bind(FramebufferTarget.Framebuffer);
            GL.Viewport(0, 0, 1920, 1080);
            Renderer.RenderActiveScene(new Surface(texture, framebuffer));
            //GraphicsApi.DrawTexture(Matrix4.CreateRotationZ(0f), textureImage);
            //GraphicsApi.DrawRect(MatrixBuilder.CreateTransform(new PointF(0.2f, 0.2f), new SizeF(0.7f, 0.7f)), App.Project.ActiveScene.AspectRatio, Color4.Pink);

            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            GL.ClearColor(new Color4(0, 32, 48, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GraphicsApi.DrawTexture(MatrixBuilder.CreateTransform(App.Project.ActiveScene.Layers[0]), textureImage);
            //GraphicsApi.DrawRect(0.5f, 0.5f, 0.5f, 0.5f, new Transform(), App.Project.ActiveScene.AspectRatio);
            //GraphicsApi.DrawTexture(textureImage);
            ImGui.ShowDemoWindow();

            if(ImGui.Begin("window"))
            {
                ImGui.Button("hey");
                ImGui.Image((IntPtr)texture.Handle, new System.Numerics.Vector2(App.Project.ActiveScene.Size.Width, App.Project.ActiveScene.Size.Height), new System.Numerics.Vector2(0,1), new System.Numerics.Vector2(1, 0));
            }
            ImGui.End();

            _controller.Render();

            ImGuiController.CheckGLError("End of frame");
            App.Project.Time.Seconds += (float)e.Time;
            SwapBuffers();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);


            _controller.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _controller.MouseScroll(e.Offset);
        }
    }
}