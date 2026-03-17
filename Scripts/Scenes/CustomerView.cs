using Godot;
using InputSystem;
using ServiceSystem;

public partial class CustomerView : Node2D, IInputState, ITick {
    private InputStateMachine _inputStateMachine;
    private SceneManager _sceneManager;
    private GameClock _gameClock;

    public override void _Ready() {
        ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        _sceneManager = serviceLocator.GetService<SceneManager>();
        _inputStateMachine = serviceLocator.GetService<InputStateMachine>();
        _gameClock = serviceLocator.GetService<GameClock>();
        _gameClock.AddActiveScene(this, GetInstanceId());
        _inputStateMachine.SetState(this);

        PlayerDataRepository.LevelTimer.TimedOut += _HandleLevelTimedOut;
    }

    public override void _ExitTree() {
        _gameClock.RemoveActiveScene(GetInstanceId());
        PlayerDataRepository.LevelTimer.TimedOut -= _HandleLevelTimedOut;
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

    public void PhysicsTick() {
        PlayerDataRepository.LevelTimer.PhysicsTick();

        if (PlayerDataRepository.PackagesInHolding.Count == 0) {
            // TODO: Spawn new customer and new package
        }
    }

    private void _HandleLevelTimedOut() {
        // TODO: Add day vs night
    }
}