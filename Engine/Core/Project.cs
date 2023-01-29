namespace Engine.Core
{
    public class Project
    {
        public string Name { get; set; }
        public List<Scene> Scenes { get; } = new();

        private Guid? _activeSceneId;
        public Scene? ActiveScene
        {
            get => Scenes.Find(scene => scene.Id == _activeSceneId);
            set
            {
                if (value == null)
                {
                    _activeSceneId = null;
                    return;
                }

                var scene = Scenes.Find(scene => scene.Id == value.Id);

                if (scene == null)
                    Scenes.Add(value);

                _activeSceneId = value.Id;

            }
        }

        public Project(string name)
        {
            Name = name;
        }
    }
}