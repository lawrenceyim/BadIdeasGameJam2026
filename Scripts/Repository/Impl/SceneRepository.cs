using Godot;
using Godot.Collections;
using RepositorySystem;

public partial class SceneRepository : Node, IAutoload, IRepository {
    public static string AutoloadPath { get; } = "/root/SceneRepository";

    [Export]
    private Dictionary<SceneId, PackedScene> _packedScenes;

    public PackedScene GetPackedScene(SceneId sceneId) {
        return _packedScenes[sceneId];
    }
}

public enum SceneId {
    MainMenu = 1,
    CustomerView = 1_001,
    StorageView = 1_002,

    TestScene1 = 10_0001,
    TestScene2 = 10_0002,
}