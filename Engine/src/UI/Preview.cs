using Engine.Attributes;
using Engine.Core;
using Engine.Graphics;
using Engine.OpenGL;
using Engine.Utilities;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.UI
{
    [OpenedByDefault]
    public class Preview : IWindow
    {
        public static Size ClientSize { get; set; }

        private float _renderQuality = 1f;

        public string Name => "Preview";

        public void Show()
        {
            if (ImGui.Begin("Preview"))
            {
                var watch = Stopwatch.StartNew();
                Surface surface = Renderer.RenderActiveScene();
                watch.Stop();

                //Console.WriteLine(watch.ElapsedTicks / 10_000f);
                GL.Viewport(ClientSize);
                Framebuffer.Unbind(FramebufferTarget.Framebuffer);

                Vector2 windowSize = ImGui.GetContentRegionAvail() - new Vector2(0, 30);
                float windowAspectRatio = windowSize.GetAspectRatio();
                float sceneAspectRatio = App.Project.ActiveScene.AspectRatio;
                Vector2 previewSize;
                Vector2 offset;
                if (windowAspectRatio > sceneAspectRatio)
                {
                    float width = windowSize.Y * sceneAspectRatio;
                    offset = new Vector2((windowSize.X - width) / 2, 0);
                    previewSize = new Vector2(width, windowSize.Y);
                }
                else
                {
                    float height = windowSize.X / sceneAspectRatio;
                    offset = new Vector2(0, (windowSize.Y - height) / 2);
                    previewSize = new Vector2(windowSize.X, height);
                }

                ImGuiHelper.MoveCursorBy(offset);
                ImGui.Image((IntPtr)surface.Texture.Handle, previewSize, new Vector2(0f, 1f * Renderer.PreviewRatio), new Vector2(1f * Renderer.PreviewRatio, 0f));
                ImGui.Separator();
                ImGui.SliderFloat("quality", ref _renderQuality, 0f, 1f);
                Renderer.PreviewRatio = MathF.Min(previewSize.X / App.Project.ActiveScene.Size.Width, 1f) * _renderQuality;
                ImGui.SameLine();
                ImGui.Text(App.Project.Time.ToString());
            }
            ImGui.End();
        }
    }
}
