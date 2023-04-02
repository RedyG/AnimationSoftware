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

namespace Editor
{
    public static class UI
    {
        public static Window Window { get; set; }

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
            ImGui.Columns(2, "Parameters");
            foreach (NamedParameter namedParameter in namedParameters)
            {
                UI.KeyframeButton(namedParameter.Parameter);
                ImGui.SameLine();

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

        private static Layer? _draggingLayer;
        private static LayerMouseState _layerMouseState = LayerMouseState.None;
        private static float _timelineZoom = 1f;
        public static void TimelineWindow()
        {
            if (ImGui.Begin("Timeline"))
            {
                ImGui.Columns(2, "Layers");

                ImGui.NextColumn();

                var cursorPos = ImGui.GetCursorScreenPos();
                ImGui.InvisibleButton("a", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetFrameHeight()));
                var drawList = ImGui.GetWindowDrawList();

                int frames = App.Project.ActiveScene.Duration.Frames;
                float ratio = ImGui.GetContentRegionAvail().X / (float)frames;
                int step = 2;
                for (int i = 0; i < frames; i += step)
                {
                    if (i % 5 == 0)
                    {
                        //Vector2 labelSize = ImGui.CalcTextSize("999");
                        //drawList.AddText(cursorPos + new Vector2(i * ratio + 2, 0), Color(255, 255, 255, 255), i.ToString());
                        drawList.AddLine(cursorPos + new Vector2(i * ratio / step, 0), cursorPos + new Vector2(i * ratio / step, 20), Color(255, 255, 255, 255));
                    }
                    else
                    {
                        drawList.AddLine(cursorPos + new Vector2(i * ratio / step, 16), cursorPos + new Vector2(i * ratio / step, 20), Color(255, 255, 255, 255));
                    }
                }

                ImGui.NextColumn();
                
                if(ImGui.BeginChild("LeftChild"))
                {
                    foreach (Layer layer in App.Project.ActiveScene.Layers)
                    {
                        if (ImGui.CollapsingHeader(layer.Name))
                        {
                        }
                    }
                }
                ImGui.EndChild();
                ImGui.NextColumn();

                if(ImGui.BeginChild("RightChild", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.HorizontalScrollbar))
                {
                    cursorPos = ImGui.GetCursorScreenPos();
                    foreach (Layer layer in App.Project.ActiveScene.Layers)
                    {
                        ImGui.PushID(layer.GetHashCode());

                        Vector2 frameSize = GetFrameSize();
                        float layerX = (layer.InPoint.Seconds / App.Project.ActiveScene.Duration.Seconds) * frameSize.X * _timelineZoom;
                        float layerWidth = (layer.Duration.Seconds / App.Project.ActiveScene.Duration.Seconds) * frameSize.X * _timelineZoom;
                        Vector2 layerScreenPos = ImGui.GetCursorScreenPos() + new Vector2(layerX, 0f);
                        Vector2 layerScreenMax = layerScreenPos + new Vector2(layerWidth, frameSize.Y);

                        ImGui.SetCursorScreenPos(layerScreenPos);
                        ImGui.InvisibleButton("layer", new Vector2(layerWidth, frameSize.Y));
                        drawList.AddRectFilled(layerScreenPos, layerScreenMax, Color(255, 255, 255, 255), 3f);

                        bool isMouseDown = ImGui.IsMouseDown(ImGuiMouseButton.Left);
                        if (ImGui.IsItemHovered())
                        {
                            LayerMouseState layerMouseState = LayerMouseState.None;
                            float mouseX = ImGui.GetMousePos().X;
                            if (mouseX - layerScreenPos.X <= 5f)
                            {
                                drawList.AddRectFilled(layerScreenPos, layerScreenPos + new Vector2(5, frameSize.Y), Color(66, 150, 250, 255), 3f);
                                ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                                layerMouseState = LayerMouseState.InPoint;
                            }
                            else if ((layerScreenPos.X + layerWidth) - mouseX <= 5f)
                            {
                                drawList.AddRectFilled(layerScreenMax - new Vector2(5, frameSize.Y), layerScreenMax, Color(66, 150, 250, 255), 3f);
                                ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                                layerMouseState = LayerMouseState.OutPoint;
                            }
                            else
                                layerMouseState = LayerMouseState.Center;

                            if (isMouseDown && _layerMouseState == LayerMouseState.None)
                            {
                                _draggingLayer = layer;
                                _layerMouseState = layerMouseState;
                            }
                        }

                        if (_draggingLayer != null && _draggingLayer.Equals(layer))
                        {
                            if (isMouseDown)
                            {
                                float deltaX = ImGui.GetMouseDragDelta().X / frameSize.X / _timelineZoom;
                                Timecode diffTime = Timecode.FromSeconds(deltaX) * App.Project.ActiveScene.Duration;
                                if (_layerMouseState == LayerMouseState.Center)
                                {
                                    layer.Offset += diffTime;
                                }
                                else if (_layerMouseState == LayerMouseState.InPoint)
                                {
                                    drawList.AddRectFilled(layerScreenPos, layerScreenPos + new Vector2(5, frameSize.Y), Color(66, 150, 250, 255), 3f);
                                    ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                                    layer.InPoint += diffTime;
                                }
                                else if (_layerMouseState == LayerMouseState.OutPoint)
                                {
                                    drawList.AddRectFilled(layerScreenMax - new Vector2(5, frameSize.Y), layerScreenMax, Color(66, 150, 250, 255), 3f);
                                    ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                                    layer.OutPoint += diffTime;
                                }
                                if (deltaX != 0f)
                                    ImGui.ResetMouseDragDelta();
                            }
                            else
                            {
                                _draggingLayer = null;
                                _layerMouseState = LayerMouseState.None;
                            }
                        }
                        ImGui.PopID();
                    }
                    float cursorX = App.Project.Time.Seconds / App.Project.ActiveScene.Duration.Seconds * ImGui.GetContentRegionAvail().X;
                    drawList.AddLine(new Vector2(cursorX, 0) + cursorPos, new Vector2(cursorX, 100) + cursorPos, Color(255, 255, 255, 255));
                }
                ImGui.EndChild();


                ImGui.Columns();
            }
            ImGui.End();
        }

