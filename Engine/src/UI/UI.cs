using Engine.Core;
using ImGuiNET;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.OpenGL;
using OpenTK.Graphics.OpenGL4;
using System.Collections.ObjectModel;
using Engine.Utilities;
using Engine.Graphics;
using System.Drawing;
using System.Diagnostics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.UI
{
    public static class UI
    {
        public static Size ClientSize { get; set; }

        public static void EffectsWindow()
        {
            if (ImGui.Begin("Effects"))
            {
                // TODO: only for selected layers
                foreach (Layer layer in App.Project.ActiveScene.Layers)
                {
                    ImGui.Text("layer: " + layer.Name);
                    ImGui.PushID(layer.GetHashCode());

                    if (ImGui.CollapsingHeader("Layer Settings"))
                    {
                        UI.Parameters(ReflectionUtilities.GetParameters(layer));
                    }

                    foreach (Effect effect in layer.Effects)
                    {
                        ImGui.PushID(effect.GetHashCode());
                        if (ImGui.CollapsingHeader(effect.Name))
                        {
                            UI.Parameters(effect.Parameters);
                        }
                        ImGui.PopID();
                    }
                    ImGui.PopID();
                }
            }
            ImGui.End();
        }

        public static void Parameters(ParameterList namedParameters)
        {
            foreach (NamedParameter namedParameter in namedParameters)
            {
                //UI.KeyframeButton(namedParameter.Parameter);
                //ImGui.SameLine();


                if (namedParameter.Name == "Size")
                {
                    ImGuiHelper.MoveCursorBy(new Vector2(-21, 0));
                    if (ImGui.TreeNode(namedParameter.Name))
                    {
                        //namedParameter.Parameter.DrawUI();
                        ImGui.TreePop();
                    }
                }
                else
                    ImGui.Text(namedParameter.Name);
                //ImGui.NextColumn();
                ImGui.SameLine();

                ImGui.PushID(namedParameter.Name);
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                namedParameter.Parameter.DrawUI();
                ImGui.PopID();

                //ImGui.NextColumn();
            }
            ImGui.Columns();
        }

        private static LayerMouseState _layerMouseState = LayerMouseState.None;
        private static float _timelineZoom = 1f;
        public static void TimelineWindow()
        {
            if (ImGui.Begin("Timeline"))
            {
                ImGui.Columns(2, "Layers");
                TimelineLeft();

                ImGui.NextColumn();

                TimelineRight();

                ImGui.Columns();
            }
            ImGui.End();
        }

        public static void TimelineLeft()
        {
            ImGui.NewLine();

            if (ImGui.BeginChild("LeftChild"))
            {
                foreach (Layer layer in App.Project.ActiveScene.Layers)
                {
                    if (ImGui.CollapsingHeader(layer.Name))
                    {
                    }
                }
            }
            ImGui.EndChild();
        }
        public static void TimelineRight()
        {
            if (ImGui.BeginChild("RightChild", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.HorizontalScrollbar))
            {
                float timeCursorX = App.Project.Time.Seconds / App.Project.ActiveScene.Duration.Seconds * ImGui.GetContentRegionAvail().X;
                TimeCursor(timeCursorX);

                var cursorPos = ImGui.GetCursorScreenPos();
                var drawList = ImGui.GetWindowDrawList();
                foreach (Layer layer in App.Project.ActiveScene.Layers)
                {
                    ImGui.PushID(layer.GetHashCode());

                    Vector2 lineSize = ImGuiHelper.GetLineSize();
                    float layerX = (layer.InPoint.Seconds / App.Project.ActiveScene.Duration.Seconds) * lineSize.X * _timelineZoom;
                    float layerWidth = (layer.Duration.Seconds / App.Project.ActiveScene.Duration.Seconds) * lineSize.X * _timelineZoom;
                    Vector2 layerScreenPos = ImGui.GetCursorScreenPos() + new Vector2(layerX, 0f);
                    Vector2 layerScreenMax = layerScreenPos + new Vector2(layerWidth, lineSize.Y);

                    ImGui.SetCursorScreenPos(layerScreenPos);
                    ImGui.InvisibleButton("layer", new Vector2(layerWidth, lineSize.Y));
                    drawList.AddRectFilled(layerScreenPos, layerScreenMax, Color(255, 255, 255, 255), 3f);

                    bool isMouseDown = ImGui.IsMouseDown(ImGuiMouseButton.Left);
                    if (ImGui.IsItemHovered())
                    {
                        LayerMouseState layerMouseState = LayerMouseState.None;
                        float mouseX = ImGui.GetMousePos().X;
                        if (mouseX - layerScreenPos.X <= 5f)
                        {
                            drawList.AddRectFilled(layerScreenPos, layerScreenPos + new Vector2(5, lineSize.Y), Color(66, 150, 250, 255), 3f);
                            ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                            layerMouseState = LayerMouseState.InPoint;
                        }
                        else if ((layerScreenPos.X + layerWidth) - mouseX <= 5f)
                        {
                            drawList.AddRectFilled(layerScreenMax - new Vector2(5, lineSize.Y), layerScreenMax, Color(66, 150, 250, 255), 3f);
                            ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                            layerMouseState = LayerMouseState.OutPoint;
                        }
                        else
                            layerMouseState = LayerMouseState.Center;

                        if (isMouseDown && _layerMouseState == LayerMouseState.None)
                        {
                            layer.Selected = true;
                            _layerMouseState = layerMouseState;
                        }
                    }

                    if (layer.Selected)
                    {
                        if (isMouseDown)
                        {
                            float deltaX = ImGui.GetMouseDragDelta().X / lineSize.X / _timelineZoom;
                            Timecode diffTime = Timecode.FromSeconds(deltaX) * App.Project.ActiveScene.Duration;
                            if (_layerMouseState == LayerMouseState.Center)
                            {
                                layer.Offset += diffTime;
                            }
                            else if (_layerMouseState == LayerMouseState.InPoint)
                            {
                                drawList.AddRectFilled(layerScreenPos, layerScreenPos + new Vector2(5, lineSize.Y), Color(66, 150, 250, 255), 3f);
                                ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                                layer.InPoint += diffTime;
                            }
                            else if (_layerMouseState == LayerMouseState.OutPoint)
                            {
                                drawList.AddRectFilled(layerScreenMax - new Vector2(5, lineSize.Y), layerScreenMax, Color(66, 150, 250, 255), 3f);
                                ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                                layer.OutPoint += diffTime;
                            }
                            if (deltaX != 0f)
                                ImGui.ResetMouseDragDelta();
                        }
                        else
                        {
                            layer.Selected = false;
                            _layerMouseState = LayerMouseState.None;
                        }
                    }
                    ImGui.PopID();
                }
                drawList.AddLine(new Vector2(timeCursorX - 0.5f, 0) + cursorPos, new Vector2(timeCursorX - 0.5f, ImGui.GetContentRegionMax().Y) + cursorPos, 0xfff6823b);
            }
            ImGui.EndChild(); 
        }
        public static void TimeCursor(float x)
        {
            var cursorPos = ImGui.GetCursorScreenPos();
            float frameHeight = ImGui.GetFrameHeight();
            var drawList = ImGui.GetWindowDrawList();

            ImGui.InvisibleButton("Timeline Ruler", new Vector2(ImGui.GetContentRegionAvail().X, frameHeight));

            Vector2 timeBottom = cursorPos + new Vector2(x, frameHeight);
            drawList.PathLineTo(timeBottom + new Vector2(6f, -7f));
            drawList.PathLineTo(timeBottom + new Vector2(6f, -14f));
            drawList.PathLineTo(timeBottom + new Vector2(-6f, -14f));
            drawList.PathLineTo(timeBottom + new Vector2(-6f, -7f));
            drawList.PathLineTo(timeBottom);
            drawList.PathFillConvex(0xfff6823b);

            if (ImGui.IsItemActive())
            {
                float mouseX = ImGui.GetMousePos().X;
                App.Project.Time = Timecode.FromSeconds(
                    MathUtilities.MinMax((mouseX - cursorPos.X) / ImGui.GetContentRegionAvail().X, 0f, 1f)
                    * App.Project.ActiveScene.Duration.Seconds);
            }
        }
        public static void PreviewEvents(float delta)
        {
            if (ImGuiHelper.IsKeyPressed(Keys.Left))
                App.Project.Time.Frames -= 1;

            if (ImGuiHelper.IsKeyPressed(Keys.Right))
                App.Project.Time.Frames += 1;

            if (ImGuiHelper.IsKeyPressed(Keys.Space))
                App.Project.PreviewPlaying = !App.Project.PreviewPlaying;

            if (App.Project.PreviewPlaying)
                App.Project.Time.Seconds = (App.Project.Time.Seconds + delta) % App.Project.ActiveScene.Duration.Seconds;
        }


        private static float _renderQuality = 1f;
        public static void PreviewWindow()
        {
            if (ImGui.Begin("Preview"))
            {
                var watch = Stopwatch.StartNew();
                Surface surface = Renderer.RenderActiveScene();
                watch.Stop();
                Console.WriteLine(watch.ElapsedTicks / 10_000f);
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

        public static void EffectListWindow()
        {
            Effect.RefreshEffects();
            if (ImGui.Begin("Effects list window"))
            {
                foreach (Type effect in Effect.Effects)
                {
                    ImGui.TreeNode(Effect.GetName(effect));
                }
            }
            ImGui.End();
        }

        public static void KeyframeButton(Parameter parameter)
        {
            var drawList = ImGui.GetWindowDrawList();
            var cursorPos = ImGui.GetCursorScreenPos();
            var keyframed = parameter.IsKeyframedAtTime(App.Project.Time);

            if (ImGui.InvisibleButton(parameter.GetHashCode().ToString(), new(16, 16)))
            {
                if (keyframed)
                    parameter.RemoveNearestKeyframeAtTime(App.Project.Time);
                else
                    parameter.AddKeyframeAtTime(App.Project.Time, IEasing.Linear);
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                drawList.AddRectFilled(cursorPos, cursorPos + new Vector2(16, 16), Color(ImGui.GetStyle().Colors[(int)ImGuiCol.Button]));
            }
            drawList.PathLineTo(cursorPos + new Vector2(8, 3));
            drawList.PathLineTo(cursorPos + new Vector2(13, 8));
            drawList.PathLineTo(cursorPos + new Vector2(8, 13));
            drawList.PathLineTo(cursorPos + new Vector2(3, 8));
            var color = Color(ImGui.GetStyle().Colors[(int)ImGuiCol.CheckMark]);
            drawList.PathFillConvex(keyframed ? color : Color(ImGui.GetStyle().Colors[(int)ImGuiCol.Button]));

            drawList.PathLineTo(cursorPos + new Vector2(8, 3));
            drawList.PathLineTo(cursorPos + new Vector2(13, 8));
            drawList.PathLineTo(cursorPos + new Vector2(8, 13));
            drawList.PathLineTo(cursorPos + new Vector2(3, 8));
            drawList.PathLineTo(cursorPos + new Vector2(8, 3));
            drawList.PathStroke(Color(ImGui.GetStyle().Colors[(int)ImGuiCol.Text]));
        }


        public static uint Color(byte r, byte g, byte b, byte a) { uint ret = a; ret <<= 8; ret += b; ret <<= 8; ret += g; ret <<= 8; ret += r; return ret; }
        public static uint Color(Vector4 vector) => Color((byte)(vector.X * 255f), (byte)(vector.Y * 255f), (byte)(vector.Z * 255f), (byte)(vector.W * 255f));
    }
}
