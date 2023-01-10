namespace Engine.Core
{
    public class Project
    {
        public List<Scene> Scenes { get; }

        private Guid? _activeSceneId;
        public Scene? ActiveScene
        {
            get => Scenes.Find(scene => scene.Id == _activeSceneId);
            set => _activeSceneId = value?.Id;
        }

        public Project()
        {
            Scenes = new List<Scene>();
        }
    }
}