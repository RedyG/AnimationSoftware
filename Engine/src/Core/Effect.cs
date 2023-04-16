using Engine.Attributes;
using Engine.Graphics;
using Engine.OpenGL;
using Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Engine.Core
{
    public abstract class Effect
    {
        // TODO: optimize if no need to ping pong
        // TDOO: make the API better
        public ParameterList Parameters { get; }
        public string Name { get; }

        protected abstract ParameterList InitParameters();
        protected virtual string InitName() => StringUtilities.UnPascalCase(GetType().Name);

        public Effect()
        {
            Parameters = InitParameters();
            Name = GetName(GetType());
        }

        public abstract RenderResult Render(RenderArgs args);

        public static List<Type> Effects { get; } = new();
        public static void RefreshEffects()
        {
            Effects.Clear();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Effect)))
                    {
                        Effects.Add(type);
                    }
                }
            }
        }


        private static Dictionary<Type, string> EffectNames { get; } = new();
        public static string GetName(Type type)
        {
            if (EffectNames.TryGetValue(type, out string? cachedName))
                return cachedName;

            CustomName? customName = type.GetCustomAttributes(typeof(CustomName)) as CustomName;
            string name;
            if (customName != null)
                name = customName.Name;
            else
                name = StringUtilities.UnPascalCase(type.Name);

            EffectNames.Add(type, name);
            return name;
        }
    }

    public struct RenderArgs
    {
        public Timecode Time { get; }
        public Surface SurfaceA;
        public Surface SurfaceB;
        public Layer Layer { get; }

        public RenderArgs(Timecode time, Layer layer, Surface surfaceA, Surface surfaceB)
        {
            Time = time;
            SurfaceA = surfaceA;
            SurfaceB = surfaceB;
            Layer = layer;
        }
    }

    public readonly struct RenderResult
    {
        public bool SwapSurfaces { get; }

        public RenderResult(bool swapSurfaces)
        {
            SwapSurfaces = swapSurfaces;
        }
    }
}
