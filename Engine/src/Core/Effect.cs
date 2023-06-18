using Engine.Attributes;
using Engine.Graphics;
using Engine.OpenGL;
using Engine.UI;
using Engine.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Engine.Core
{
    public abstract class Effect
    {
        // TODO: optimize if no need to ping pong
        // TDOO: make the API better
        private ParameterList? _parameters;

        [JsonIgnore]
        public ParameterList Parameters
        {
            get
            {
                if (_parameters == null)
                    SetupParameters();

                return _parameters!;
            }
        }

        [JsonIgnore]
        public EffectDescription Description => GetDescription(GetType());


        private event Action? _drawParameters;

        internal float UIHeight; 

        internal void DrawParameters()
        {
            if (_parameters == null)
                SetupParameters();

            _drawParameters?.Invoke();
        }

        public Effect()
        {
        }

        private void SetupParameters()
        {
            Stopwatch watch = Stopwatch.StartNew();
            Type type = GetType();

            List<OrderedUI> UIs = new();

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;

            var properties = type.GetProperties(flags)
                .Where(property => Attribute.IsDefined(property, typeof(Param)) && typeof(Parameter).IsAssignableFrom(property.PropertyType));

            foreach (var property in properties)
                UIs.Add(UIFromMember(property, (Parameter)property.GetValue(this)!));

            var fields = type.GetFields(flags)
                .Where(field => Attribute.IsDefined(field, typeof(Param)) && typeof(Parameter).IsAssignableFrom(field.FieldType));

            foreach (var field in fields)
                UIs.Add(UIFromMember(field, (Parameter)field.GetValue(this)!));

            var methods = type.GetMethods(flags)
                .Where(method => Attribute.IsDefined(method, typeof(UIMethod)));

            foreach (var method in methods)
                UIs.Add(UIFromMethod(method));

            UIs.Sort((a, b) => a.Order.CompareTo(b.Order));

            string? path = null;
            Stack<string> groupNames = new();

            Stack<Action?> groupHandlers = new();
            groupHandlers.Push(null);

            List<UIParameter> parameters = new();

            foreach (var UI in UIs)
            {
                foreach (string group in UI.BeginGroups)
                {
                    groupHandlers.Push(null);

                    groupNames.Push(group);
                    path = string.Join('/', groupNames);
                }

                for (int i = 0; i < UI.EndGroups; i++)
                {
                    var childHandler = groupHandlers.Pop();
                    var parentHandler = groupHandlers.Pop();

                    string name = groupNames.Pop();
                    groupHandlers.Push(parentHandler + (() => ImGuiHelper.Group(name, childHandler)));

                    if (groupNames.Count > 0)
                        path = string.Join('/', groupNames);
                    else
                        path = null;
                }

                if (UI.Handler == null)
                {
                    var newParameter = new UIParameter(UI.NamedParameter.Parameter, UI.NamedParameter.Name, path);
                    parameters.Add(newParameter);

                    var parent = groupHandlers.Pop();
                    groupHandlers.Push(parent + newParameter.DrawUI);
                }
                else
                {
                    var parent = groupHandlers.Pop();
                    groupHandlers.Push(parent + UI.Handler);
                }
            }

            _parameters = new(parameters);
            _drawParameters = groupHandlers.Peek();
            watch.Stop();
            Console.WriteLine("Setup Time" + watch.ElapsedTicks / 10_000f);
        }

        private static OrderedUI UIFromMember(MemberInfo member, Parameter parameter)
        {
            Param param = member.GetCustomAttribute<Param>()!;
            // TODO: might wanna get these in order of declaration
            var beginGroups = member.GetCustomAttributes<BeginGroup>().Select(group => group.Name).ToList();
            int endGroups = member.GetCustomAttributes<EndGroup>().Count();
            string name = param.CustomName ?? StringUtilities.UnPascalCase(member.Name);
            return new OrderedUI(
                param.Order, new(parameter, name),
                beginGroups, endGroups);
        }

        private OrderedUI UIFromMethod(MethodInfo method)
        {
            UIMethod ui = method.GetCustomAttribute<UIMethod>()!;
            var beginGroups = method.GetCustomAttributes<BeginGroup>().Select(group => group.Name).ToList();
            int endGroups = method.GetCustomAttributes<EndGroup>().Count();
            var handler = method.CreateDelegate<Action>(this);
             return new OrderedUI(
                ui.Order, UIParameter.Empty, beginGroups,
                endGroups, handler);
        }

        public static Dictionary<string, Dictionary<string, Type>> Effects { get; } = new();

        public static void LoadEffectsFromAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Effect)))
                {
                    if (type.IsAbstract)
                        continue;

                    var description = GetDescription(type);
                    if (description.Hidden)
                        continue;
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


        private static Dictionary<Type, EffectDescription> EffectDescriptions { get; } = new();
        public static EffectDescription GetDescription(Type type)
        {
            if (EffectDescriptions.TryGetValue(type, out EffectDescription cachedDesc))
                return cachedDesc;

            Description? descAttribute = type.GetCustomAttribute<Description>();

            var newDescription = new EffectDescription(
                descAttribute?.Name ?? StringUtilities.UnPascalCase(type.Name),
                descAttribute?.Category,
                descAttribute?.Hidden ?? false
            );

            EffectDescriptions.Add(type, newDescription);
            return newDescription;
        }

        // TODO: could crash
        public static Effect Create(Type type) => (Effect)Activator.CreateInstance(type)!;

        private readonly struct OrderedUI
        {
            public int Order { get; init; }
            public UIParameter NamedParameter { get; init; }
            public List<string> BeginGroups { get; init; }
            public int EndGroups { get; init; }
            public Action? Handler { get; init; }

            public OrderedUI(int order, UIParameter namedParameter, List<string> beginGroup, int endGroup, Action? handler = null)
            {
                Order = order;
                NamedParameter = namedParameter;
                BeginGroups = beginGroup;
                EndGroups = endGroup;
                Handler = handler;
            }
        }

        /*private readonly struct OrderedUI
        {
            public int Order { get; init; }
            public Action Handler { get; init; }

            public OrderedUI(int order, Action handler)
            {
                Order = order;
                Handler = handler;
            }
        }*/
    } 
    public abstract class VideoEffect : Effect
    {
        public abstract RenderResult Render(RenderArgs args);
    }

    // TODO: Audio effects ( just audio in general )

    public readonly struct EffectDescription
    {
        public const string DefaultCategory = "Global";

        public string Name { get; init; }
        public bool Hidden { get; init; }

        private readonly string? _category;
        public string Category
        {
            get => _category ?? DefaultCategory;
            init => _category = value;
        }

        public EffectDescription(string name, string? category = null, bool hidden = false)
        {
            Name = name;
            _category = category;
            Hidden = hidden;
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
