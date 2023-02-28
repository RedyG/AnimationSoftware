using Engine.Core;
using Engine.Graphics;
using Engine.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class Rectangle : ContentEffect
    {
        public override void Render(Framebuffer framebuffer, SizeF size)
        {
            GraphicsApi.DrawRect(0, 0, 200, 200);
        }
    }
}

/*

Fps 60
Render precomp
    Fps 30
    Render layer back 5 frames with ActiveScene.Time--;

*/
