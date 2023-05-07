using Engine.Attributes;
using Engine.Core;
using Engine.Graphics;
using Engine.OpenGL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    [EffectDesc(Category = "Filters", Name = "Black And White")]
    public class BlackAndWhite : VideoEffect
    {
        public Parameter<GrayScaleAlgorithm> Algorithm { get; set; } = new (GrayScaleAlgorithm.Luma);

        private static ShaderProgram _average = Surface.CompileShader(@"
            vec4 surface()
            {
                vec4 color = texture(input, uv);
                float average = (color.r + color.g + color.b) / 3;
                return vec4(vec3(average), color.a);
            }
        ");

        private static ShaderProgram _luma = Surface.CompileShader(@"
            vec4 surface()
            {
                vec4 color = texture(input, uv);
                float average = color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
                return vec4(vec3(average), color.a);
            }
        ");

        private static Dictionary<GrayScaleAlgorithm, ShaderProgram> _shaders = new Dictionary<GrayScaleAlgorithm, ShaderProgram>()
        {
            { GrayScaleAlgorithm.Average, _average },
            { GrayScaleAlgorithm.Luma, _luma }

        };

        public override RenderResult Render(RenderArgs args)
        {
            args.SurfaceB.Bind(FramebufferTarget.Framebuffer);
            Console.WriteLine(Algorithm.GetValueAtTime(args.Time));
            GraphicsApi.DrawSurface(MatrixBuilder.Empty, args.SurfaceA, _shaders[Algorithm.GetValueAtTime(args.Time)]);

            return new RenderResult(true);
        }

        protected override ParameterList InitParameters() => new(
            new NamedParameter("Algorithm", Algorithm)
        );
    }

    public enum GrayScaleAlgorithm
    {
        Luma,
        Average,
    }
}
