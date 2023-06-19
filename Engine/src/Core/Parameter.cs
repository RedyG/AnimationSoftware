using Engine.UI;
using Engine.Utilities;
using ImGuiNET;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public abstract class Parameter
    {
        public abstract Parameter? LinkedParameter { get; set; }

        /*public static Parameter<ParameterList> CreateGroup(params UIParameter[] parameters) => new Parameter<ParameterList>(new ParameterList(parameters), false, false);
        public static Parameter<ParameterList> CreateGroup(List<UIParameter> parameters) => new Parameter<ParameterList>(new ParameterList(parameters), false, false);*/

        public T1 GetValueAsType<T1>() => GetValueAtTimeAsType<T1>(App.Project.Time);

        public void SetValueAsType<T1>(T1 value) => SetValueAtTimeAsType(App.Project.Time, value);

        public abstract T GetValueAtTimeAsType<T>(Timecode time);
        public abstract void SetValueAtTimeAsType<T>(Timecode time, T value);
        public abstract void RemoveNearestKeyframeAtTime(Timecode time); // go
        public abstract void AddKeyframe(Keyframe keyframe);
        public abstract bool IsKeyframedAtTime(Timecode time); // maybe go
        public abstract void DrawUI();

        public abstract KeyframeList? Keyframes { get; }

        public abstract float GetEditorHeightAt(int keyframeIndex);
        public abstract void SetEditorHeightAt(int keyframeIndex, float editorHeight);
        public abstract void SetEasingAt(int keyframeIndex, IEasing easing); // go
        public abstract IEasing GetEasingAt(int keyframeIndex); // go


        [JsonIgnore]
        public abstract UILocation UILocation { get; }
        public abstract bool CanBeKeyframed { get; }

        [JsonIgnore]
        public abstract bool IsKeyframed { get; }
        public abstract bool CanBeLinked { get; init; }

        [JsonIgnore]
        public abstract bool IsLinked { get; }

        [JsonIgnore]
        public bool Opened { get; set; }
    }

    public class Parameter<T> : Parameter
    {
        private Func<T, T>? _validateMethod;

        [JsonProperty]
        private T _unkeyframedValue;

        [JsonIgnore]
        public T Value
        {
            get => GetValueAtTime(App.Project.Time);
            set => SetValueAtTime(App.Project.Time, value);
        }

        public override KeyframeList? Keyframes { get; }

        public override bool CanBeKeyframed { get; } = true;
        public override bool IsKeyframed => Keyframes != null && Keyframes.Count != 0;


        public delegate T ValueGetterEventHandler(object? sender, ValueGetterEventArgs args);
        public event ValueGetterEventHandler? ValueGetter;

        public event EventHandler<ValueSetterEventArgs<T>>? ValueSetter;


        private Parameter? _linkedParameter;
        public override Parameter? LinkedParameter
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

                _ = value.GetValueAtTimeAsType<T>(App.Project.Time);

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

                    return Lerp(GetKeyframeValue(firstKeyframe), GetKeyframeValue(secondKeyframe), easedTime);
                }
            }
            return GetKeyframeValue(Keyframes[Keyframes.Count - 1]);
        }
        public void SetValueAtTime(Timecode time, T value)
        {
            if (ValueSetter != null)
            {
                ValueSetter(this, new ValueSetterEventArgs<T>(time, value));
                return;
            }

            if (IsKeyframed)
                AddKeyframe(new Keyframe(time, IEasing.Linear), value);
            else
            {
                var newValue = _validateMethod == null ? value : _validateMethod(value);
                if (_unkeyframedValue.Equals(newValue))
                    return;

                CommandManager.Execute(new ParameterValueChanged(this, newValue));
            }
        }

        public T BeginValueChange()
        {
            T value = Value;
            ImGuiHelper.BeginValueChange(value);
            return value;
        }

        public void EndValueChange(T value)
        {
            ImGuiHelper.EndValueChange(value, newValue => Value = newValue);
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

        public override void AddKeyframe(Keyframe keyframe)
        {
            if (Keyframes == null)
                return;

            T value = GetValueAtTime(keyframe.Time);
            Keyframes.Add(keyframe);
            SetKeyframeValue(keyframe, value);
        }

        public void AddKeyframe(Keyframe keyframe, T value)
        {
            if (Keyframes == null)
                return;

            Keyframes.Add(keyframe);
            SetKeyframeValue(keyframe, value);
        }

        private Dictionary<Keyframe, T>? _keyframeValues;

        /// <summary>Set the keyframe's value</summary>
        /// <param name="keyframe"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public void SetKeyframeValue(Keyframe keyframe, T value)
        {
            if (!Keyframes!.Contains(keyframe))
                throw new KeyNotFoundException();

            T validatedValue = _validateMethod == null ? value : _validateMethod(value);
            if (_keyframeValues!.ContainsKey(keyframe))
                _keyframeValues[keyframe] = validatedValue;
            else
                _keyframeValues.Add(keyframe, validatedValue);
        }

        /// <param name="keyframe"></param>
        /// <returns>The value of the keyframe</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public T GetKeyframeValue(Keyframe keyframe) => _keyframeValues![keyframe];

        public override void RemoveNearestKeyframeAtTime(Timecode time)
        {
            Keyframes?.RemoveNearestAtTime(time);
        }


        // TODO: will crash
        public override T1 GetValueAtTimeAsType<T1>(Timecode time) => (T1)Convert.ChangeType(GetValueAtTime(time), typeof(T1))!;

        // TODO: will crash
        public override void SetValueAtTimeAsType<T1>(Timecode time, T1 value) => SetValueAtTime(time, (T)Convert.ChangeType(value, typeof(T))!);


        public Parameter(T value)
        {
            Keyframes = new();
            _keyframeValues = new();
            SetupEvents();

            _unkeyframedValue = value;
        }
        public Parameter(T value, bool canBeKeframed, bool canBeLinked)
        {
            if (canBeKeframed)
            {
                Keyframes = new();
                _keyframeValues = new();
                SetupEvents();
            }
            _unkeyframedValue = value;
            CanBeKeyframed = canBeKeframed;
            CanBeLinked = canBeLinked;
        }

        public Parameter(T value, bool canBeKeframed, bool canBeLinked, IParameterUI<T>? customUI)
        {
            if (canBeKeframed)
            {
                Keyframes = new();
                _keyframeValues = new();
                SetupEvents();
            }
            _unkeyframedValue = value;
            CanBeKeyframed = canBeKeframed;
            CanBeLinked = canBeLinked;
            CustomUI = customUI;
        }


        public Parameter(T value, bool canBeKeframed, bool canBeLinked, Func<T, T> validateMethod)
        {
            if (canBeKeframed)
            {
                Keyframes = new();
                _keyframeValues = new();
                SetupEvents();
            }

            _unkeyframedValue = value;
            CanBeKeyframed = canBeKeframed;
            CanBeLinked = canBeLinked;
            _validateMethod = validateMethod;

        }

        private void SetupEvents()
        {
            if (Keyframes == null)
                throw new NullReferenceException();

            Keyframes.AddingKeyframe += OnAddingKeyframe;
            Keyframes.RemovingKeyframe += OnRemovingKeyframe;
            Keyframes.ClearingKeyframes += OnClearingKeyframes;
        }

        private void OnAddingKeyframe(object? sender, KeyframeEventArgs args)
        {
            _keyframeValues!.Add(args.Keyframe, GetValueAtTime(args.Keyframe.Time));
        }

        private void OnRemovingKeyframe(object? sender, KeyframeEventArgs args)
        {
            _keyframeValues!.Remove(args.Keyframe);
        }

        private void OnClearingKeyframes(object? sender, EventArgs args)
        {
            _keyframeValues!.Clear();
        }

        public delegate T Lerper(T a, T b, float t);
        public static Lerper? DefaultTypeLerp { get; set; }
        [JsonIgnore]
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

        public override void DrawUI()
        {
            if (CustomUI != null)
                CustomUI.Draw(this);
            else if (typeof(T).IsEnum)
            {
                var names = Enum.GetNames(typeof(T));
                var values = Enum.GetValues(typeof(T));
                var value = Value;
                var index = Array.IndexOf(values, value);

                var beforeIndex = index;
                ImGui.Combo("", ref index, names, names.Length);

                if (beforeIndex == index)
                    return;

                Value = (T)values.GetValue(index)!;
            }
            else
                ImGui.NewLine();
        }

        public override float GetEditorHeightAt(int keyframeIndex)
        {
            if (EditorConverter == null || Keyframes == null)
                return 0f;

            return EditorConverter.ToEditorHeight(GetKeyframeValue(Keyframes[keyframeIndex]));
        }

        public override void SetEditorHeightAt(int keyframeIndex, float editorHeight)
        {
            if (EditorConverter == null || Keyframes == null)
                return;

            SetKeyframeValue(Keyframes[keyframeIndex], EditorConverter.FromEditorHeight(editorHeight));

        }

        public override void SetEasingAt(int keyframeIndex, IEasing easing)
        {
            if (Keyframes == null)
                return;

            Keyframes[keyframeIndex].Easing = easing;
        }

        public override IEasing GetEasingAt(int keyframeIndex)
        {
            if (Keyframes == null)
                throw new Exception("This is not a keyframed parameter");

            return Keyframes[keyframeIndex].Easing;
        }

        public override UILocation UILocation => CustomUI?.Location ?? UILocation.Right;

        public static IEditorConverter<T>? EditorConverter { get; set; }



        public class ParameterValueChanged : ICommand
        {
            private Parameter<T> _parameter;
            private T _newValue;
            private T _oldValue;

            public string Name => $"Parameter value changed";

            public void Execute()
            {
                _oldValue = _parameter._unkeyframedValue;
                _parameter._unkeyframedValue = _newValue;
            }

            public void Undo()
            {
                _parameter._unkeyframedValue = _oldValue;
            }

            public ParameterValueChanged(Parameter<T> parameter, T newValue)
            {
                _parameter = parameter;
                _newValue = newValue;
            }
        }
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
