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
using Engine.UI;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Runtime.CompilerServices;
using System.Reflection;

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

            var assembly = typeof(App).Assembly;

            Effect.LoadEffectsFromAssembly(assembly);
            IEasing.LoadEasingsFromAssembly(assembly);

            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            GL.BlendEquation(BlendEquationMode.FuncAdd);


            int a = GL.GetInteger(GetPName.MaxTextureImageUnits);
            //GL.Enable(EnableCap.Multisample);
            App.Project = new Project("a");
            scene = new Scene(60f, new Size(1920, 1080), Timecode.FromSeconds(100f));
            App.Project.ActiveScene = scene;


            var layer1 = new Layer("Layer1", PointF.Empty, new System.Drawing.Size(250, 250));
            layer1.AddEffect(new Engine.Effects.Rectangle());
            App.Project.ActiveScene.Layers.Add(layer1);

            var layer2 = new Layer("Layer2", new PointF(250, 250), new System.Drawing.Size(250, 250));
            layer2.AddEffect(new Engine.Effects.Rectangle());
            App.Project.ActiveScene.Layers.Add(layer2);





            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);

            // TODO: make my own style, I just copy pasted all that.
            var style = ImGui.GetStyle();
            style.Colors[(int)ImGuiCol.Text] = Colors.Text;
            style.Colors[(int)ImGuiCol.TextDisabled] = Colors.Text;
            style.Colors[(int)ImGuiCol.WindowBg] = Colors.Background;
            style.Colors[(int)ImGuiCol.ChildBg] = Colors.Transparent;
            style.Colors[(int)ImGuiCol.PopupBg] = Colors.MidGray;
            style.Colors[(int)ImGuiCol.Border] = Colors.ReallyDarkGray;
            style.Colors[(int)ImGuiCol.BorderShadow] = Colors.Transparent;
            style.Colors[(int)ImGuiCol.FrameBg] = Colors.DarkGray;
            style.Colors[(int)ImGuiCol.FrameBgHovered] = Colors.DarkGrayHovered;
            style.Colors[(int)ImGuiCol.FrameBgActive] = Colors.DarkGrayHovered;
            style.Colors[(int)ImGuiCol.TitleBg] = Colors.ReallyDarkGray;
            style.Colors[(int)ImGuiCol.TitleBgActive] = Colors.ReallyDarkGray;
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = Colors.Red;
            style.Colors[(int)ImGuiCol.MenuBarBg] = Colors.DarkGray;
            style.Colors[(int)ImGuiCol.ScrollbarBg] = Colors.ReallyDarkGray;
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = Colors.MidGray;
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = Colors.MidGrayHovered;
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = Colors.MidGrayHovered;
            style.Colors[(int)ImGuiCol.CheckMark] = Colors.Blue;
            style.Colors[(int)ImGuiCol.SliderGrab] = Colors.Blue;
            style.Colors[(int)ImGuiCol.SliderGrabActive] = Colors.BlueHovered;
            style.Colors[(int)ImGuiCol.Button] = Colors.DarkGray;
            style.Colors[(int)ImGuiCol.ButtonHovered] = Colors.DarkGrayHovered;
            style.Colors[(int)ImGuiCol.ButtonActive] = Colors.DarkGrayHovered;
            style.Colors[(int)ImGuiCol.Header] = Colors.DarkGray;
            style.Colors[(int)ImGuiCol.HeaderHovered] = Colors.DarkGrayHovered;
            style.Colors[(int)ImGuiCol.HeaderActive] = Colors.DarkGrayHovered;
            style.Colors[(int)ImGuiCol.Separator] = Colors.ReallyDarkGray;
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
            style.GrabRounding = style.FrameRounding = 0f;
            style.WindowPadding = new System.Numerics.Vector2(4f);
            style.ChildBorderSize = 1f;
            style.CellPadding = new System.Numerics.Vector2(0f);
            style.ItemSpacing = new System.Numerics.Vector2(4f);
            //style.Padding = new System.Numerics.Vector2(10f, 10f);
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
            var time = Stopwatch.StartNew();
            _controller.Update(this, (float)e.Time);


            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            GraphicsApi.Clear(Color4.Black);

            UI.GlobalEvents((float)e.Time);
            Preview.ClientSize = (Size)ClientSize;
            MainMenuBar.Draw();
            ImGui.ShowDemoWindow();
            ImGui.ShowStackToolWindow();
            _controller.Render();
            time.Stop();
            Console.WriteLine(time.ElapsedTicks / 10_000f);

            ImGuiController.CheckGLError("End of frame");
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