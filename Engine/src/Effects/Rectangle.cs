﻿using Engine.Core;
using Engine.Graphics;
using Engine.OpenGL;
using Engine.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
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
        public Parameter<PointF> Position { get; set; } = new Parameter<PointF>(new PointF(0f, 0f));
        public Parameter<SizeF> Size { get; set; } = new Parameter<SizeF>(new SizeF(100f, 100f));
        public Parameter<Color4> Color { get; set; } = new Parameter<Color4>(Color4.White);
        public Parameter<bool> FitToLayer { get; set; } = new Parameter<bool>(true);


        public override RenderResult Render(Surface mainSurface, Surface secondSurface, SizeF size)
        {
           // GraphicsApi.Clear(Color.Value);
               if (FitToLayer.Value)
                   GraphicsApi.Clear(Color.Value);
               /*else
                   GraphicsApi.DrawRect(MatrixBuilder.CreateTransform(Position.Value, Size.Value), Color.Value);
               */
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
