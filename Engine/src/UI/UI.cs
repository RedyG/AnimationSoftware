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


        public static void GlobalEvents(float delta)
        {
            if (ImGuiHelper.IsKeyPressed(Keys.Left))
                App.Project.Time.Frames -= ImGuiHelper.IsKeyDown(Keys.LeftShift) ? 10 : 1;

            if (ImGuiHelper.IsKeyPressed(Keys.Right))
                App.Project.Time.Frames += ImGuiHelper.IsKeyDown(Keys.LeftShift) ? 10 : 1;

            if (ImGuiHelper.IsKeyPressed(Keys.Space))
                App.Project.PreviewPlaying = !App.Project.PreviewPlaying;

            if (App.Project.PreviewPlaying)
                App.Project.Time.Seconds = (App.Project.Time.Seconds + delta) % App.Project.ActiveScene.Duration.Seconds;

            if (ImGuiHelper.Shortcut(Keys.Z, Keys.LeftControl))
            {
                if (ImGuiHelper.IsKeyDown(Keys.LeftShift))
                    CommandManager.Redo();
                else
                    CommandManager.Undo();
            }
        }

    }
}
