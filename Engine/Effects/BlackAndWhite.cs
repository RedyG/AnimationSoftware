using Engine.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class BlackAndWhite : FilterEffect
    {
        private static readonly string _src = @"
        uniform shader inputShader;

        half4 main(float2 coord)
        {
            half4 color = sample(inputShader);
            return half4(half3(color.r * 0.3 + color.g * 0.59 + color.b * 0.11), color.a);
        }
        ";

        private static readonly SKRuntimeEffect _effect = SKRuntimeEffect.Create(_src, out var errorText);
        public override SKShader MakeShader(FilterEffectArgs args)
        {
            SKRuntimeEffect.Create(_src, out var errorText);
            var children = new SKRuntimeEffectChildren(_effect)
            {
                ["inputShader"] = args.InputShader
            };

            var uniforms = new SKRuntimeEffectUniforms(_effect);

            return _effect.ToShader(false, uniforms, children);
        }
    }
}
