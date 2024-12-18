﻿using Engine.Core;
using Engine.Graphics;
using Engine.OpenGL;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects
{
    public class AntiAliasing : VideoEffect
    {
        private static ShaderProgram _shader = Surface.CompileShader(@"
const float FXAA_SPAN_MAX = 2.0;
const float FXAA_REDUCE_MUL = 1.0/2.0;
const float FXAA_REDUCE_MIN = 1.0/128.0;

vec4 surface()
{
    vec2 offset = 1.0/iResolution.xy;
    
    vec3 nw = texture(input, uv + vec2(-1.0, -1.0) * offset).rgb;
    vec3 ne = texture(input, uv + vec2( 1.0, -1.0) * offset).rgb;
    vec3 sw = texture(input, uv + vec2(-1.0,  1.0) * offset).rgb;
    vec3 se = texture(input, uv + vec2( 1.0,  1.0) * offset).rgb;
    vec4 color = texture(input, uv);
    vec3 m = color.rgb;

    vec3 luma = vec3(0.299, 0.587, 0.114);
    float lumaNW = dot(nw, luma);
    float lumaNE = dot(ne, luma);
    float lumaSW = dot(sw, luma);
    float lumaSE = dot(se, luma);
    float lumaM  = dot(m,  luma);

    float lumaMin = min(lumaM, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
    float lumaMax = max(lumaM, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));
    vec2 dir = vec2(
        -((lumaNW + lumaNE) - (lumaSW + lumaSE)),
        ((lumaNW + lumaSW) - (lumaNE + lumaSE)));

    float dirReduce = max((lumaNW + lumaNE + lumaSW + lumaSE) * (0.25 * FXAA_REDUCE_MUL), FXAA_REDUCE_MIN);
    float rcpDirMin = 1.0 / (min(abs(dir.x), abs(dir.y)) + dirReduce);
    dir = min(vec2(FXAA_SPAN_MAX), max(vec2(-FXAA_SPAN_MAX), dir * rcpDirMin)) * offset;

    vec3 rgbA = 0.5 * (texture(input, uv + dir * (1.0 / 3.0 - 0.5)).xyz + texture(input, uv + dir * (2.0 / 3.0 - 0.5)).xyz);
    vec3 rgbB = rgbA * 0.5 + 0.25 * (texture(input, uv + dir * -0.5).xyz + texture(input, uv + dir * 0.5).xyz);
    float lumaB = dot(rgbB, luma);
    if (lumaB < lumaMin || lumaB > lumaMax) {
        return vec4(rgbA, color.a);
    } else {
        return vec4(rgbB, color.a);
    }
}
");

        public override RenderResult Render(RenderArgs args)
        {

            args.SurfaceB.Bind(FramebufferTarget.Framebuffer);
            GraphicsApi.Clear(new OpenTK.Mathematics.Color4(0f, 0f, 0f, 0f));
            GraphicsApi.DrawSurface(MatrixBuilder.Empty, args.SurfaceA, _shader);

            return new RenderResult(true);
        }
    }
}
