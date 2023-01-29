using Engine.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class Blur : FilterEffect
    {
        private static readonly string _src = @"
    uniform shader inputShader;
    uniform float height;
    uniform float width;
            
    vec4 main(vec2 coords) {
        vec4 currValue = sample(inputShader, coords);
        float top = height - 100;
        if (coords.y < top) {
            return currValue;
        } else {
            // Avoid blurring edges
            if (coords.x > 1 && coords.y > 1 &&
                    coords.x < (width - 1) &&
                    coords.y < (height - 1)) {
                // simple box blur - average 5x5 grid around pixel
                vec4 boxSum =
                    sample(inputShader,coords + vec2(-2, -2)) + 
                    // ...
                    currValue +
                    // ...
                    sample(inputShader,coords + vec2(2, 2));
                currValue = boxSum / 25;
            }
            
            const vec4 white = vec4(1);
            // top-left corner of label area
            vec2 lefttop = vec2(0, top);
            float lightenFactor = min(1.0, .6 *
                    length(coords - lefttop) /
                    (0.85 * length(vec2(width, 100))));
            // White in upper-left, blended increasingly
            // toward lower-right
            return mix(currValue, white, 1 - lightenFactor);
        }
    }
";

        private static readonly SKRuntimeEffect _effect = SKRuntimeEffect.Create(_src, out var errorText);
        public override SKShader GetShader(SKShader input)
        {
            var children = new SKRuntimeEffectChildren(_effect)
            {
                ["inputShader"] = input
            };

            var uniforms = new SKRuntimeEffectUniforms(_effect)
            {
                ["width"] = 300f,
                ["height"] = 300f
            };

            return _effect.ToShader(false, uniforms, children);
        }
    }
}
