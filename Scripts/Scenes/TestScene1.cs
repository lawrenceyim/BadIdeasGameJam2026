using Godot;
using ServiceSystem;

public partial class TestScene1 : Node, IScene {
	public void InitScene(string jsonParameters) {
		GD.Print($"Test scene 1 received parameters {jsonParameters}");
	}

	public override void _Ready() {
		CallDeferred(nameof(ChangeScene));
	}

	public void ChangeScene() {
		ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
		SceneManager sceneManager = serviceLocator.GetService<SceneManager>();
		sceneManager.ChangeScene(SceneId.TestScene2, "Test scene successful");
	}
}
