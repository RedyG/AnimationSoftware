using Engine.Core;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class NoChange : Effect
    {
        private static string _src = @"
            uniform shader input;
            uniform float amount;

            half4 main(float2 coord) {
                return sample(input) / amount;
            }
        ";


        public override RenderResult Render(Surface mainSurface, Surface secondSurface)
        {
            return new RenderResult(true);
        }
    }
}
