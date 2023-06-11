using Engine.Attributes;
using Engine.Core;
using Engine.Graphics;
using Engine.OpenGL;
using Engine.Utilities;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    [Description(Category = "Content")]
    public class Rectangle : VideoEffect
    {
        public Parameter<PointF> Position { get; set; } = new Parameter<PointF>(new PointF(0f, 0f));
        public Parameter<SizeF> Size { get; set; } = new Parameter<SizeF>(new SizeF(100f, 100f));
        public Parameter<OpenTK.Mathematics.Color4> Color { get; set; } = new(OpenTK.Mathematics.Color4.White);
        public Parameter<bool> FitToLayer { get; set; } = new Parameter<bool>(true);
        public Parameter<float> idk  { get; set; } = new Parameter<float>(20f, true, true, (value) => Math.Max(Math.Min(value, 100f), 0f));
        public Parameter<int> secondIdk { get; set; } = new Parameter<int>(20);




        public override RenderResult Render(RenderArgs args)
        {
           // GraphicsApi.Clear(Color.Value);
               if (FitToLayer.GetValueAtTime(args.Time))
                   GraphicsApi.Clear(Color.GetValueAtTime(args.Time));
               else
                   GraphicsApi.DrawRect(
                       MatrixBuilder.CreateTransform(
                           args.Layer.Transform.Size.GetValueAtTime(args.Time),
                           Position.GetValueAtTime(args.Time),
                           Size.GetValueAtTime(args.Time),
                           PointF.Empty,
                           new Vector2(Renderer.PreviewRatio),
                           0f
                       ),
                       Color.GetValueAtTime(args.Time)
                   );
               
            return new(false);
        }

        protected override ParameterList InitParameters() => new ParameterList(
            new("Position", Position),
            new("Size", Size),
            new("Color", Color),
            new("Fit To Layer", FitToLayer),
            new("idk", idk),
            new("second Idk", secondIdk)
        );

        public Rectangle()
        {
            secondIdk.LinkedParameter = idk;
        }
    }
}


/*

Fps 60
Render precomp
    Fps 30
    Render layer back 5 frames with ActiveScene.Time--;

*/
