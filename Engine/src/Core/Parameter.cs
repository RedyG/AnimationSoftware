using Engine.UI;
using Engine.Utilities;
using ImGuiNET;
using Newtonsoft.Json;
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

        public abstract float GetEditorHeightAt(int keyframeIndex);
        public abstract void SetEditorHeightAt(int keyframeIndex, float editorHeight);

        public abstract IEnumerable<KeyframeDefinition> KeyframeDefinitions { get; }
        public IEnumerable<KeyframeDefinition> SelectedKeyframeDefinitions
        {
            get
            {
                foreach (var def in KeyframeDefinitions)
                    if (def.Selected)
                        yield return def;
            }
        }

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

        public Parameter()
        {
        }
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
                var newValue = _validateMethod == null ? value : _validateMethod(value);
                CommandManager.ExecuteIfNeeded(_unkeyframedValue, newValue, new ParameterValueChanged(this, newValue));
            }
        }

        private T _oldValue;
        public T BeginValueChange()
        {
            _oldValue = Value;
            return _oldValue;
        }

        public void EndValueChange(T value)
        {
            if (_oldValue.Equals(value))
                return;

            ImGuiHelper.EditValue<T>(value, newValue => Value = newValue);
        }

        public void EditValue(T value, bool beginEdit, bool endEdit)
        {
            if (_oldValue.Equals(value))
                return;

            ImGuiHelper.EditValue<T>(value, newValue => Value = newValue, beginEdit, endEdit);
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

        [JsonConstructor]
        public Parameter()
        {
            Keyframes = new();
            _unkeyframedValue = default;
        }

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

        private static Dictionary<object, int> _currentItems = new();
        public override void DrawUI()
        {
            if (CustomUI != null)
                CustomUI.Draw(this);
            else if (typeof(T).IsEnum)
            {
                var names = Enum.GetNames(typeof(T));
                if (!_currentItems.TryGetValue(this, out int currentItem))
                {
                    var index = Array.IndexOf(Enum.GetValues(typeof(T)), Value);
                    _currentItems.Add(this, index);
                    currentItem = index;
                }

                var beforeCurrent = currentItem;
                ImGui.Combo("", ref currentItem, names, names.Length);

                if (beforeCurrent != currentItem)
                {
                    if (beforeCurrent == currentItem)
                        return;

                    _currentItems[this] = currentItem;
                    var values = Enum.GetValues(typeof(T));
                    Value = (T)values.GetValue(currentItem)!;
                }


            }
            else
                ImGui.NewLine();
        }

        public override float GetEditorHeightAt(int keyframeIndex)
        {
            if (EditorConverter == null || Keyframes == null)
                return 0f;

            return EditorConverter.ToEditorHeight(Keyframes[keyframeIndex].Value);
        }

        public override void SetEditorHeightAt(int keyframeIndex, float editorHeight)
        {
            if (EditorConverter == null || Keyframes == null)
                return;

            Keyframes[keyframeIndex].Value = EditorConverter.FromEditorHeight(editorHeight);

        }

        public override UILocation UILocation => CustomUI?.Location ?? UILocation.Right;

        public static IEditorConverter<T>? EditorConverter { get; set; }

        public override IEnumerable<KeyframeDefinition> KeyframeDefinitions
        {
            get
            {
                if (Keyframes == null)
                    yield break;

                foreach (var keyframe in Keyframes)
                    yield return KeyframeDefinition.FromKeyframe(keyframe);
            }
        }


        public class ParameterValueChanged : ICommand
        {
            private Parameter<T> _parameter;
            private T _newValue;
            private T _oldValue;

            public string Name => $"Parameter value changed " + typeof(T).Name ;

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

    public readonly struct KeyframeDefinition
    {
        public Timecode Time { get; }
        public IEasing Easing { get; }
        public bool Selected { get; }

        public static KeyframeDefinition FromKeyframe<T>(Keyframe<T> keyframe) => new KeyframeDefinition(keyframe.Time, keyframe.Easing, keyframe.Selected);

        public KeyframeDefinition(Timecode time, IEasing easing, bool selected)
        {
            Time = time;
            Easing = easing;
            Selected = selected;
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
