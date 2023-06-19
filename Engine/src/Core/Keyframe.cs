using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Engine.Core
{
    public class Keyframe : IComparable<Keyframe>
    {

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


        private IEasing _easing;
        public IEasing Easing
        {
            get => _easing;
            set => CommandManager.ExecuteSetter($"Keyframe Easing Changed", _easing, value, easing => _easing = easing);
        }

        public bool Selected { get; set; }

        public Keyframe(Timecode time, IEasing easing)
        { 
            Time = time;
            _easing = easing;
        }

        public event EventHandler<EventArgs>? TimeChanged;
        private void OnTimeChanged(EventArgs e)
        {
            if (TimeChanged != null)
                TimeChanged(this, e);
        }

        public int CompareTo(Keyframe? other) => Time.Seconds.CompareTo(other.Time.Seconds);
    }
}
