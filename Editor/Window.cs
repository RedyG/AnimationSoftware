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

            var group1 = new Layer("group1", new PointF(100, 100), new Size(500, 500));
            group1.Offset = Timecode.FromSeconds(3f);
            group1.Position.Keyframes.Add(new Keyframe<PointF>(Timecode.FromSeconds(0), new PointF(100f, 100f), new BezierEasing(new PointF(1f, 0f), new PointF(0f, 1f))));
            group1.Position.Keyframes.Add(new Keyframe<PointF>(Timecode.FromSeconds(5), new PointF(500f, 500f), IEasing.Linear));
            group1.Effects.Add(new Engine.Effects.Rectangle());
            group1.Effects.Add(new Engine.Effects.Rectangle());
            group1.Effects.Add(new RenderChildren());
            //group1.Effects.Add(new NoChange());

            var layer1 = new Layer("layer1", new PointF(0f, 0f), new Size(200, 200));
            layer1.Effects.Add(new Engine.Effects.Image());
            group1.Layers.Add(layer1);

            var layer2 = new Layer("layer2", new PointF(250f, 250f), new Size(200, 200));
            group1.Layers.Add(layer2);

            var group2 = new Layer("group2", new PointF(0f, 0f), new System.Drawing.Size(100, 100));
            group2.Duration = Timecode.FromSeconds(1.5f);
            group2.Offset = Timecode.FromSeconds(0.5f);
            group2.Effects.Add(new Engine.Effects.Image());
            group2.Effects.Add(new Expression());

            App.Project.ActiveScene.Layers.Add(group1);
            App.Project.ActiveScene.Layers.Add(group2);
            

            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);

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
        static Texture fishTexture = Texture.FromImage("Z:\\1.jpg", TextureTarget.Texture2D);

        static Texture testTexture = Texture.Create(1920, 1080);
        static Framebuffer testFramebuffer = Framebuffer.FromTexture(testTexture);
        static Surface testSurface = new Surface(testTexture, testFramebuffer, new System.Drawing.Size(1920, 1080), new System.Drawing.Size(50, 100));

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            App.Project.Time.Seconds += (float)e.Time;

            _controller.Update(this, (float)e.Time);

            testSurface.Bind(FramebufferTarget.Framebuffer);
            GraphicsApi.Clear(Color4.Red);
            GraphicsApi.DrawTexture(MatrixBuilder.Empty, fishTexture);

            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            GraphicsApi.Clear(Color4.Black);
            GraphicsApi.DrawSurface(MatrixBuilder.Empty, testSurface);
            ImGui.DockSpaceOverViewport();
            ImGui.ShowDemoWindow();
            UI.EffectsWindow();
            UI.PreviewWindow();
            UI.TimelineWindow();
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