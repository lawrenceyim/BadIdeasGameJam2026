using Godot;

public partial class TestScene2 : Node, IScene {
    public void InitScene(string jsonParameters) {
        GD.Print($"Test scene 2 received parameters {jsonParameters}");
    }
}