using Engine.Core;
using Engine.Graphics;
using Engine.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Engine.Effects
{
    public class Shader : VideoEffect
    {
        public Parameter<float> Time { get; } = new Parameter<float>(0f);
        public Parameter<string> Code { get; } = new Parameter<string>(string.Empty, false, true);

        private static uint[] rectIndices = {
            0, 1, 3, // left triange
            2, 1, 3, // right triangle
        };
        private static float[] textureVertices = {
             1f,  1f, 1.0f, 0.0f,   // bottom right
             1f,  0f, 1.0f, 1.0f,   // top right
             0f,  0f, 0.0f, 1.0f,   // top left
             0f,  1f, 0.0f, 0.0f    // bottom left
        };
        private static ShaderProgram textureShader;
        private static VertexArray textureVao;
        private static Buffer<float> textureVbo;
        private static Buffer<uint> textureEbo;
        public override RenderResult Render(RenderArgs args)
        {
            args.SurfaceA.Bind(FramebufferTarget.Framebuffer);

            Matrix4 matrix = MatrixBuilder.TopLeft;

            textureShader.Uniform1(textureShader.GetUniformLocation("iTime"), Time.GetValueAtTime(args.Time));
            textureShader.Uniform3(textureShader.GetUniformLocation("iResolution"), args.SurfaceA.Size.Width, args.SurfaceA.Size.Height, 0);
            textureShader.UniformMatrix4(textureShader.GetUniformLocation("transform"), ref matrix);
            textureShader.Bind();
            textureEbo.Bind();
            textureVao.Bind();
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            return new RenderResult(false);
        }
        static Shader()
        {
            string vertexShaderSource = @"
                #version 330 core
                
                layout(location = 0) in vec3 aPos;
                layout(location = 1) in vec2 aTexCoord;                
                
                out vec2 tempUv;

                uniform mat4 transform;

                void main()
                {
                    tempUv = aTexCoord;
                    gl_Position = vec4(aPos, 1.0) * transform;
                }
            ";
            //string fragmentShaderSource = "#version 330 core\r\n// Splitting DNA by Martijn Steinrucken aka BigWings - 2017\r\n// Email:countfrolic@gmail.com Twitter:@The_ArtOfCode\r\n// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.\r\n//\r\n// Its still a little slow. I tried a bunch of things to optimize:\r\n// Using raytracing, instead of marching:  works, is significantly faster but I couldn't get rid of artifacts. \r\n// Using bounding volumes: makes it a little bit faster, though not nearly as much as I had hoped.\r\n// Only calculating the color once in the end: should save a ton of mix-es, not noticably faster\r\n// Skipping to the next strand when marching away from current one: works, makes it a little faster\r\n// Mirroring the backbone: doesn't have any noticable effect\r\n//\r\n// Took me a loong time to figure out the atomic structure of the bases, its not easy to figure \r\n// out from 2d pictures, I might very well have made a mistake.\r\n//\r\n// Use the mouse to look around a little bit.\r\n//\r\n// Anyways, worked on this for too long already, gotta ship it. Hope you like!\r\n\r\n\r\n#define INVERTMOUSE -1.\r\n\r\n// comment out to see one basepair by itself\r\n#define STRANDS\r\n\r\n#define MAX_INT_STEPS 100\r\n\r\n#define MIN_DISTANCE 0.1\r\n#define MAX_DISTANCE 1000.\r\n#define RAY_PRECISION .1\r\n\r\n#define OPTIMIZED\r\n#define USE_BOUNDING_VOLUMES\r\n// set to -1 to see bounding spheres\r\n#define SHOW_BOUNDING_VOLUMES 1.  \r\n\r\nout vec4 fragColor;\r\nin vec2 tempUv;\r\n\r\nuniform float iTime;\r\nuniform vec3 iResolution; \r\n\r\n#define S(x,y,z) smoothstep(x,y,z)\r\n#define B(x,y,z,w) S(x-z, x+z, w)*S(y+z, y-z, w)\r\n#define sat(x) clamp(x,0.,1.)\r\n#define SIN(x) sin(x)*.5+.5\r\n\r\nfloat smth = .6;\r\nfloat hr = 1.;\t\t\t\t\t// radii of atoms\r\nfloat nr = 2.264;\r\nfloat cr = 2.674;\r\nfloat or = 2.102;\r\nfloat pr = 3.453;\r\n\r\nvec3 hc = vec3(1.);\t\t\t\t// colors of atoms\r\nvec3 nc = vec3(.1, .1, 1.);\r\nvec3 cc = vec3(.1);\r\nvec3 oc = vec3(1., .1, .1);\r\nvec3 pc = vec3(1., .75, .3);\r\n\r\nconst vec3 lf=vec3(1., 0., 0.);\r\nconst vec3 up=vec3(0., 1., 0.);\r\nconst vec3 fw=vec3(0., 0., 1.);\r\n\r\nconst float halfpi = 1.570796326794896619;\r\nconst float pi = 3.141592653589793238;\r\nconst float twopi = 6.283185307179586;\r\n\r\n\r\nvec3 bg = vec3(.1, .5, 1.); // global background color\r\n\r\nfloat L2(vec3 p) {return dot(p, p);}\r\nfloat L2(vec2 p) {return dot(p, p);}\r\n\r\nfloat N1( float x ) { return fract(sin(x)*5346.1764); }\r\nfloat N2(float x, float y) { return N1(x + y*23414.324); }\r\n\r\nfloat N3(vec3 p) {\r\n    p  = fract( p*0.3183099+.1 );\r\n\tp *= 17.0;\r\n    return fract( p.x*p.y*p.z*(p.x+p.y+p.z) );\r\n}\r\n\r\nvec3 N31(float p) {\r\n    //  3 out, 1 in... DAVE HOSKINS\r\n   vec3 p3 = fract(vec3(p) * vec3(.1031,.11369,.13787));\r\n   p3 += dot(p3, p3.yzx + 19.19);\r\n   return fract(vec3((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y, (p3.y+p3.z)*p3.x));\r\n}\r\n\r\n\r\nstruct ray {\r\n    vec3 o;\r\n    vec3 d;\r\n};\r\n\r\nstruct camera {\r\n    vec3 p;\t\t\t// the position of the camera\r\n    vec3 forward;\t// the camera forward vector\r\n    vec3 left;\t\t// the camera left vector\r\n    vec3 up;\t\t// the camera up vector\r\n\t\r\n    vec3 center;\t// the center of the screen, in world coords\r\n    vec3 i;\t\t\t// where the current ray intersects the screen, in world coords\r\n    ray ray;\t\t// the current ray: from cam pos, through current uv projected on screen\r\n    vec3 lookAt;\t// the lookat point\r\n    float zoom;\t\t// the zoom factor\r\n};\r\n\r\nstruct de {\r\n    // data type used to pass the various bits of information used to shade a de object\r\n\tfloat d;\t// distance to the object\r\n    float m; \t// material\r\n    vec3 col;\r\n    \r\n    vec3 id;\r\n    float spread;\r\n    // shading parameters\r\n    vec3 pos;\t\t// the world-space coordinate of the fragment\r\n    vec3 nor;\t\t// the world-space normal of the fragment\r\n    vec3 rd;\r\n    float fresnel;\t\r\n};\r\n    \r\nstruct rc {\r\n    // data type used to handle a repeated coordinate\r\n\tvec3 id;\t// holds the floor'ed coordinate of each cell. Used to identify the cell.\r\n    vec3 h;\t\t// half of the size of the cell\r\n    vec3 p;\t\t// the repeated coordinate\r\n    vec3 c;\t\t// the center of the cell, world coordinates\r\n};\r\n    \r\nrc Repeat(vec3 pos, vec3 size) {\r\n\trc o;\r\n    o.h = size*.5;\t\t\t\t\t\r\n    o.id = floor(pos/size);\t\t\t// used to give a unique id to each cell\r\n    o.p = mod(pos, size)-o.h;\r\n    o.c = o.id*size+o.h;\r\n    \r\n    return o;\r\n}\r\n    \r\ncamera cam;\r\n\r\n\r\nvoid CameraSetup(vec2 uv, vec3 position, vec3 lookAt, float zoom) {\r\n\t\r\n    cam.p = position;\r\n    cam.lookAt = lookAt;\r\n    cam.forward = normalize(cam.lookAt-cam.p);\r\n    cam.left = cross(up, cam.forward);\r\n    cam.up = cross(cam.forward, cam.left);\r\n    cam.zoom = zoom;\r\n    \r\n    cam.center = cam.p+cam.forward*cam.zoom;\r\n    cam.i = cam.center+cam.left*uv.x+cam.up*uv.y;\r\n    \r\n    cam.ray.o = cam.p;\t\t\t\t\t\t// ray origin = camera position\r\n    cam.ray.d = normalize(cam.i-cam.p);\t// ray direction is the vector from the cam pos through the point on the imaginary screen\r\n}\r\n\r\nfloat remap01(float a, float b, float t) { return (t-a)/(b-a); }\r\n\r\n\r\n// DE functions from IQ\r\n// https://www.shadertoy.com/view/Xds3zN\r\n\r\nfloat smin( float a, float b, float k )\r\n{\r\n    float h = clamp( 0.5+0.5*(b-a)/k, 0.0, 1.0 );\r\n    return mix( b, a, h ) - k*h*(1.0-h);\r\n}\r\n\r\nvec2 smin2( float a, float b, float k )\r\n{\r\n    float h = clamp( 0.5+0.5*(b-a)/k, 0.0, 1.0 );\r\n    return vec2(mix( b, a, h ) - k*h*(1.0-h), h);\r\n}\r\n\r\nfloat smax( float a, float b, float k )\r\n{\r\n\tfloat h = clamp( 0.5 + 0.5*(b-a)/k, 0.0, 1.0 );\r\n\treturn mix( a, b, h ) + k*h*(1.0-h);\r\n}\r\n\r\nfloat sdSphere( vec3 p, vec3 pos, float s ) { return (length(p-pos)-s)*.9; }\r\n\r\n\r\nvec3 background(vec3 r) {\r\n\tfloat y = pi*0.5-acos(r.y);  \t\t// from -1/2pi to 1/2pi\t\t\r\n    \r\n    return bg*(1.+y);\r\n}\r\n\r\nvec4 Adenine(vec3 p, float getColor) {\r\n   #ifdef USE_BOUNDING_VOLUMES\r\n    float b = sdSphere(p, vec3(29.52, 6.64, 3.04), 11.019);\r\n    \r\n    if(b>0.)\r\n        return vec4(bg, b+SHOW_BOUNDING_VOLUMES);\r\n    else {\r\n #endif\r\n    \r\n    float h =  sdSphere(p, vec3(22.44, 13.63, 3.04), hr);\r\n    h = min(h, sdSphere(p, vec3(21.93, 0.28, 3.04), hr));\r\n    h = min(h, sdSphere(p, vec3(26.08, -1.19, 3.04), hr));\r\n    h = min(h, sdSphere(p, vec3(39.04, 3.98, 3.04), hr));\r\n    \r\n    float n =  sdSphere(p, vec3(23.18, 7.49, 3.04), nr);\r\n    n = min(n, sdSphere(p, vec3(28.39, 11.95, 3.04), nr));\r\n    n = min(n, sdSphere(p, vec3(24.43, 0.75, 3.04), nr));\r\n    n = min(n, sdSphere(p, vec3(32.79, 2.79, 3.04), nr));\r\n    n = min(n, sdSphere(p, vec3(34.93, 8.83, 3.04), nr));\r\n    \r\n    float c =  sdSphere(p, vec3(24.50, 11.22, 3.04), cr);\r\n    c = min(c, sdSphere(p, vec3(25.75, 4.47, 3.04), cr));\r\n    c = min(c, sdSphere(p, vec3(29.65, 5.2, 3.04), cr));\r\n    c = min(c, sdSphere(p, vec3(30.97, 8.93, 3.04), cr));\r\n    c = min(c, sdSphere(p, vec3(36.06, 5.03, 3.04), cr));\r\n    \r\n    \r\n        vec3 col = vec3(0.);\r\n        float d;\r\n\r\n        if(getColor!=0.) {\r\n            vec2 i = smin2(h, n, smth);\r\n            col = mix(nc, hc, i.y);        \r\n\r\n            i = smin2(i.x, c, smth);\r\n            col = mix(cc, col, i.y);\r\n\r\n            d = i.x;\r\n        } else\r\n            d = smin(c, smin(h, n, smth), smth);\r\n\r\n        return vec4(col, d);\r\n    #ifdef USE_BOUNDING_VOLUMES\r\n    }\r\n    #endif\r\n}\r\n\r\nvec4 Thymine(vec3 p, float getColor) {\r\n\r\n #ifdef USE_BOUNDING_VOLUMES\r\n    float b = sdSphere(p, vec3(12.96, 5.55, 3.04), 10.466);\r\n    \r\n    if(b>0.)\r\n        return vec4(bg, b+SHOW_BOUNDING_VOLUMES);\r\n    else {\r\n #endif\r\n    float o =  sdSphere(p, vec3(18.171, -.019, 3.04), or);\r\n    o = min(o, sdSphere(p, vec3(15.369, 13.419, 3.04), or));\r\n    \r\n    float h =  sdSphere(p, vec3(19.253, 7.218, 3.04), hr);\r\n    h = min(h, sdSphere(p, vec3(12.54, -3.449, 4.534), hr));\r\n    h = min(h, sdSphere(p, vec3(7.625, -1.831, 4.533), hr));\r\n    h = min(h, sdSphere(p, vec3(10.083, -2.64, 0.052), hr));\r\n    \r\n    float n =  sdSphere(p, vec3(16.77, 6.7, 3.04), nr);\r\n    n = min(n, sdSphere(p, vec3(10.251, 8.846, 3.04), nr));\r\n    \r\n    float c =  sdSphere(p, vec3(10.541, -1.636, 3.04), cr);\r\n    c = min(c, sdSphere(p, vec3(11.652, 2.127, 3.04), cr));\r\n    c = min(c, sdSphere(p, vec3(15.531, 2.936, 3.04), cr));\r\n    c = min(c, sdSphere(p, vec3(9.012, 5.082, 3.04), cr));\r\n    c = min(c, sdSphere(p, vec3(14.13, 9.655, 3.04), cr));\r\n    \r\n\r\n        vec3 col = vec3(0.);\r\n        float d;\r\n\r\n        if(getColor!=0.) {\r\n            vec2 i = smin2(h, n, smth);\r\n            col = mix(nc, hc, i.y);        \r\n\r\n            i = smin2(i.x, c, smth);\r\n            col = mix(cc, col, i.y);\r\n\r\n            i = smin2(i.x, o, smth);\r\n            col = mix(oc, col, i.y);\r\n\r\n            d = i.x;\r\n        } else\r\n            d = smin(o, smin(c, smin(h, n, smth), smth), smth);\r\n\r\n        return vec4(col, d);\r\n    #ifdef USE_BOUNDING_VOLUMES\r\n    }\r\n    #endif\r\n}\r\n\r\n\r\n\r\n\r\nvec4 Cytosine(vec3 p, float getColor) {\r\n\r\n #ifdef USE_BOUNDING_VOLUMES\r\n    float b = sdSphere(p, vec3(14.556, 5.484, 3.227), 10.060);\r\n    if(b>0.)\r\n        return vec4(bg, b+SHOW_BOUNDING_VOLUMES);\r\n    else {\r\n #endif\r\n        \r\n        float c = sdSphere(p, vec3(11.689, 1.946, 3.067), cr);\r\n        c = min(c, sdSphere(p, vec3(15.577, 2.755, 3.067), cr));\r\n        c = min(c, sdSphere(p, vec3(14.176, 9.474, 3.067), cr));\r\n        c = min(c, sdSphere(p, vec3(9.058, 4.9, 3.067), cr));\r\n\r\n        float n = sdSphere(p, vec3(18.412, 0.342, 3.067), nr);\r\n        n = min(n, sdSphere(p, vec3(16.816, 6.519, 3.067), nr));\r\n        n = min(n, sdSphere(p, vec3(10.297, 8.665, 3.067), nr));\r\n\r\n        float h = sdSphere(p, vec3(6.526, 3.015, 3.067), hr);\r\n        h = min(h, sdSphere(p, vec3(10.61, -1.045, 3.067), hr));\r\n        h = min(h, sdSphere(p, vec3(18.805, -2.297, 3.067), hr));\r\n        h = min(h, sdSphere(p, vec3(20.95, 0.584, 3.067), hr));\r\n\r\n\r\n        float o = sdSphere(p, vec3(15.415, 13.237, 3.067), or);\r\n\r\n        vec3 col = vec3(1.);\r\n\r\n        float d;\r\n        \r\n        if(getColor!=0.) {\r\n            vec2 i = smin2(c, n, smth);\r\n            col = mix(nc, cc, i.y);        \r\n\r\n            i = smin2(i.x, h, smth);\r\n            col = mix(hc, col, i.y);\r\n\r\n            i = smin2(i.x, o, smth);\r\n            col = mix(oc, col, i.y);\r\n            \r\n            d = i.x;\r\n        } else\r\n            d = smin(o, smin(h, smin(c, n, smth), smth), smth);\r\n        \r\n        return vec4(col, d);\r\n    #ifdef USE_BOUNDING_VOLUMES\r\n    }\r\n    #endif\r\n}\r\n\r\nvec4 Guanine(vec3 p, float getColor) {\r\n\r\n #ifdef USE_BOUNDING_VOLUMES\r\n    float b = sdSphere(p, vec3(29.389, 8.944, 3.227), 12.067);\r\n    \r\n    if(b>0.)\r\n        return vec4(bg, b+SHOW_BOUNDING_VOLUMES);\r\n    else {\r\n #endif\r\n        \r\n        float c = sdSphere(p, vec3(24.642, 11.602, 3.067), cr);\r\n        c = min(c, sdSphere(p, vec3(31.111, 9.311, 3.067), cr));\r\n        c = min(c, sdSphere(p, vec3(29.79, 5.576, 3.067), cr));\r\n        c = min(c, sdSphere(p, vec3(25.893, 4.854, 3.067), cr));\r\n        c = min(c, sdSphere(p, vec3(36.19, 5.409, 3.067), cr));\r\n\r\n        float n = sdSphere(p, vec3(22.56, 14.31, 3.067), nr);\r\n        n = min(n, sdSphere(p, vec3(23.32, 7.867, 3.067), nr));\r\n        n = min(n, sdSphere(p, vec3(28.538, 12.325, 3.067), nr));\r\n        n = min(n, sdSphere(p, vec3(32.934, 3.164, 3.067), nr));\r\n        n = min(n, sdSphere(p, vec3(35.07, 9.209, 3.067), nr));\r\n\r\n        float h = sdSphere(p, vec3(20.044, 14.723, 3.04), hr);\r\n        h = min(h, sdSphere(p, vec3(22.852, 16.965, 3.04), hr));\r\n        h = min(h, sdSphere(p, vec3(20.856, 7.404, 3.067), hr));\r\n        h = min(h, sdSphere(p, vec3(39.187, 4.352, 3.067), hr));\r\n\r\n\r\n        float o = sdSphere(p, vec3(24.7, 1.893, 3.067), or);\r\n\r\n        vec3 col = vec3(1.);\r\n        \r\n        float d;\r\n        \r\n        if(getColor!=0.) {\r\n            vec2 i = smin2(c, n, smth);\r\n            col = mix(nc, cc, i.y);        \r\n\r\n            i = smin2(i.x, h, smth);\r\n            col = mix(hc, col, i.y);\r\n\r\n            i = smin2(i.x, o, smth);\r\n            col = mix(oc, col, i.y);\r\n            \r\n            d = i.x;\r\n        } else\r\n            d = smin(o, smin(h, smin(c, n, smth), smth), smth);\r\n        \r\n        return vec4(col, d);\r\n    #ifdef USE_BOUNDING_VOLUMES\r\n    }\r\n    #endif\r\n}\r\n\r\n\r\nvec4 Backbone(vec3 p, float getColor) {\r\n\r\n #ifdef USE_BOUNDING_VOLUMES\r\n    float b = sdSphere(p, vec3(0., 7.03, 0.), 10.572);   \r\n    if(b>0.)\r\n        return vec4(bg, b+SHOW_BOUNDING_VOLUMES);\r\n    else {\r\n #endif       \r\n        float c = sdSphere(p, vec3(1.391, 8.476, -0.708), cr);\r\n        c = min(c, sdSphere(p, vec3(5.173, 9.661, -0.708), cr));\r\n        c = min(c, sdSphere(p, vec3(6.342, 10.028, 3.061), cr));\r\n        c = min(c, sdSphere(p, vec3(0.222, 8.109, 3.061), cr));\r\n        c = min(c, sdSphere(p, vec3(0.658, 4.4, 4.8871), cr));\r\n\r\n        float h = sdSphere(p, vec3(-5.853, 0., 2.213), hr);\r\n        h = min(h, sdSphere(p, vec3(5.4512, 12.437, -2.216), hr));\r\n        h = min(h, sdSphere(p, vec3(6.986, 7.541, -2.216), hr));\r\n        h = min(h, sdSphere(p, vec3(-1.726, 10.517, 4.39), hr));\r\n        h = min(h, sdSphere(p, vec3(3.203, 2.519, 4.691), hr));\r\n        h = min(h, sdSphere(p, vec3(-1.619, 3.162, 3.063), hr));\r\n\r\n        float o = sdSphere(p, vec3(-4.918, 1.599, 0.344), or);\r\n        o = min(o, sdSphere(p, vec3(-1.471, 0.995, -5.1), or));\r\n        o = min(o, sdSphere(p, vec3(-0.836, 6.288, -1.438), or));\r\n        o = min(o, sdSphere(p, vec3(3.282, 9.068, 5.391), or));\r\n        o = min(o, sdSphere(p, vec3(-6.286, 5.299, -4.775), or));\r\n        \r\n        float ph = sdSphere(p, vec3(-3.377, 3.544, -2.742), pr);\r\n\t\t\r\n        #ifdef STRANDS\r\n        o = min(o, sdSphere(p, vec3(-6.286, 5.299, 6.558), or));\r\n        ph = min(ph, sdSphere(p, vec3(-3.377, 3.544, 8.592), pr)); // extra so it tiles better\r\n        #endif\r\n        \r\n        vec3 col = vec3(1.);\r\n\t\tfloat d;\r\n        \r\n        if(getColor!=0.) {\r\n            vec2 i = smin2(c, h, smth);\r\n            col = mix(hc, cc, i.y);        \r\n\r\n            i = smin2(i.x, o, smth);\r\n            col = mix(oc, col, i.y);\r\n\r\n            i = smin2(i.x, ph, smth);\r\n            col = mix(pc, col, i.y);\r\n            \r\n            d = i.x;\r\n        } else\r\n            d = smin(ph, smin(o, smin(c, h, smth), smth), smth);\r\n        \r\n        return vec4(col, d);\r\n  #ifdef USE_BOUNDING_VOLUMES      \r\n    }\r\n  #endif  \r\n}\r\n\r\n\r\n\r\n\r\nvec4 map( vec3 p, vec3 id, float spread, float getColor ) {\r\n    \r\n    p.z += 2.4;// offset so it tiles better\r\n    vec4 col;\r\n    \r\n    vec3 bp = p;    \r\n    bp.x = 22.5-bp.x;\r\n    float side = sign(bp.x);\r\n    bp.x = 22.5-abs(bp.x)+spread;\r\n    bp.z = bp.z*side - min(0., side)*5.;\r\n    vec4 b = Backbone(bp, getColor);\r\n    \r\n    vec4 c = vec4(1000.);\r\n    vec4 g = vec4(1000.);\r\n    vec3 cp = p;\r\n    vec3 gp = p;\r\n    \r\n    float n = N3(id);\r\n    \r\n    if(n<.5) {\r\n    \tcp.xz = -cp.xz + vec2(46., 6.);\r\n    \tgp.xz = -gp.xz + vec2(46., 6.);\r\n    }\r\n    cp.x += spread;\r\n    gp.x -= spread;\r\n    \r\n    if(mod(floor(n*4.), 2.)==0.) {\r\n    \tc = Cytosine(cp, getColor);\r\n    \tg = Guanine(gp, getColor);\r\n    } else {    \r\n    \tg = Adenine(gp, getColor);\r\n    \tc = Thymine(cp, getColor);\r\n    }\r\n  \r\n    col.a = min(b.a, min(c.a, g.a));\r\n  \r\n    if(getColor!=0.) {\r\n        if(col.a==b.a)\r\n            col.rgb = b.rgb;\t\r\n        else if(col.a==c.a)\r\n            col.rgb = c.rgb;\r\n        else\r\n            col.rgb = g.rgb;\r\n    }\r\n    \r\n    return col;\r\n}\r\n\r\n\r\nde castRay( ray r ) {\r\n    float t = iTime*.3;\r\n    \r\n    de o;\r\n    o.m = -1.0;\r\n    vec2 cbd = vec2(MIN_DISTANCE, MAX_DISTANCE);\r\n    vec2 bbd = cbd;\r\n    \r\n    vec4 col_pos;\r\n    \r\n    vec3 p = vec3(0.);\r\n    \r\n    float d = MIN_DISTANCE;\r\n    rc q;\r\n    vec3 center = vec3(19.12, 7.09, 3.09);\r\n    float spread;\r\n    \r\n    vec3 grid = vec3(180., 180., 11.331);\r\n    \r\n    #ifdef STRANDS\r\n    for( int i=0; i<MAX_INT_STEPS; i++ ) {\r\n        p = r.o+r.d*d;\r\n        float oz = p.z;\r\n        \r\n        q = Repeat(p, grid);\r\n        float sd = length((q.c.xy-center.xy));\r\n            \r\n        p.z += t*200.*S(800., 100., sd);\r\n        float n = N2(q.id.x, q.id.y);\r\n        \r\n        p.y += sin(n*twopi + p.z*.003+t)*50.*S(300., 500., sd);\r\n        \r\n        q = Repeat(p, grid);\r\n\t\t\r\n       \r\n        float z = oz*.05;\r\n        float z2 = smax(0., abs(oz*.03)-6., 2.);\r\n        float s = sin(z2);\r\n        float c = cos(z2);\r\n        \r\n        oz *= .012;\r\n        spread = max(0., 6.-oz*oz);\r\n        spread *= spread;\r\n        spread *= S(250., 1., length(q.id.xy*grid.xy+q.h.xy-r.o.xy));\r\n            \r\n        vec3 rC = ((2.*step(0., r.d)-1.)*q.h-q.p)/r.d;\t// ray to cell boundary\r\n        float dC = min(min(rC.x, rC.y), rC.z)+.01;\t\t// distance to cell just past boundary\r\n        \r\n       \r\n        \r\n        float dS = MAX_DISTANCE;\r\n        \r\n        #ifdef OPTIMIZED\r\n        vec2 bla = q.p.xy-center.xy;\r\n        if(dot(bla, r.d.xy)>0. && length(bla)>50.)\t// if we are stepping away from the strand and we are already far enough\r\n        \tdC = min(rC.x, rC.y)+1.;\t\t\t\t// then we won't hit this strand anymore and we can skip to the next one\r\n        else {\r\n            #endif \r\n             q.p-=center;\r\n        mat2 m = mat2(c, -s, s, c);\r\n        q.p.xy *= m;\r\n        q.p+=center;\r\n            \r\n        \tdC = rC.z +.01;\r\n            dS = map( q.p, q.id, spread, 0. ).a;\r\n        #ifdef OPTIMIZED\r\n        } \r\n        #endif\r\n        \r\n            \r\n        if( dS<RAY_PRECISION || d>MAX_DISTANCE ) break;      \r\n        \r\n        d+=min(dS, dC);\t// move to distance to next cell or surface, whichever is closest\r\n    }\r\n    \r\n    #else\r\n\tq.id = vec3(0.);\r\n    spread = 0.;\r\n     for( int i=0; i<MAX_INT_STEPS; i++ ) {\r\n        p = r.o+r.d*d;\r\n         \r\n        col_pos = map( p, vec3(0.), 0., 0. );\r\n        float dS = col_pos.a;\r\n        if( dS<RAY_PRECISION || d>MAX_DISTANCE ) break;      \r\n       \r\n        d+=dS;\r\n    }\r\n    #endif\r\n    \r\n    if(d<MAX_DISTANCE) { \r\n        o.m=1.;\r\n        o.d = d;\r\n        o.id = q.id;\r\n        o.spread = spread;\r\n        #ifdef STRANDS\r\n        o.pos = q.p;\r\n        #else\r\n        o.pos = p;\r\n        #endif\r\n        \r\n        o.d = d;\r\n    }\r\n    return o;\r\n}\r\n\r\nvec4 nmap( de o, vec3 offs ) {\r\n   \r\n    return map(o.pos+offs, o.id, o.spread, 0.);\r\n}\r\n\r\nde GetSurfaceProps( de o )\r\n{\r\n\tvec3 eps = vec3( 0.001, 0.0, 0.0 );\r\n\tvec3 p = o.pos-eps.yyx;\r\n    vec4 c = map(p, o.id, o.spread, 1.);\r\n    o.col = c.rgb;\r\n    \r\n    vec3 nor = vec3(\r\n\t    nmap(o, eps.xyy).a - nmap(o, -eps.xyy).a,\r\n\t    nmap(o, eps.yxy).a - nmap(o, -eps.yxy).a,\r\n\t    nmap(o, eps.yyx).a - c.a );\r\n\to.nor = normalize(nor);\r\n    \r\n    return o;\r\n}\r\n\r\nvec3 AtomMat(de o, vec3 rd) {\r\n    o = GetSurfaceProps( o );\r\n    \r\n    vec3 R = reflect(cam.ray.d, o.nor);\r\n    vec3 ref = background(R);\r\n    \r\n    float dif = dot(up, o.nor)*.5+.5;\r\n    dif = mix(.3, 1., dif);\r\n    \r\n\tvec3 col = o.col*dif;\r\n    \r\n    float t = iTime*50.+length(o.col)*10.;\r\n\r\n    float fresnel = 1.-sat(dot(o.nor, -rd));\r\n    fresnel = pow(fresnel, .5);\r\n          \r\n    \r\n    \r\n    #ifdef STRANDS\r\n    float up = dot(rd, vec3(0., 1., 0.));\r\n    col = mix(col, ref, fresnel*.5*S(.8, .0, up));\r\n    col *= S(.9, .2, up);\r\n    #else  \r\n    col = mix(col, ref, fresnel*.5);\r\n    #endif\r\n    \r\n    col = mix(col, bg, S(0., 1000., o.d));\r\n\r\n    return col;\r\n}\r\n\r\nvec3 render( vec2 uv, ray camRay, float depth ) {\r\n    // outputs a color\r\n    \r\n    bg = background(cam.ray.d);\r\n    \r\n    vec3 col = bg;\r\n    de o = castRay(camRay);\r\n   \r\n    if(o.m>0.) {\r\n        col = AtomMat(o, cam.ray.d);\r\n    }\r\n    \r\n    return col;\r\n}\r\n\r\nvoid main()\r\n{\r\nvec2 uv = tempUv;\r\n    uv -= .5;\r\n    uv.y *= iResolution.y/iResolution.x;\r\n    vec2 m = iResolution.xy / 2.0;\r\n    \r\n\tfloat t = iTime;\r\n    \r\n   if(m.x==0.&&m.y==0.) m = vec2(.5, .5);\r\n    \r\n    \r\n    #ifdef STRANDS\r\n    float camDist = -4.;\r\n    \r\n    t = t * .2;\r\n    \r\n    float y = t*.5;;\r\n    float x = t;\r\n    vec3 camPos = vec3(-60.+sin(t)*180., -80.+sin(y)*250., 0.);\r\n    \r\n    m -= .5;\r\n    vec3 pos = vec3(-(cos(x)+m.x)*3., -(cos(y)+m.y)*3., camDist);//*rotX;\r\n    #else\r\n    \r\n    float turn = (.1-m.x)*twopi+t*.0;\r\n    float s = sin(turn);\r\n    float c = cos(turn);\r\n    mat3 rotX = mat3(c,  0., s, 0., 1., 0., s,  0., -c);\r\n    \r\n    float camDist = -100.;\r\n    vec3 camPos = vec3(19., 0., 0.);\r\n    \r\n    vec3 pos = vec3(0., INVERTMOUSE*camDist*cos((m.y)*pi), camDist)*rotX;\r\n    #endif\r\n    \t\r\n    CameraSetup(uv, camPos+pos, camPos, 1.);\r\n    \r\n    vec3 col = render(uv, cam.ray, 0.);\r\n   \r\n    fragColor = vec4(col, 1.);\r\n}";
            string fragmentShaderSource = @"
                #version 330 core

                uniform float iTime;
                uniform vec3 iResolution;

                in vec2 tempUv;                

                out vec4 fragColor;

// ""The Drive Home"" by Martijn Steinrucken aka BigWings - 2017
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// Email:countfrolic@gmail.com Twitter:@The_ArtOfCode
//
// I was looking for something 3d, that can be made just with a point-line distance function.
// Then I saw the cover graphic of the song I'm using here on soundcloud, which is a bokeh traffic
// shot which is a perfect for for what I was looking for.
//
// It took me a while to get to a satisfying rain effect. Most other people use a render buffer for
// this so that is how I started. In the end though, I got a better effect without. Uncomment the
// DROP_DEBUG define to get a better idea of what is going on.
//
// If you are watching this on a weaker device, you can uncomment the HIGH_QUALITY define
//
// Music:
// Mr. Bill - Cheyah (Zefora's digital rain remix) 
// https://soundcloud.com/zefora/cheyah
//
// Video can be found here:
// https://www.youtube.com/watch?v=WrxZ4AZPdOQ
//
// Making of tutorial:
// https://www.youtube.com/watch?v=eKtsY7hYTPg
//

#define S(x, y, z) smoothstep(x, y, z)
#define B(a, b, edge, t) S(a-edge, a+edge, t)*S(b+edge, b-edge, t)
#define sat(x) clamp(x,0.,1.)

#define streetLightCol vec3(1., .7, .3)
#define headLightCol vec3(.8, .8, 1.)
#define tailLightCol vec3(1., .1, .1)

#define HIGH_QUALITY
#define CAM_SHAKE 1.
#define LANE_BIAS .5
#define RAIN
//#define DROP_DEBUG

vec3 ro, rd;

float N(float t) {
	return fract(sin(t*10234.324)*123423.23512);
}
vec3 N31(float p) {
    //  3 out, 1 in... DAVE HOSKINS
   vec3 p3 = fract(vec3(p) * vec3(.1031,.11369,.13787));
   p3 += dot(p3, p3.yzx + 19.19);
   return fract(vec3((p3.x + p3.y)*p3.z, (p3.x+p3.z)*p3.y, (p3.y+p3.z)*p3.x));
}
float N2(vec2 p)
{	// Dave Hoskins - https://www.shadertoy.com/view/4djSRW
	vec3 p3  = fract(vec3(p.xyx) * vec3(443.897, 441.423, 437.195));
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}


float DistLine(vec3 ro, vec3 rd, vec3 p) {
	return length(cross(p-ro, rd));
}
 
vec3 ClosestPoint(vec3 ro, vec3 rd, vec3 p) {
    // returns the closest point on ray r to point p
    return ro + max(0., dot(p-ro, rd))*rd;
}

float Remap(float a, float b, float c, float d, float t) {
	return ((t-a)/(b-a))*(d-c)+c;
}

float BokehMask(vec3 ro, vec3 rd, vec3 p, float size, float blur) {
	float d = DistLine(ro, rd, p);
    float m = S(size, size*(1.-blur), d);
    
    #ifdef HIGH_QUALITY
    m *= mix(.7, 1., S(.8*size, size, d));
    #endif
    
    return m;
}



float SawTooth(float t) {
    return cos(t+cos(t))+sin(2.*t)*.2+sin(4.*t)*.02;
}

float DeltaSawTooth(float t) {
    return 0.4*cos(2.*t)+0.08*cos(4.*t) - (1.-sin(t))*sin(t+cos(t));
}  

vec2 GetDrops(vec2 uv, float seed, float m) {
    
    float t = iTime+m*30.;
    vec2 o = vec2(0.);
    
    #ifndef DROP_DEBUG
    uv.y += t*.05;
    #endif
    
    uv *= vec2(10., 2.5)*2.;
    vec2 id = floor(uv);
    vec3 n = N31(id.x + (id.y+seed)*546.3524);
    vec2 bd = fract(uv);
    
    vec2 uv2 = bd;
    
    bd -= .5;
    
    bd.y*=4.;
    
    bd.x += (n.x-.5)*.6;
    
    t += n.z * 6.28;
    float slide = SawTooth(t);
    
    float ts = 1.5;
    vec2 trailPos = vec2(bd.x*ts, (fract(bd.y*ts*2.-t*2.)-.5)*.5);
    
    bd.y += slide*2.;								// make drops slide down
    
    #ifdef HIGH_QUALITY
    float dropShape = bd.x*bd.x;
    dropShape *= DeltaSawTooth(t);
    bd.y += dropShape;								// change shape of drop when it is falling
    #endif
    
    float d = length(bd);							// distance to main drop
    
    float trailMask = S(-.2, .2, bd.y);				// mask out drops that are below the main
    trailMask *= bd.y;								// fade dropsize
    float td = length(trailPos*max(.5, trailMask));	// distance to trail drops
    
    float mainDrop = S(.2, .1, d);
    float dropTrail = S(.1, .02, td);
    
    dropTrail *= trailMask;
    o = mix(bd*mainDrop, trailPos, dropTrail);		// mix main drop and drop trail
    
    #ifdef DROP_DEBUG
    if(uv2.x<.02 || uv2.y<.01) o = vec2(1.);
    #endif
    
    return o;
}

void CameraSetup(vec2 uv, vec3 pos, vec3 lookat, float zoom, float m) {
	ro = pos;
    vec3 f = normalize(lookat-ro);
    vec3 r = cross(vec3(0., 1., 0.), f);
    vec3 u = cross(f, r);
    float t = iTime;
    
    vec2 offs = vec2(0.);
    #ifdef RAIN
    vec2 dropUv = uv; 
    
    #ifdef HIGH_QUALITY
    float x = (sin(t*.1)*.5+.5)*.5;
    x = -x*x;
    float s = sin(x);
    float c = cos(x);
    
    mat2 rot = mat2(c, -s, s, c);
   
    #ifndef DROP_DEBUG
    dropUv = uv*rot;
    dropUv.x += -sin(t*.1)*.5;
    #endif
    #endif
    
    offs = GetDrops(dropUv, 1., m);
    
    #ifndef DROP_DEBUG
    offs += GetDrops(dropUv*1.4, 10., m);
    #ifdef HIGH_QUALITY
    offs += GetDrops(dropUv*2.4, 25., m);
    //offs += GetDrops(dropUv*3.4, 11.);
    //offs += GetDrops(dropUv*3., 2.);
    #endif
    
    float ripple = sin(t+uv.y*3.1415*30.+uv.x*124.)*.5+.5;
    ripple *= .005;
    offs += vec2(ripple*ripple, ripple);
    #endif
    #endif
    vec3 center = ro + f*zoom;
    vec3 i = center + (uv.x-offs.x)*r + (uv.y-offs.y)*u;
    
    rd = normalize(i-ro);
}

vec3 HeadLights(float i, float t) {
    float z = fract(-t*2.+i);
    vec3 p = vec3(-.3, .1, z*40.);
    float d = length(p-ro);
    
    float size = mix(.03, .05, S(.02, .07, z))*d;
    float m = 0.;
    float blur = .1;
    m += BokehMask(ro, rd, p-vec3(.08, 0., 0.), size, blur);
    m += BokehMask(ro, rd, p+vec3(.08, 0., 0.), size, blur);
    
    #ifdef HIGH_QUALITY
    m += BokehMask(ro, rd, p+vec3(.1, 0., 0.), size, blur);
    m += BokehMask(ro, rd, p-vec3(.1, 0., 0.), size, blur);
    #endif
    
    float distFade = max(.01, pow(1.-z, 9.));
    
    blur = .8;
    size *= 2.5;
    float r = 0.;
    r += BokehMask(ro, rd, p+vec3(-.09, -.2, 0.), size, blur);
    r += BokehMask(ro, rd, p+vec3(.09, -.2, 0.), size, blur);
    r *= distFade*distFade;
    
    return headLightCol*(m+r)*distFade;
}


vec3 TailLights(float i, float t) {
    t = t*1.5+i;
    
    float id = floor(t)+i;
    vec3 n = N31(id);
    
    float laneId = S(LANE_BIAS, LANE_BIAS+.01, n.y);
    
    float ft = fract(t);
    
    float z = 3.-ft*3.;						// distance ahead
    
    laneId *= S(.2, 1.5, z);				// get out of the way!
    float lane = mix(.6, .3, laneId);
    vec3 p = vec3(lane, .1, z);
    float d = length(p-ro);
    
    float size = .05*d;
    float blur = .1;
    float m = BokehMask(ro, rd, p-vec3(.08, 0., 0.), size, blur) +
    			BokehMask(ro, rd, p+vec3(.08, 0., 0.), size, blur);
    
    #ifdef HIGH_QUALITY
    float bs = n.z*3.;						// start braking at random distance		
    float brake = S(bs, bs+.01, z);
    brake *= S(bs+.01, bs, z-.5*n.y);		// n.y = random brake duration
    
    m += (BokehMask(ro, rd, p+vec3(.1, 0., 0.), size, blur) +
    	BokehMask(ro, rd, p-vec3(.1, 0., 0.), size, blur))*brake;
    #endif
    
    float refSize = size*2.5;
    m += BokehMask(ro, rd, p+vec3(-.09, -.2, 0.), refSize, .8);
    m += BokehMask(ro, rd, p+vec3(.09, -.2, 0.), refSize, .8);
    vec3 col = tailLightCol*m*ft; 
    
    float b = BokehMask(ro, rd, p+vec3(.12, 0., 0.), size, blur);
    b += BokehMask(ro, rd, p+vec3(.12, -.2, 0.), refSize, .8)*.2;
    
    vec3 blinker = vec3(1., .7, .2);
    blinker *= S(1.5, 1.4, z)*S(.2, .3, z);
    blinker *= sat(sin(t*200.)*100.);
    blinker *= laneId;
    col += blinker*b;
    
    return col;
}

vec3 StreetLights(float i, float t) {
	 float side = sign(rd.x);
    float offset = max(side, 0.)*(1./16.);
    float z = fract(i-t+offset); 
    vec3 p = vec3(2.*side, 2., z*60.);
    float d = length(p-ro);
	float blur = .1;
    vec3 rp = ClosestPoint(ro, rd, p);
    float distFade = Remap(1., .7, .1, 1.5, 1.-pow(1.-z,6.));
    distFade *= (1.-z);
    float m = BokehMask(ro, rd, p, .05*d, blur)*distFade;
    
    return m*streetLightCol;
}

vec3 EnvironmentLights(float i, float t) {
	float n = N(i+floor(t));
    
    float side = sign(rd.x);
    float offset = max(side, 0.)*(1./16.);
    float z = fract(i-t+offset+fract(n*234.));
    float n2 = fract(n*100.);
    vec3 p = vec3((3.+n)*side, n2*n2*n2*1., z*60.);
    float d = length(p-ro);
	float blur = .1;
    vec3 rp = ClosestPoint(ro, rd, p);
    float distFade = Remap(1., .7, .1, 1.5, 1.-pow(1.-z,6.));
    float m = BokehMask(ro, rd, p, .05*d, blur);
    m *= distFade*distFade*.5;
    
    m *= 1.-pow(sin(z*6.28*20.*n)*.5+.5, 20.);
    vec3 randomCol = vec3(fract(n*-34.5), fract(n*4572.), fract(n*1264.));
    vec3 col = mix(tailLightCol, streetLightCol, fract(n*-65.42));
    col = mix(col, randomCol, n);
    return m*col*.2;
}

void main()
{
	float t = iTime;
    vec3 col = vec3(0.);
    vec2 uv = tempUv; // 0 <> 1
    
    uv -= .5;
    uv.x *= iResolution.x/iResolution.y;
    
    vec2 mouse = vec2(0.5, 0.5);
    
    vec3 pos = vec3(.3, .15, 0.);
    
    float bt = t * 5.;
    float h1 = N(floor(bt));
    float h2 = N(floor(bt+1.));
    float bumps = mix(h1, h2, fract(bt))*.1;
    bumps = bumps*bumps*bumps*CAM_SHAKE;
    
    pos.y += bumps;
    float lookatY = pos.y+bumps;
    vec3 lookat = vec3(0.3, lookatY, 1.);
    vec3 lookat2 = vec3(0., lookatY, .7);
    lookat = mix(lookat, lookat2, sin(t*.1)*.5+.5);
    
    uv.y += bumps*4.;
    CameraSetup(uv, pos, lookat, 2., mouse.x);
   
    t *= .03;
    t += mouse.x;
    
    // fix for GLES devices by MacroMachines
    #ifdef GL_ES
	const float stp = 1./8.;
	#else
	float stp = 1./8.;
	#endif
    
    for(float i=0.; i<1.; i+=stp) {
       col += StreetLights(i, t);
    }
    
    for(float i=0.; i<1.; i+=stp) {
        float n = N(i+floor(t));
    	col += HeadLights(i+n*stp*.7, t);
    }
    
    #ifndef GL_ES
    #ifdef HIGH_QUALITY
    stp = 1./32.;
    #else
    stp = 1./16.;
    #endif
    #endif
    
    for(float i=0.; i<1.; i+=stp) {
       col += EnvironmentLights(i, t);
    }
    
    col += TailLights(0., t);
    col += TailLights(.5, t);
    
    col += sat(rd.y)*vec3(.6, .5, .9);
    
	fragColor = vec4(col, 1.);
}
            ";
            textureShader = new ShaderProgram(vertexShaderSource, fragmentShaderSource);

            textureVao = new VertexArray();
            textureVao.Bind();

            textureVbo = Buffer<float>.FromData(BufferTarget.ArrayBuffer, textureVertices.Length * sizeof(float), textureVertices, BufferUsageHint.StaticDraw);

            textureEbo = Buffer<uint>.FromData(BufferTarget.ElementArrayBuffer, rectIndices.Length * sizeof(uint), rectIndices, BufferUsageHint.StaticDraw);

            VertexAttribPointer[] attribs = {
                new(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0),
                new(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float))
            };

            textureVao.AttribPointers(textureVbo, attribs);
        }
    }
}
