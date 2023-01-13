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
        public float Seconds { get; set; }
        public float Milliseconds { get => Seconds * 1000f; set => Seconds = (value / 1000f); }

        public int Frames
        {
            get
            {
                if (App.Project == null || App.Project.ActiveScene == null) throw new Exception("Scene or project is null");
                return (int)MathF.Round(Seconds * App.Project.ActiveScene.FrameRate);
            }
            set
            {
                if (App.Project == null || App.Project.ActiveScene == null) throw new Exception("Scene or project is null");
                Seconds = (value / (float)App.Project.ActiveScene.FrameRate);
            }
        }

        public Timecode(float seconds)
        {
            Seconds = seconds;
        }

        public static Timecode FromFrames(int frames) => new Timecode { Frames = frames };
        public static Timecode FromMilliseconds(float milliseconds) => new Timecode { Milliseconds = milliseconds };
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
        public static Timecode operator +(Timecode a, Timecode b) => new Timecode(a.Seconds + b.Seconds);
        public static Timecode operator -(Timecode a, Timecode b) => new Timecode(a.Seconds - b.Seconds);

        public override string ToString()
        {
            if (App.Project == null || App.Project.ActiveScene == null) throw new Exception("Scene or project is null");

            return $"{(int)(Seconds / 3600f)}:".PadLeft(3, '0') +
                   $"{(int)((Seconds / 60f) % 60)}:".PadLeft(3, '0') +
                   $"{(int)(Seconds % 60)}:".PadLeft(3, '0') +
                   $"{Frames % App.Project.ActiveScene.FrameRate}".PadLeft(2, '0');
        }

        public int CompareTo(Timecode other)
        {
            return Seconds.CompareTo(other.Seconds);
        }
    }
}
