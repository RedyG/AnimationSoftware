using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Engine.Core
{
    public struct Timecode : IEquatable<Timecode>, IComparable<Timecode>
    {
        public static Timecode Zero = Timecode.FromSeconds(0);

        public float Seconds { get; set; }
        public float Milliseconds { get => Seconds * 1000f; set => Seconds = (value / 1000f); }
        public int Frames
        {
            get => GetFrames(App.Project.ActiveScene.FrameRate);
            set => SetFrames(value, App.Project.ActiveScene.FrameRate);
        }

        public int GetFrames(float frameRate) => (int)MathF.Round(Seconds * frameRate);
        public void SetFrames(int frames, float frameRate) => Seconds = (frames / (float)frameRate);

        public static Timecode FromFrames(int frames)
        {
            return FromFrames(frames, App.Project.ActiveScene.FrameRate);
        }
        public static Timecode FromFrames(int frames, float frameRate)
        {
            var timecode = new Timecode();
            timecode.SetFrames(frames, frameRate);
            return timecode;
        }

        public static Timecode FromMilliseconds(float milliseconds) => new Timecode { Milliseconds = milliseconds};

        public static Timecode FromSeconds(float seconds) => new Timecode { Seconds = seconds };

        public override int GetHashCode() => Seconds.GetHashCode();

        public bool Equals(Timecode other) => (Seconds == other.Seconds);
        public override bool Equals(object? obj) => (obj != null && obj is Timecode && Equals((Timecode)obj));

        public static bool operator ==(Timecode a, Timecode b) => a.Equals(b);
        public static bool operator !=(Timecode a, Timecode b) => !(a == b);
        public static bool operator <(Timecode a, Timecode b) => (a.Seconds < b.Seconds);
        public static bool operator >(Timecode a, Timecode b) => (a.Seconds > b.Seconds);
        public static bool operator <=(Timecode a, Timecode b) => (a.Seconds <= b.Seconds);
        public static bool operator >=(Timecode a, Timecode b) => (a.Seconds >= b.Seconds);
        public static Timecode operator +(Timecode a, Timecode b) => Timecode.FromSeconds(a.Seconds + b.Seconds);
        public static Timecode operator -(Timecode a, Timecode b) => Timecode.FromSeconds(a.Seconds - b.Seconds);
        public static Timecode operator *(Timecode a, Timecode b) => Timecode.FromSeconds(a.Seconds* b.Seconds);
        public static Timecode operator /(Timecode a, Timecode b) => Timecode.FromSeconds(a.Seconds / b.Seconds);

        public override string ToString()
        {
            // TODO: this uses ActiveItem which may not be what we want depending. ( might wanna remove frames and just use milliseconds or something )

            return ToString(App.Project.ActiveScene.FrameRate);
        }

        public string ToString(float frameRate)
        {
            return $"{(int)(Seconds / 3600f)}:".PadLeft(3, '0') +
                   $"{(int)((Seconds / 60f) % 60)}:".PadLeft(3, '0') +
                   $"{(int)(Seconds % 60)}:".PadLeft(3, '0') +
                   $"{GetFrames(frameRate) % frameRate}".PadLeft(2, '0');
        }

        public TimeSpan ToTimeSpan()
        {
            return TimeSpan.FromSeconds(Seconds);
        }

        public int CompareTo(Timecode other)
        {
            return Seconds.CompareTo(other.Seconds);
        }
    }
}
