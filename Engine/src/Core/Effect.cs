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
        Type _type;
        // TODO: optimize if no need to ping pong
        // TDOO: make the API better
        private ParameterList? _parameters;
        public ParameterList Parameters
        {
            get
            {
                if (_parameters == null)
                    _parameters = InitParameters();

                return _parameters;
            }
        }
        public EffectDescription Description => GetDescription(_type);

        protected abstract ParameterList InitParameters();

        public Effect()
        {
            _type = GetType();
        }

        public static Dictionary<string, Dictionary<string, Type>> Effects { get; } = new();

        // TODO: make this per assembly, so each time you load a plugin you add those types to Effects and you don't have to clear it all
        public static void RefreshEffects()
        {
            Effects.Clear();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Effect)))
                    {
                        var description = GetDescription(type);
                        if (Effects.TryGetValue(description.Category, out var category))
                        {
                            category[description.Name] = type;
                        }
                        else
                        {
                            Effects.Add(description.Category, new());
                            Effects[description.Category][description.Name] = type;
                        }
                    }
                }
            }
        }


        private static Dictionary<Type, EffectDescription> EffectDescriptions { get; } = new();
        public static EffectDescription GetDescription(Type type)
        {
            if (EffectDescriptions.TryGetValue(type, out EffectDescription cachedDesc))
                return cachedDesc;

            EffectDesc? descAttribute = type.GetCustomAttribute<EffectDesc>();
            var newDescription = new EffectDescription(
                descAttribute?.Name ?? StringUtilities.UnPascalCase(type.Name),
                descAttribute?.Category
            );

            EffectDescriptions.Add(type, newDescription);
            return newDescription;
        }

        // TODO: could crash
        public static Effect Create(Type type) => (Effect)Activator.CreateInstance(type)!;
    } 
    public abstract class VideoEffect : Effect
    {
        public abstract RenderResult Render(RenderArgs args);

        protected override abstract ParameterList InitParameters();
    }
    // TODO: Audio effects ( just audio in general )

    public readonly struct EffectDescription
    {
        public const string DefaultCategory = "Global";

        public string Name { get; init; }

        private readonly string? _category;
        public string Category
        {
            get => _category ?? DefaultCategory;
            init => _category = value;
        }

        public EffectDescription(string name, string? category = null)
        {
            Name = name;
            _category = category;
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