        public static Vector2 GetFrameSize() => new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetFrameHeight());

        public static void PreviewWindow()
        {
            if (ImGui.Begin("Preview"))
            {
                float a =  Renderer.PreviewRatio;
                ImGui.DragFloat("qualit", ref a, 0.01f);
                Renderer.PreviewRatio = a;
                Surface surface = Renderer.RenderActiveScene();
                GL.Viewport((Size)Window.ClientSize);
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

                UI.MoveCursorBy(offset);
                ImGui.Image((IntPtr)surface.Texture.Handle, previewSize, new Vector2(0f, 1f), new Vector2(1f, 0f));


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
                    parameter.AddKeyframeAtTime(App.Project.Time);
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

        public static void MoveCursorBy(Vector2 translate)
        {
            var cursorPos = ImGui.GetCursorScreenPos();
            ImGui.SetCursorScreenPos(cursorPos + translate);
        }
        public static void MoveCursorBy(float x, float y)
        {
            var cursorPos = ImGui.GetCursorScreenPos();
            ImGui.SetCursorScreenPos(cursorPos + new Vector2(x, y));
        }

        public static uint Color(byte r, byte g, byte b, byte a) { uint ret = a; ret <<= 8; ret += b; ret <<= 8; ret += g; ret <<= 8; ret += r; return ret; }
        public static uint Color(Vector4 vector) => Color((byte)(vector.X * 255f), (byte)(vector.Y * 255f), (byte)(vector.Z * 255f), (byte)(vector.W * 255f));
    }
}
