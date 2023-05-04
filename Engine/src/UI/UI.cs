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
using System.Reflection.Emit;
using Engine.Effects;

namespace Engine.UI
{
    public static class UI
    {
        public static ImFontPtr Font;
        public static Size ClientSize { get; set; }

        public static void EffectsWindow()
        {

            if (ImGui.Begin("Effects"))
            {
                // TODO: only for selected layers
                var selectedLayers = App.Project.ActiveScene.Layers.Selected;
                foreach (Layer layer in selectedLayers)
                {
                    ImGui.PushFont(Font);
                    var drawList = ImGui.GetWindowDrawList();
                    drawList.AddRectFilled(ImGui.GetCursorScreenPos(), ImGui.GetCursorScreenPos() + ImGuiHelper.GetLineSize(), Colors.MidGrayHex);
                    TextCentered(layer.Name);
                    ImGui.PopFont();

                    ImGui.Separator();

                    ImGui.PushID(layer.GetHashCode());

                    ImGuiHelper.MoveCursorBy(4f, 0f);
                    ImGui.Text("Miscellaneous");
                    
                    EffectUI(layer.Settings);
                    foreach (Effect effect in layer.OtherEffects)
                    {
                        EffectUI(effect);
                    }

                    if (layer.VideoEffects.Count > 0)
                    {
                        ImGuiHelper.MoveCursorBy(4f, 0f);
                        ImGui.Text("Video Effects");
                    }

                    foreach (Effect effect in layer.VideoEffects)
                    {
                        EffectUI(effect);
                    }
                    ImGui.PopID();
                }
                ImGuiHelper.MoveCursorBy(0, ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeight());
                ImGui.DragFloat("Width", ref _parameterWidth);
            }


            ImGui.End();
        }

        private static void TextCentered(string text)
        {
            var windowWidth = ImGui.GetWindowSize().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            ImGui.Text(text);
        }

        private static void TextCentered(string text, Vector4 color)
        {
            var windowWidth = ImGui.GetWindowSize().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            ImGui.TextColored(color, text);
        }

        private static float _parameterWidth = 400f;
        public static void EffectUI(Effect effect)
        {
            ImGui.PushID(effect.GetHashCode());
            string name = effect.Description.Name;

            var headerVisible = ImGui.CollapsingHeader(name);

            if (headerVisible)
            {
                ImGui.TreePush(name);
                foreach (NamedParameter namedParameter in effect.Parameters)
                {
                    ImGui.PushID(namedParameter.Name);

                    UI.KeyframeButton(namedParameter.Parameter);
                    ImGui.SameLine();

                    if (namedParameter.Parameter.UILocation == UILocation.Under)
                    {
                        bool opened = namedParameter.Parameter.Opened;
                        ArrowButton("##opened", ref opened);
                        ImGui.SameLine(0f, 4f);
                        namedParameter.Parameter.Opened = opened;
                    }

                    ImGui.Text(namedParameter.Name);

                    if (namedParameter.Parameter.UILocation == UILocation.Right)
                    {
                        ImGui.SameLine();
                        ImGuiHelper.MoveCursorBy(ImGui.GetContentRegionAvail().X - _parameterWidth, 0);
                        ImGui.SetNextItemWidth(_parameterWidth);
                        namedParameter.Parameter.DrawUI();
                    }
                    else
                    {
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                        if (namedParameter.Parameter.Opened)
                            namedParameter.Parameter.DrawUI();
                    }
                    ImGui.PopID();
                }
                ImGui.TreePop();
            }
            ImGui.PopID();
        }

        private static LayerMouseState _layerMouseState = LayerMouseState.None;
        private static bool _dragging = false;
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
            ImGui.InvisibleButton("Temporary spacing", ImGuiHelper.GetLineSize());

            if (ImGui.BeginChild("LeftChild"))
            {
                DrawLayers(App.Project.ActiveScene.Layers);
            }
            ImGui.EndChild();
        }

        private static int _layerDepth = 0;
        public static void DrawLayers(LayerList layers)
        {
            foreach (var layer in layers)
            {
                ImGui.PushID(layer.GetHashCode());

                var drawList = ImGui.GetWindowDrawList();
                drawList.AddRectFilled(ImGui.GetCursorScreenPos(), ImGui.GetCursorScreenPos() + ImGuiHelper.GetLineSize(), Colors.MidGrayHex);


                ImGui.Checkbox("##visible", ref layer.Visible);
                ImGui.SameLine(0f, 2f);
                ImGuiHelper.MoveCursorBy((ImGui.GetFrameHeight() + 2 ) * _layerDepth, 0f);

                if (layer.IsGroup)
                {
                    ArrowButton(layer.Name, ref layer.Opened);
                    ImGui.SameLine(0f, 2f);
                }
                else
                    ImGuiHelper.MoveCursorBy(ImGui.GetFrameHeight() + 2, 0f);

                ImGui.Text(layer.Name);

                if (layer.Opened)
                {
                    _layerDepth++;
                    DrawLayers(layer.Layers);
                    _layerDepth--;
                }


                ImGui.PopID();
            }
        }

        public static void ArrowButton(string id, ref bool opened)
        {
            ImGuiDir arrowDir = opened ? ImGuiDir.Down : ImGuiDir.Right;
            ImGui.ArrowButton(id, arrowDir);
            if (ImGui.IsItemClicked())
                opened = !opened;
        }

