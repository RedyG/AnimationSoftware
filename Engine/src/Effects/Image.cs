﻿using Engine.Core;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Engine.OpenGL;
using OpenTK.Graphics.OpenGL4;

namespace Engine.Effects
{
    public class Image : Effect
    {
        Texture texture;

        public override RenderResult Render(Surface activeSurface, Surface secondSurface, SizeF size)
        {
            GraphicsApi.Clear(Color4.Azure);
            //GraphicsApi.DrawTexture(MatrixBuilder.Empty, texture);

            return new RenderResult(false);
        }
        public Image()
        {
            texture = Texture.FromImage("Z:\\1.jpg", TextureTarget.Texture2D);
        }
    }
}