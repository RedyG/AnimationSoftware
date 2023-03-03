using Engine.Core;
using Engine.Graphics;
using Engine.OpenGL;
using Engine.Utilities;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class Rectangle : Effect
    {

        public override RenderResult Render(Surface mainSurface, Surface secondSurface)
        {
            GraphicsApi.DrawRect(MatrixBuilder.CreateTransform(new PointF(0f, 0f), new SizeF(0.5f, 0.5f)), App.Project!.ActiveScene!.AspectRatio, Color.Red);

            return new(false);
        }
    }
}


/*

Fps 60
Render precomp
    Fps 30
    Render layer back 5 frames with ActiveScene.Time--;

*/
