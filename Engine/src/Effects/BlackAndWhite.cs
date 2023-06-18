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
    [Description(Category = "Filters", Name = "Black And White")]
    public class BlackAndWhite : VideoEffect
    {
        [Param] public Parameter<GrayScaleAlgorithm> Algorithm { get; } = new (GrayScaleAlgorithm.Luma);

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

        private static ShaderProgram _desaturation = Surface.CompileShader(@"
            vec4 surface()
            {
                vec4 color = texture(input, uv);
                float average = ( max(color.r, max(color.g, color.b)) + min(color.r, min(color.g, color.b)) ) / 2.0;
                return vec4(vec3(average), color.a);
            }
        ");

        private static ShaderProgram _maximumDecomposition = Surface.CompileShader(@"
            vec4 surface()
            {
                vec4 color = texture(input, uv);
                float average = max(color.r, max(color.g, color.b));
                return vec4(vec3(average), color.a);
            }
        ");

        private static ShaderProgram _minimumDecomposition = Surface.CompileShader(@"
            vec4 surface()
            {
                vec4 color = texture(input, uv);
                float average = min(color.r, min(color.g, color.b));
                return vec4(vec3(average), color.a);
            }
        ");

        private static ShaderProgram _red = Surface.CompileShader(@"
            vec4 surface()
            {
                vec4 color = texture(input, uv);
                return vec4(vec3(color.r), color.a);
            }
        ");

        private static ShaderProgram _green = Surface.CompileShader(@"
            vec4 surface()
            {
                vec4 color = texture(input, uv);
                return vec4(vec3(color.g), color.a);
            }
        ");

        private static ShaderProgram _blue = Surface.CompileShader(@"
            vec4 surface()
            {
                vec4 color = texture(input, uv);
                return vec4(vec3(color.b), color.a);
            }
        ");

        private static Dictionary<GrayScaleAlgorithm, ShaderProgram> _shaders = new Dictionary<GrayScaleAlgorithm, ShaderProgram>()
        {
            { GrayScaleAlgorithm.Average, _average },
            { GrayScaleAlgorithm.Luma, _luma },
            { GrayScaleAlgorithm.Desaturation, _desaturation },
            { GrayScaleAlgorithm.MaximumDecomposition, _maximumDecomposition },
            { GrayScaleAlgorithm.MinimumDecomposition, _minimumDecomposition },
            { GrayScaleAlgorithm.Red, _red },
            { GrayScaleAlgorithm.Green, _green },
            { GrayScaleAlgorithm.Blue, _blue },

        };

        public override RenderResult Render(RenderArgs args)
        {
            args.SurfaceB.Bind(FramebufferTarget.Framebuffer);
            GraphicsApi.DrawSurface(MatrixBuilder.Empty, args.SurfaceA, _shaders[Algorithm.GetValueAtTime(args.Time)]);

            return new RenderResult(true);
        }
    }

    public enum GrayScaleAlgorithm
    {
        Luma,
        Average,
        Desaturation,
        MaximumDecomposition,
        MinimumDecomposition,
        Red,
        Green,
        Blue
    }
}
