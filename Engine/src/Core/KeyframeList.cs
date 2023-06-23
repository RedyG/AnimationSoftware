using Engine.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class KeyframeList : ICollection<Keyframe>
    {
        [JsonProperty]
        private readonly UndoableList<Keyframe> _list = new();
        private int GetClosest(int val1, int val2, Timecode time)
        {
            if (time - _list[val1].Time >= _list[val2].Time - time)
                return val2;
            else
                return val1;
        }
        private void SortList()
        {
            _list.Sort((a, b) => a.Time.Seconds.CompareTo(b.Time.Seconds));

            if (_list.Count <= 1)
                return;

            for (int i = 1; i < _list.Count; i++)
            {
                if (_list[i - 1].Time == _list[i].Time)
                    _list.RemoveAt(i - 1);
            }
        }

        public int NearestIndexAtTime(Timecode time)
        {

            int n = _list.Count;

            // Corner cases
            if (time <= _list[0].Time)
                return 0;
            if (time >= _list[n - 1].Time)
                return n - 1;

            // Doing binary search
            int i = 0, j = n, mid = 0;
            while (i < j)
            {
                mid = (i + j) / 2;

                if (_list[mid].Time == time)
                    return mid;

                // If time is less than list element, then search in left
                if (time < _list[mid].Time)
                {

                    // If time is greater than previous to mid, return closest of two
                    if (mid > 0 && time > _list[mid - 1].Time)
                        return GetClosest(mid - 1,
                                     mid, time);

                    // Repeat for left half
                    j = mid;
                }
                else
                {
                    if (mid < n - 1 && time < _list[mid + 1].Time)
                        return GetClosest(mid,
                             mid + 1, time);
                    i = mid + 1; // update i
                }
            }

            // Only single element left after search
            return mid;
        }
        public int IndexOf(Keyframe keyframe)
        {
            return _list.IndexOf(keyframe);
        }
        public void RemoveNearestAtTime(Timecode time)
        {
            int index = NearestIndexAtTime(time);

            OnRemovingKeyframe(new KeyframeEventArgs(_list[index]));

            _list.RemoveAt(index);
        }
        public void RemoveAt(int index)
        {
            OnRemovingKeyframe(new KeyframeEventArgs(_list[index]));

            _list.RemoveAt(index);
        }
        public Keyframe NearestAtTime(Timecode time)
        {
            return _list[NearestIndexAtTime(time)];
        }
        public Keyframe At(int index)
        {
            return _list.ElementAt(0);
        }
        public Keyframe this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        public int Count => _list.Count;
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds a keyframe to the list. If there is already a keyframe at the new keyframe's time, it will replace the old one.
        /// </summary>
        /// <param name="newKeyframe">The keyframe to add to the list</param>
        /// <remarks>Warning: use Parameter.AddKeyframe Instead of this method to ensure the keyframe has a value.</remarks>
        public void Add(Keyframe newKeyframe)
        {
            OnAddingKeyframe(new KeyframeEventArgs(newKeyframe));

            newKeyframe.TimeChanged += new EventHandler<EventArgs>(Keyframe_TimeChanged);

            for (int i = 0; i < _list.Count; i++)
            {
                var keyframe = _list[i];

                if (keyframe.Time == newKeyframe.Time)
                {
                    _list[i] = newKeyframe;
                    return;
                }
                
                if (keyframe.Time > newKeyframe.Time)
                {
                    _list.Insert(i, newKeyframe);
                    return;
                }
            }

            _list.Add(newKeyframe);
        }
        public void Clear()
        {
            OnClearingKeyframes();

            _list.Clear();
        }
        public bool Contains(Keyframe keyframe)
        {
            return _list.Contains(keyframe);
        }
        public void CopyTo(Keyframe[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }
        public bool Remove(Keyframe keyframe)
        {
            OnRemovingKeyframe(new KeyframeEventArgs(keyframe));

            return _list.Remove(keyframe);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        public IEnumerator<Keyframe> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        private void Keyframe_TimeChanged(object? sender, EventArgs e)
        {
            SortList();
        }

        public IEnumerable<Keyframe> Selected
        {
            get
            {
                foreach (var keyframe in _list)
                {
                    if (keyframe.Selected)
                        yield return keyframe;
                }
            }
        }
        public bool IsKeyframedAtTime(Timecode time)
        {
            foreach (var keyframe in _list)
            {
                if (keyframe.Time == time)
                    return true;
            }
            return false;
        }

        public KeyframeList()
        { 
        }

        public event EventHandler<KeyframeEventArgs> AddingKeyframe;
        public event EventHandler<KeyframeEventArgs> RemovingKeyframe;
        public event EventHandler ClearingKeyframes;

        private void OnAddingKeyframe(KeyframeEventArgs e)
        {
            AddingKeyframe?.Invoke(this, e);
        }

        private void OnRemovingKeyframe(KeyframeEventArgs e)
        {
            RemovingKeyframe?.Invoke(this, e);
        }

        private void OnClearingKeyframes()
        {
            ClearingKeyframes?.Invoke(this, EventArgs.Empty);
        }

    }
    public class KeyframeEventArgs : EventArgs
    {
        public Keyframe Keyframe { get; }

        public KeyframeEventArgs(Keyframe keyframe)
        {
            Keyframe = keyframe;
        }
    }
}
