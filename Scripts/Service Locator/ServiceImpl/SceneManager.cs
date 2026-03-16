using Godot;
using ServiceSystem;

public class SceneManager : IService {
    private readonly SceneRepository _sceneRepository;
    private SceneId _currentSceneId;

    public SceneManager(SceneRepository sceneRepository) {
        _sceneRepository = sceneRepository;
    }

    public void ChangeScene(SceneId sceneId, string jsonParameters) {
        GD.Print($"Changing scene id {sceneId}");
        _currentSceneId = sceneId;
        Node instance = _sceneRepository.GetPackedScene(sceneId).Instantiate();

        if (Engine.GetMainLoop() is not SceneTree sceneTree) {
            GD.Print($"Scene tree is null");
            return;
        }

        sceneTree.CurrentScene?.QueueFree();
        sceneTree.Root.AddChild(instance);
        if (instance is IScene scene) {
            GD.Print($"scene is IScene");
            scene.InitScene(jsonParameters);
        }

        sceneTree.CurrentScene = instance;
    }

    public SceneId GetCurrentSceneId() {
        return _currentSceneId;
    }
}