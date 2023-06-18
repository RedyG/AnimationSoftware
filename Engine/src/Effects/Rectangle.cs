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
        [Param] public Parameter<PointF> Position { get; set; } = new Parameter<PointF>(new PointF(0f, 0f));
        [Param] public Parameter<SizeF> Size { get; set; } = new Parameter<SizeF>(new SizeF(100f, 100f));
        [Param] public Parameter<OpenTK.Mathematics.Color4> Color { get; set; } = new(OpenTK.Mathematics.Color4.White);
        [Param] public Parameter<bool> FitToLayer { get; set; } = new Parameter<bool>(true);


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
                           Vector2.One,
                           0f
                       ),
                       Color.GetValueAtTime(args.Time)
                   );
               
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
