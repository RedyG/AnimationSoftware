namespace Engine.Core
{
    public class Project
    {
        public string Name { get; set; }
        public List<Scene> Scenes { get; } = new();

        public Scene ActiveScene { get; set; }

        public Timecode Time = Timecode.FromSeconds(0);

        public Project(string name)
        {
            Name = name;
        }
    }
}