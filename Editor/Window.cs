using System;
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
using System.Drawing.Drawing2D;
using OpenTK.Audio.OpenAL.Extensions.Creative.EFX;

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

            /* var group = new Layer(new PointF(0, 0), new Size(500, 500));
             group.Size.Keyframes!.Add(new Keyframe<SizeF>(Timecode.FromSeconds(0), new SizeF(500, 500), EasingPresets.Linear));
             group.Size.Keyframes!.Add(new Keyframe<SizeF>(Timecode.FromSeconds(5), new SizeF(1920, 1080), EasingPresets.Linear));

             var layer = new Layer(new PointF(0, 0), new Size(250, 250));
             layer.Position.Keyframes!.Add(new Keyframe<PointF>(Timecode.FromSeconds(0), new PointF(1920, 0), EasingPresets.Linear));
             layer.Position.Keyframes!.Add(new Keyframe<PointF>(Timecode.FromSeconds(5), new PointF(0, 0), EasingPresets.Linear));
             layer.Effects.Add(new Engine.Effects.Rectangle());

             group.Layers.Add(layer);
             App.Project.ActiveScene.Layers.Add(group);*/
            var p = new Parameter<float>(20f);
            var layer1 = new Layer("layer1", new PointF(0f, 0f), new System.Drawing.Size(1920, 1080));
            layer1.Effects.Add(new Engine.Effects.Rectangle());
            var layer2 = new Layer("second one", new PointF(50f, 50f), new System.Drawing.Size(1000, 1000));
            layer2.Effects.Add(new Engine.Effects.Image());
            var layer3 = new Layer("333", new PointF(50f, 50f), new System.Drawing.Size(500, 500));
            layer3.Effects.Add(new NoChange());
            layer3.Effects.Add(new Engine.Effects.Image());

            App.Project.ActiveScene.Layers.Add(layer1);
            App.Project.ActiveScene.Layers.Add(layer2);
            App.Project.ActiveScene.Layers.Add(layer3);

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

            // TODO: make my own style, I just copy pasted all that.
            var style = ImGui.GetStyle();
            style.Colors[(int)ImGuiCol.Text] = new System.Numerics.Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TextDisabled] = new System.Numerics.Vector4(0.50f, 0.50f, 0.50f, 1.00f);
            style.Colors[(int)ImGuiCol.WindowBg] = new System.Numerics.Vector4(0.13f, 0.14f, 0.15f, 1.00f);
            style.Colors[(int)ImGuiCol.ChildBg] = new System.Numerics.Vector4(0.13f, 0.14f, 0.15f, 1.00f);
            style.Colors[(int)ImGuiCol.PopupBg] = new System.Numerics.Vector4(0.13f, 0.14f, 0.15f, 1.00f);
            style.Colors[(int)ImGuiCol.Border] = new System.Numerics.Vector4(0.43f, 0.43f, 0.50f, 0.50f);
            style.Colors[(int)ImGuiCol.BorderShadow] = new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            style.Colors[(int)ImGuiCol.FrameBg] = new System.Numerics.Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new System.Numerics.Vector4(0.38f, 0.38f, 0.38f, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgActive] = new System.Numerics.Vector4(0.67f, 0.67f, 0.67f, 0.39f);
            style.Colors[(int)ImGuiCol.TitleBg] = new System.Numerics.Vector4(0.08f, 0.08f, 0.09f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new System.Numerics.Vector4(0.08f, 0.08f, 0.09f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.51f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = new System.Numerics.Vector4(0.14f, 0.14f, 0.14f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new System.Numerics.Vector4(0.02f, 0.02f, 0.02f, 0.53f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new System.Numerics.Vector4(0.31f, 0.31f, 0.31f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new System.Numerics.Vector4(0.41f, 0.41f, 0.41f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new System.Numerics.Vector4(0.51f, 0.51f, 0.51f, 1.00f);
            style.Colors[(int)ImGuiCol.CheckMark] = new System.Numerics.Vector4(0.26f, 0.59f, 0.98f, 0.95f);
            style.Colors[(int)ImGuiCol.SliderGrab] = new System.Numerics.Vector4(0.11f, 0.64f, 0.92f, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new System.Numerics.Vector4(0.08f, 0.50f, 0.72f, 1.00f);
            style.Colors[(int)ImGuiCol.Button] = new System.Numerics.Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new System.Numerics.Vector4(0.38f, 0.38f, 0.38f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new System.Numerics.Vector4(0.67f, 0.67f, 0.67f, 0.39f);
            style.Colors[(int)ImGuiCol.Header] = new System.Numerics.Vector4(0.22f, 0.22f, 0.22f, 1.00f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new System.Numerics.Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            style.Colors[(int)ImGuiCol.HeaderActive] = new System.Numerics.Vector4(0.67f, 0.67f, 0.67f, 0.39f);
            style.Colors[(int)ImGuiCol.Separator] = style.Colors[(int)ImGuiCol.Border];
            style.Colors[(int)ImGuiCol.SeparatorHovered] = new System.Numerics.Vector4(0.41f, 0.42f, 0.44f, 1.00f);
            style.Colors[(int)ImGuiCol.SeparatorActive] = new System.Numerics.Vector4(0.26f, 0.59f, 0.98f, 0.95f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = new System.Numerics.Vector4(0.29f, 0.30f, 0.31f, 0.67f);
            style.Colors[(int)ImGuiCol.ResizeGripActive] = new System.Numerics.Vector4(0.26f, 0.59f, 0.98f, 0.95f);
            style.Colors[(int)ImGuiCol.Tab] = new System.Numerics.Vector4(0.08f, 0.08f, 0.09f, 0.83f);
            style.Colors[(int)ImGuiCol.TabHovered] = new System.Numerics.Vector4(0.33f, 0.34f, 0.36f, 0.83f);
            style.Colors[(int)ImGuiCol.TabActive] = new System.Numerics.Vector4(0.23f, 0.23f, 0.24f, 1.00f);
            style.Colors[(int)ImGuiCol.TabUnfocused] = new System.Numerics.Vector4(0.08f, 0.08f, 0.09f, 1.00f);
            style.Colors[(int)ImGuiCol.TabUnfocusedActive] = new System.Numerics.Vector4(0.13f, 0.14f, 0.15f, 1.00f);
            style.Colors[(int)ImGuiCol.DockingPreview] = new System.Numerics.Vector4(0.26f, 0.59f, 0.98f, 0.70f);
            style.Colors[(int)ImGuiCol.DockingEmptyBg] = new System.Numerics.Vector4(0.20f, 0.20f, 0.20f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLines] = new System.Numerics.Vector4(0.61f, 0.61f, 0.61f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = new System.Numerics.Vector4(1.00f, 0.43f, 0.35f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogram] = new System.Numerics.Vector4(0.90f, 0.70f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new System.Numerics.Vector4(1.00f, 0.60f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TextSelectedBg] = new System.Numerics.Vector4(0.26f, 0.59f, 0.98f, 0.35f);
            style.Colors[(int)ImGuiCol.DragDropTarget] = new System.Numerics.Vector4(0.11f, 0.64f, 0.92f, 1.00f);
            style.Colors[(int)ImGuiCol.NavHighlight] = new System.Numerics.Vector4(0.26f, 0.59f, 0.98f, 1.00f);
            style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new System.Numerics.Vector4(1.00f, 1.00f, 1.00f, 0.70f);
            style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new System.Numerics.Vector4(0.80f, 0.80f, 0.80f, 0.20f);
            style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new System.Numerics.Vector4(0.80f, 0.80f, 0.80f, 0.35f);
            style.GrabRounding = style.FrameRounding = 2.3f;
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
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            //Texture.Unbind(TextureTarget.Texture2D);
            //framebuffer.Bind(FramebufferTarget.Framebuffer);
            //GL.ClearColor(new Color4(255, 255, 48, 255));
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            //GraphicsApi.DrawRect(0, 0, 0, 0);
            //Framebuffer.Unbind(FramebufferTarget.Framebuffer);
            
            _controller.Update(this, (float)e.Time);
            Texture.Unbind(TextureTarget.Texture2D);
            Texture.Unbind(TextureTarget.Texture2DMultisample);
            framebuffer.Bind(FramebufferTarget.Framebuffer);
            GL.Viewport(0, 0, 1920, 1080);
            Texture result = Renderer.RenderActiveScene();
            //GraphicsApi.DrawTexture(Matrix4.CreateRotationZ(0f), textureImage);
            //GraphicsApi.DrawRect(MatrixBuilder.CreateTransform(new PointF(0.2f, 0.2f), new SizeF(0.7f, 0.7f)), App.Project.ActiveScene.AspectRatio, Color4.Pink);

            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            GL.ClearColor(new Color4(0, 32, 48, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
   
            //GraphicsApi.DrawRect(0.5f, 0.5f, 0.5f, 0.5f, new Transform(), App.Project.ActiveScene.AspectRatio);
            //GraphicsApi.DrawTexture(textureImage);
            ImGui.DockSpaceOverViewport();
            ImGui.ShowDemoWindow();

            if (ImGui.Begin("Effects panel"))
            {
                // TODO: only for selected layers
                foreach (Layer layer in App.Project.ActiveScene.Layers)
                {
                    ImGui.Text("layer: " + layer.Name);
                    foreach (Effect effect in layer.Effects)
                    {
                        if (ImGui.CollapsingHeader(effect.Name))
                        {
                            ImGui.Columns(2, "params columns");
                            foreach (NamedParameter namedParameter in effect.Parameters)
                            {
                                var cursorPos = ImGui.GetCursorScreenPos();
                                //UI.MoveCursorBy(0f, 3f);
                                UI.KeyframeIcon(namedParameter.Parameter);
                                ImGui.SameLine();
                                //UI.MoveCursorBy(0f, -2f);
                                //UI.MoveCursorBy(new System.Numerics.Vector2(0, -150));

                                ImGui.Text(namedParameter.Name);
                                ImGui.NextColumn();

                                ImGui.PushID(namedParameter.Name);
                                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                                namedParameter.Parameter.DrawUI();
                                ImGui.PopID();
                                ImGui.NextColumn();
                            }
                            ImGui.Columns();
                        }
                    }
                }
            }
            ImGui.End();
            ImGui.ShowStackToolWindow();

            _controller.Render();

            ImGuiController.CheckGLError("End of frame");
            //App.Project.Time.Seconds += (float)e.Time;
            //if (App.Project.Time.Seconds > 16f)
                //App.Project.Time.Seconds = 0;
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