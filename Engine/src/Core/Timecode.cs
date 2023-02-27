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
        public Scene Scene { get; set; }
        public float Seconds { get; set; }
        public float Milliseconds { get => Seconds * 1000f; set => Seconds = (value / 1000f); }

        public int Frames
        {
            get
            {
                return (int)MathF.Round(Seconds * Scene.FrameRate);
            }
            set
            {
                Seconds = (value / (float)Scene.FrameRate);
            }
        }

        public static Timecode FromFrames(int frames) => FromFrames(frames, App.Project!.ActiveScene!);
        public static Timecode FromFrames(int frames, Scene scene) => new Timecode { Frames = frames, Scene = scene };

        public static Timecode FromMilliseconds(float milliseconds) => FromMilliseconds(milliseconds, App.Project!.ActiveScene!);
        public static Timecode FromMilliseconds(float milliseconds, Scene scene) => new Timecode { Milliseconds = milliseconds, Scene = scene };

        public static Timecode FromSeconds(float seconds) => FromSeconds(seconds, App.Project!.ActiveScene!);
        public static Timecode FromSeconds(float seconds, Scene scene) => new Timecode { Seconds = seconds, Scene = scene };

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

        public override string ToString()
        {
            return $"{(int)(Seconds / 3600f)}:".PadLeft(3, '0') +
                   $"{(int)((Seconds / 60f) % 60)}:".PadLeft(3, '0') +
                   $"{(int)(Seconds % 60)}:".PadLeft(3, '0') +
                   $"{Frames % Scene.FrameRate}".PadLeft(2, '0');
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