        public static void TimelineRight()
        {
            ImGui.PushStyleColor(ImGuiCol.ChildBg, Colors.MidGray);
            if (ImGui.BeginChild("RightChild", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.HorizontalScrollbar))
            {
                float timeCursorX = App.Project.Time.Seconds / App.Project.ActiveScene.Duration.Seconds * ImGui.GetContentRegionAvail().X;
                TimeCursor(timeCursorX);

                var cursorPos = ImGui.GetCursorScreenPos();
                var drawList = ImGui.GetWindowDrawList();

                Vector2 lineSize = ImGuiHelper.GetLineSize();
                float deltaX = ImGui.GetMouseDragDelta().X / lineSize.X / _timelineZoom;
                Timecode diffTime = Timecode.FromSeconds(deltaX) * App.Project.ActiveScene.Duration;

                DrawLayersRight(App.Project.ActiveScene.Layers, drawList, lineSize, diffTime);


                if (deltaX != 0f)
                    ImGui.ResetMouseDragDelta();
                drawList.AddLine(new Vector2(timeCursorX - 0.5f, 0) + cursorPos, new Vector2(timeCursorX - 0.5f, ImGui.GetContentRegionMax().Y) + cursorPos, 0xfff6823b);
            }
            ImGui.EndChild();
            ImGui.PopStyleColor();

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                App.Project.ActiveScene.Layers.UnselectAll();
        }

        public static void DrawLayersRight(LayerList layers, ImDrawListPtr drawList, Vector2 lineSize, Timecode diffTime)
        {
            foreach (Layer layer in layers)
            {
                ImGui.PushID(layer.GetHashCode());

                float layerX = (layer.InPoint.Seconds / App.Project.ActiveScene.Duration.Seconds) * lineSize.X * _timelineZoom;
                float layerWidth = (layer.Duration.Seconds / App.Project.ActiveScene.Duration.Seconds) * lineSize.X * _timelineZoom;
                Vector2 layerScreenPos = ImGui.GetCursorScreenPos() + new Vector2(layerX, 0f);
                Vector2 layerScreenMax = layerScreenPos + new Vector2(layerWidth, lineSize.Y);

                ImGui.SetCursorScreenPos(layerScreenPos);
                ImGui.InvisibleButton("layer", new Vector2(layerWidth, lineSize.Y));
                drawList.AddRectFilled(layerScreenPos, layerScreenMax, layer.Selected ? Color(255, 0, 0, 255) : Color(255, 255, 255, 255), 3f);

                bool isMouseDown = ImGui.IsMouseDown(ImGuiMouseButton.Left);

                HandleLayerSelection(layer);
                if (ImGui.IsItemHovered())
                {
                    LayerMouseState newLayerMouseState;
                    float mouseX = ImGui.GetMousePos().X;
                    if (mouseX - layerScreenPos.X <= 5f)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                        newLayerMouseState = LayerMouseState.InPoint;
                    }
                    else if ((layerScreenPos.X + layerWidth) - mouseX <= 5f)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                        newLayerMouseState = LayerMouseState.OutPoint;
                    }
                    else
                        newLayerMouseState = LayerMouseState.Center;

                    if (isMouseDown && _layerMouseState == LayerMouseState.None)
                    {
                        _dragging = true;
                        _layerMouseState = newLayerMouseState;
                    }
                }

                if (layer.Selected && _dragging)
                {
                    if (isMouseDown)
                    {
                        if (_layerMouseState == LayerMouseState.Center)
                        {
                            layer.Offset += diffTime;
                        }
                        else if (_layerMouseState == LayerMouseState.InPoint)
                        {
                            ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                            var inPoint = layer.InPoint + diffTime;
                            if (inPoint < layer.OutPoint)
                                layer.InPoint = inPoint;
                        }
                        else if (_layerMouseState == LayerMouseState.OutPoint)
                        {
                            ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                            var outPoint = layer.OutPoint + diffTime;
                            if (outPoint > layer.InPoint)
                                layer.OutPoint = outPoint;
                        }
                    }
                    else
                    {
                        _dragging = false;
                        _layerMouseState = LayerMouseState.None;
                    }
                }

                if (layer.Opened)
                {
                    DrawLayersRight(layer.Layers, drawList, lineSize, diffTime);
                }

                ImGui.PopID();
            }
        }


        private static void HandleLayerSelection(Layer layer)
        {
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                if (ImGuiHelper.IsKeyDown(Keys.LeftControl))
                {
                    layer.Selected = !layer.Selected;
                }
                else if (ImGuiHelper.IsKeyDown(Keys.LeftShift))
                {
                    // TODO: support shift select
                }
                else
                {
                    if (!layer.Selected)
                    {
                        App.Project.ActiveScene.Layers.UnselectAll();
                        layer.Selected = true;
                    }
                }
            }
        }

        private static void TimeCursor(float x)
        {
            var cursorPos = ImGui.GetCursorScreenPos();
            float frameHeight = ImGui.GetFrameHeight();
            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled(cursorPos, cursorPos + ImGuiHelper.GetLineSize(), Colors.BackgroundHex);

            ImGui.InvisibleButton("Timeline Ruler", ImGuiHelper.GetLineSize());

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
            if (ImGui.Begin("Effects list window"))
            {
                foreach (var categories in Effect.Effects)
                {
                    if (ImGui.TreeNode(categories.Key))
                    {
                        foreach (var namedEffect in categories.Value)
                        {
                            if (ImGui.Button(namedEffect.Key))
                            {
                                foreach (Layer layer in App.Project.ActiveScene.Layers.Selected)
                                {
                                    layer.AddEffect(Effect.Create(namedEffect.Value));
                                }
                            }
                        }
                        ImGui.TreePop();
                    }
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

        private static uint Color(Vector4 color) => Color((byte)(color.X * 255f), (byte)(color.Y * 255f), (byte)(color.Z * 255f), (byte)(color.W * 255f));
        private static uint Color(byte r, byte g, byte b, byte a) { uint ret = a; ret <<= 8; ret += b; ret <<= 8; ret += g; ret <<= 8; ret += r; return ret; }
    }
}
