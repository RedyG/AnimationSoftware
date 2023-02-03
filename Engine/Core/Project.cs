namespace Engine.Core
{
    public class Project
    {
        public string Name { get; set; }
        public List<Scene> Scenes { get; } = new();

        public Scene? ActiveScene { get; set; }

        public Project(string name)
        {
            Name = name;
        }
    }
}