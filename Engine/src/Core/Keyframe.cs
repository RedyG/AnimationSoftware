using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class Keyframe<T> : IComparable<Keyframe<T>>
    {
        internal Func<T, T>? ValidateMethod { get; set; }

        private Timecode _time;
        public Timecode Time
        {
            get => _time;
            set
            {
                _time = value;
                OnTimeChanged(EventArgs.Empty);
            }
        }

        public bool Selected { get; set; }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                T newValue = ValidateMethod == null ? value : ValidateMethod(value);
                CommandManager.ExecuteIfNeeded(_value, newValue, new ValueChangedCommand(this, newValue));
            }
        }
        public IEasing Easing { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Keyframe(Timecode time, T value, IEasing easing)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        { 
            _value = value;
            Time = time;
            Easing = easing;
        }

        public event EventHandler<EventArgs>? TimeChanged;
        private void OnTimeChanged(EventArgs e)
        {
            if (TimeChanged != null)
                TimeChanged(this, e);
        }

        public int CompareTo(Keyframe<T>? other) => Time.Seconds.CompareTo(other.Time.Seconds);

        public class ValueChangedCommand : ICommand
        {
            private Keyframe<T> _keyframe;
            private T _newValue;
            private T _oldValue;

            public string Name => "Keyframe value changed";

            public void Execute()
            {
                _oldValue = _keyframe._value;

                 _keyframe._value = _newValue;
            }

            public void Undo()
            {
                _keyframe._value = _oldValue;
            }

            public ValueChangedCommand(Keyframe<T> keyframe, T newValue)
            {
                _keyframe = keyframe;
                _newValue = newValue;
            }
        }
    }
}
