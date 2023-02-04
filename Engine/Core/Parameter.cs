using Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public abstract class Parameter
    {
        public T GetCurrentValueAsType<T>() => GetValueAtTimeAsType<T>(App.Project!.ActiveScene!.Time);
        public void SetCurrentValueAsType<T>(T value) => SetValueAtTimeAsType<T>(value, App.Project!.ActiveScene!.Time);
        public T GetValueAtTimeAsType<T>(Timecode time)
        {
            var result = GetType().GetMethod("GetValueAtTime")!.Invoke(this, new object[] { time })!;
            return (T)Convert.ChangeType(result, typeof(T));
        }
        public void SetValueAtTimeAsType<T>(T value, Timecode time)
        {
            var castedValue = (T)Convert.ChangeType(value, typeof(T))!;
            GetType().GetMethod("SetValueAtTime")!.Invoke(this, new object[] { castedValue, time });
        }

        public KeyframeList<T> GetKeyframesAsType<T>()
        {
            var result = GetType().GetProperty("Keyframes")!.GetValue(this)!;
            return (KeyframeList<T>)Convert.ChangeType(result, typeof(KeyframeList<T>));
        }


        public abstract bool CanBeKeyframed { get; }
        public abstract bool IsKeyframed { get; }
        public abstract bool CanBeLinked { get; init; }
        public abstract bool IsLinked { get; }

    }
    public class Parameter<T> : Parameter
    {
        private T _unkeyframedValue;
        public T Value
        {
            get => GetValueAtTime(App.Project!.ActiveScene!.Time);
            set => SetValueAtTime(App.Project!.ActiveScene!.Time, value);
        }

        private KeyframeList<T> _keyframes = new();
        public KeyframeList<T>? Keyframes
        {
            get
            {
                if (CanBeKeyframed)
                {
                    return _keyframes;
                }

                return null;
            }
        }
        public override bool CanBeKeyframed { get; } = true;
        public override bool IsKeyframed
        {
            get
            {
                if (Keyframes == null)
                    return false;

                return Keyframes.Count != 0;
            }
        }

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
                    var temp = value.GetValueAtTimeAsType<T>(App.Project!.ActiveScene!.Time);
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

        public T GetValueAtTime(Timecode time)
        {
            if (IsLinked)
                return LinkedParameter!.GetValueAtTimeAsType<T>(time);

            if (ValueGetter != null)
                return ValueGetter(this, new ValueGetterEventArgs(time));

            if (!IsKeyframed)
                return _unkeyframedValue;

            // TODO : optimize with binary search or something
            for (int i = 0; i < Keyframes!.Count - 1; i++)
            {
                var firstKeyframe = Keyframes[i];
                var secondKeyframe = Keyframes[i + 1];
                if (secondKeyframe.Time > time)
                {
                    var timeBetweenKeyframes = MathF.Max(MathUtilities.Map(time.Seconds, firstKeyframe.Time.Seconds, secondKeyframe.Time.Seconds, 0, 1), 0f);
                    var easedTime = firstKeyframe.Easing.Evaluate(timeBetweenKeyframes);
                    return ParameterValues<T>.Lerp(firstKeyframe.Value, secondKeyframe.Value, easedTime);
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
                Keyframes!.Add(new Keyframe<T>(App.Project!.ActiveScene!.Time, value, EasingPresets.Linear));
            else
                _unkeyframedValue = value;
        }

        public Parameter(T value)
        {
            _unkeyframedValue = value;
        }
        public Parameter(T value, bool canBeKeframed, bool canBeLinked)
        {
            _unkeyframedValue = value;
            CanBeKeyframed = canBeKeframed;
            CanBeLinked = canBeLinked;
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
