using Engine.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class NoChange : FilterEffect
    {
        private static string _src = @"
            uniform shader image;
            half4 main(float2 coord) {
                return sample(image) / 2;
            }";

        private static SKRuntimeEffect _effect = SKRuntimeEffect.Create(_src, out var errorText);
        public override SKShader MakeShader(FilterEffectArgs args)
        {
            var children = new SKRuntimeEffectChildren(_effect)
            {
                ["image"] = args.InputShader
            };

            var uniforms = new SKRuntimeEffectUniforms(_effect);

            return _effect.ToShader(false, uniforms, children);
        }
    }
}
