﻿using Engine.UI;
using Engine.Utilities;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public abstract class Parameter
    {
        public static Parameter<ParameterList> CreateGroup(params NamedParameter[] parameters) => new Parameter<ParameterList>(new ParameterList(parameters), false, false);
        public static Parameter<ParameterList> CreateGroup(List<NamedParameter> parameters) => new Parameter<ParameterList>(new ParameterList(parameters), false, false);

        // TODO: cache the methods using delegates or just the MethodInfo
        public T1 GetValueAsType<T1>() => GetValueAtTimeAsType<T1>(App.Project.Time);

        public void SetValueAsType<T1>(T1 value) => SetValueAtTimeAsType(App.Project.Time, value);

        public abstract T GetValueAtTimeAsType<T>(Timecode time);
        public abstract void SetValueAtTimeAsType<T>(Timecode time, T value);
        public abstract void RemoveNearestKeyframeAtTime(Timecode time);
        public abstract void AddKeyframeAtTime(Timecode time, IEasing easing);
        public abstract bool IsKeyframedAtTime(Timecode time);
        public abstract void DrawUI();
        public abstract UILocation UILocation { get; }
        public abstract bool CanBeKeyframed { get; }
        public abstract bool IsKeyframed { get; }
        public abstract bool CanBeLinked { get; init; }
        public abstract bool IsLinked { get; }
        public abstract bool Opened { get; set; }

        public Parameter()
        {
        }
    }
    public class Parameter<T> : Parameter
    {
        private Func<T, T>? _validateMethod;

        private T _unkeyframedValue;
        public T Value
        {
            get => GetValueAtTime(App.Project.Time);
            set => SetValueAtTime(App.Project.Time, value);
        }

        public KeyframeList<T>? Keyframes { get; }

        public override bool CanBeKeyframed { get; } = true;
        public override bool IsKeyframed => Keyframes != null && Keyframes.Count != 0;


        public delegate T ValueGetterEventHandler(object? sender, ValueGetterEventArgs args);
        public event ValueGetterEventHandler? ValueGetter;

        public event EventHandler<ValueSetterEventArgs<T>>? ValueSetter;


        private Parameter? _linkedParameter;
        public Parameter? LinkedParameter
        {
            get => _linkedParameter;
            set
            {
                if (!CanBeLinked)
                    return;

                if (value == null)
                {
                    _linkedParameter = null;
                    return;
                }

                try
                {
                    var temp = value.GetValueAtTimeAsType<T>(App.Project.Time);
                }
                catch
                {
                    throw new Exception("Couldn't cast.");
                }

                _linkedParameter = value;
            }
        }

        private bool _canBeLinked = true;
        public override bool CanBeLinked
        {
            get
            {
                if (ValueGetter != null)
                    return false;

                return _canBeLinked;
            }
            init => _canBeLinked = value;
        }
        public override bool IsLinked => LinkedParameter != null;

        public virtual T GetValueAtTime(Timecode time)
        {
            if (IsLinked)
                return LinkedParameter!.GetValueAtTimeAsType<T>(time);

            if (ValueGetter != null)
                return ValueGetter(this, new ValueGetterEventArgs(time));

            if (!IsKeyframed)
                return _unkeyframedValue;

            // TODO: optimize with binary search or something
            for (int i = 0; i < Keyframes!.Count - 1; i++)
            {
                var firstKeyframe = Keyframes[i];
                var secondKeyframe = Keyframes[i + 1];
                if (secondKeyframe.Time > time)
                {
                    var timeBetweenKeyframes = MathF.Max(MathUtilities.Map(time.Seconds, firstKeyframe.Time.Seconds, secondKeyframe.Time.Seconds, 0, 1), 0f);
                    var easedTime = firstKeyframe.Easing.Evaluate(timeBetweenKeyframes);

                    return Lerp(firstKeyframe.Value, secondKeyframe.Value, easedTime);
                }
            }
            return Keyframes[Keyframes.Count - 1].Value;
        }
        public void SetValueAtTime(Timecode time, T value)
        {
            if (ValueSetter != null)
            {
                ValueSetter(this, new ValueSetterEventArgs<T>(time, value));
                return;
            }

            if (IsKeyframed)
                Keyframes!.Add(new Keyframe<T>(time, value, IEasing.Linear));
            else
            {
                if (_validateMethod == null)
                    _unkeyframedValue = value;
                else
                    _unkeyframedValue = _validateMethod(value);
            }
        }

        // TODO: might wanna make this static
        private T _oldValue;

        public T BeginValueChange()
        {
            _oldValue = Value;
            return _oldValue;
        }

        public void EndValueChange(T value)
        {
            // TODO: will crash
            if (!_oldValue!.Equals(value))
                Value = value;
        }

        public override bool IsKeyframedAtTime(Timecode time)
        {
            if (Keyframes == null)
                return false;

            foreach (var keyframe in Keyframes)
            {
                if (keyframe.Time == time)
                    return true;
            }
            return false;
        }

        public override void AddKeyframeAtTime(Timecode time, IEasing easing)
        {
            // TODO: will crash
            Keyframes!.Add(new Keyframe<T>(time, GetValueAtTime(time), easing));
        }

        public override void RemoveNearestKeyframeAtTime(Timecode time)
        {
            Keyframes.RemoveNearestAtTime(time);
        }


        // TODO: will crash
        public override T1 GetValueAtTimeAsType<T1>(Timecode time) => (T1)Convert.ChangeType(GetValueAtTime(time), typeof(T1))!;

        // TODO: will crash
        public override void SetValueAtTimeAsType<T1>(Timecode time, T1 value) => SetValueAtTime(time, (T)Convert.ChangeType(value, typeof(T))!);

        public Parameter(T value)
        {
            Keyframes = new();
            _unkeyframedValue = value;
        }
        public Parameter(T value, bool canBeKeframed, bool canBeLinked)
        {
            if (canBeKeframed)
                Keyframes = new();
            _unkeyframedValue = value;
            CanBeKeyframed = canBeKeframed;
            CanBeLinked = canBeLinked;
        }

        public Parameter(T value, bool canBeKeframed, bool canBeLinked, Func<T, T> validateMethod)
        {
            _unkeyframedValue = value;
            CanBeKeyframed = canBeKeframed;
            CanBeLinked = canBeLinked;

            _validateMethod = validateMethod;
            if (canBeKeframed)
                Keyframes = new(validateMethod);
        }


        public delegate T Lerper(T a, T b, float t);
        public static Lerper? DefaultTypeLerp { get; set; }
        public Lerper? CustomLerp { get; set; } = DefaultTypeLerp;
        private T Lerp(T a, T b, float t)
        {
            if (CustomLerp != null)
                return CustomLerp(a, b, t);

            /*if (DefaultTypeLerp != null)
                return DefaultTypeLerp(a, b, t);*/

            // basically "Hold" easing.
            return a;
        }

        public static Type? DefaultTypeUI { get; set; }
        public IParameterUI<T>? CustomUI { get; set; } = DefaultTypeUI != null ? (IParameterUI<T>)Instancer.Create(DefaultTypeUI) : null;

        private Dictionary<object, int> currentItems = new();

        public override void DrawUI()
        {
            if (CustomUI != null)
                CustomUI.Draw(this);
            else if (typeof(T).IsEnum)
            {
                var names = Enum.GetNames(typeof(T));
                if (!currentItems.TryGetValue(this, out int currentItem))
                {
                    var index = Array.IndexOf(Enum.GetValues(typeof(T)), Value);
                    currentItems.Add(this, index);
                    currentItem = index;
                }

                var beforeCurrent = currentItem;
                ImGui.Combo("", ref currentItem, names, names.Length);

                if (beforeCurrent != currentItem)
                {
                    currentItems[this] = currentItem;
                    var values = Enum.GetValues(typeof(T));
                    Value = (T)values.GetValue(currentItem)!;
                }


            }
            else
                ImGui.NewLine();
        }

        public override UILocation UILocation => CustomUI?.Location ?? UILocation.Right;

        public override bool Opened { get; set; } = false;
    }

    public class ValueGetterEventArgs : EventArgs
    {
        public Timecode Time { get; }

        public ValueGetterEventArgs(Timecode time)
        {
            Time = time;
        }
    }
    public class ValueSetterEventArgs<T> : EventArgs
    {
        public T Value { get; }
        public Timecode Time { get; }

        public ValueSetterEventArgs(Timecode time, T value)
        {
            Time = time;
            Value = value;
        }
    }

}
