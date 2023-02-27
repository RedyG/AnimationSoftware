using Engine.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class Wave : FilterEffect
    {
        private static string _src = @"
        half4 main(float2 coord) {
          return half4(half3(fract(sin(dot(coord, float2(12.9898, 78.233))) * 43758.5453)), 1);
        }";

        public static SKRuntimeEffect _effect = SKRuntimeEffect.Create(_src, out string errors);
        public override SKShader MakeShader(FilterEffectArgs args)
        {
            SKRuntimeEffect.Create(_src, out string errors);
            var uniforms = new SKRuntimeEffectUniforms(_effect);

            return _effect.ToShader(true);
        }
    }
}
