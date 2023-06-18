using Engine.Attributes;
using Engine.Core;
using Engine.Utilities;
using ImGuiNET;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Engine.UI
{
    [OpenedByDefault]
    public class Timeline : IWindow
    {
        private LayerMouseState _layerMouseState = LayerMouseState.None;
        private bool _dragging = false;
        private float _timelineZoom = 1f;

        public void TimelineLeft()
        {
            bool newLayer = false;
            ImGui.InvisibleButton("Temporary spacing", ImGuiHelper.GetLineSize());

            if (ImGui.BeginChild("LeftChild"))
            {
                DrawLayersLeft(App.Project.ActiveScene.Layers);
            }
            if (ImGui.BeginPopupContextWindow())
            {
                if (ImGui.MenuItem("New Layer"))
                    newLayer = true;

                ImGui.EndPopup();
            }

            ImGui.EndChild();

            if (newLayer)
                ImGui.OpenPopup("Aaa");

            if (ImGui.BeginPopupModal("Aaa"))
            {
                ImGui.Text("Lorem ipsum");
                if (ImGui.Button("A"))
                {
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }
        }

        private int _layerDepth = 0;

        public string Name => "Timeline";

        public void DrawLayersLeft(LayerList layers)
        {
            foreach (var layer in layers)
            {
                ImGui.PushID(layer.GetHashCode());

                ImGui.BeginChild("LayerLeft", ImGuiHelper.GetLineSize());

                var drawList = ImGui.GetWindowDrawList();
                drawList.AddRectFilled(ImGui.GetCursorScreenPos(), ImGui.GetCursorScreenPos() + ImGuiHelper.GetLineSize(), Colors.ToHex(ImGui.GetStyle().Colors[(int)ImGuiCol.PlotLinesHovered]));

                var visible = layer.Visible;
                ImGui.Checkbox("##visible", ref visible);
                layer.Visible = visible;
                ImGui.SameLine(0f, 2f);
                ImGuiHelper.MoveCursorBy((ImGui.GetFrameHeight() + 2) * _layerDepth, 0f);

                if (layer.IsGroup)
                {
                    ImGuiHelper.ArrowButton(layer.Name, ref layer.Opened);
                    ImGui.SameLine(0f, 2f);
                }
                else
                    ImGuiHelper.MoveCursorBy(ImGui.GetFrameHeight() + 2, 0f);

                ImGui.Text(layer.Name);
                ImGui.EndChild();

                bool delete = false;
                if (ImGui.BeginPopupContextItem())
                {
                    if (ImGui.MenuItem("Rename"))
                        delete = true;
                    ImGui.EndPopup();
                }

                if (delete)
                    ImGui.OpenPopup("Rename Layer");

                if (ImGui.BeginPopupModal("Rename Layer"))
                {
                    string name = layer.Name;
                    ImGui.InputText("", ref name, 999);
                    ImGuiHelper.EndValueChange<string>(name, (value) => layer.Name = value);
                    ImGui.SameLine();
                    if (ImGui.Button("Done"))
                    {
                        ImGui.CloseCurrentPopup();
                    }
                    
                    ImGui.EndPopup();
                }

                if (layer.Opened)
                {
                    _layerDepth++;

                    DrawLayersLeft(layer.Layers);
                    _layerDepth--;
                }


                ImGui.PopID();
            }
        }



        public void TimelineRight()
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
                drawList.AddLine(new Vector2(timeCursorX - 0.5f, 0) + cursorPos, new Vector2(timeCursorX - 0.5f, ImGui.GetContentRegionMax().Y) + cursorPos, Colors.BlueHex);
            }
            ImGui.EndChild();
            ImGui.PopStyleColor();

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                App.Project.ActiveScene.Layers.UnselectAll();
        }

        public void DrawLayersRight(LayerList layers, ImDrawListPtr drawList, Vector2 lineSize, Timecode diffTime)
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

                drawList.AddRectFilled(layerScreenPos, layerScreenMax, layer.Selected ? Colors.BlueHex : Colors.WhiteHex, 3f);

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
                    if (_layerMouseState == LayerMouseState.Center)
                    {
                        ImGuiHelper.BeginValueChange(layer.Offset);
                        ImGuiHelper.EndValueChange(layer.Offset + diffTime, newOffset => layer.Offset = newOffset);
                    }
                    else if (_layerMouseState == LayerMouseState.InPoint)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                        var inPoint = layer.InPoint + diffTime;
                        if (inPoint < layer.OutPoint)
                        {
                            ImGuiHelper.BeginValueChange(layer.InPoint);
                            ImGuiHelper.EndValueChange<Timecode>(inPoint, newInPoint => layer.InPoint = newInPoint);
                        }
                    }
                    else if (_layerMouseState == LayerMouseState.OutPoint)
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
                        var outPoint = layer.OutPoint + diffTime;
                        if (outPoint > layer.InPoint)
                        {
                            ImGuiHelper.BeginValueChange(layer.OutPoint);
                            ImGuiHelper.EndValueChange<Timecode>(outPoint, newOutPoint => layer.OutPoint = newOutPoint);
                        }
                    }

                    if (!isMouseDown)
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

        private void HandleLayerSelection(Layer layer)
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

        private void TimeCursor(float x)
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

        public void Show()
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
    }
}
