using Godot;
using InputSystem;
using ServiceSystem;

public partial class CustomerView : Node2D, IInputState {
    private InputStateMachine _inputStateMachine;
    private SceneManager _sceneManager;

    public override void _Ready() {
        ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        _sceneManager = serviceLocator.GetService<SceneManager>();
        _inputStateMachine = serviceLocator.GetService<InputStateMachine>();
        _inputStateMachine.SetState(this);
    }

    public void ProcessInput(InputEventDto eventDto) {
        if (eventDto is KeyDto keyDto) {
            _HandleKeyPress(keyDto);
        }
    }

    private void _HandleKeyPress(KeyDto keyDto) {
        if (keyDto.Pressed) {
            if (keyDto.Identifier == "Space") {
                GD.Print("Space pressed to switch scene");
                _sceneManager.ChangeScene(SceneId.StorageView, string.Empty);
            }
        }
    }
}