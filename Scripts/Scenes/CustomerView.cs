using Godot;
using InputSystem;
using ServiceSystem;

public partial class CustomerView : Node2D, IInputState, ITick {
    private InputStateMachine _inputStateMachine;
    private SceneManager _sceneManager;
    private GameClock _gameClock;
    private Vector2 _newPackageSpawnPosition = new(-60, 40);

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
            _SpawnCustomer();
            // TODO: Spawn new customer and new package
        }
    }

    private void _HandleLevelTimedOut() {
        // TODO: Add day vs night
    }

    private void _SpawnCustomer() {
        // TODO: create customer
        // TODO: create package
        _CreatePackage();
    }

    private void _CreatePackage() {
        Package package = new Package(
            PlayerDataRepository.NewPackageId++,
            TextureId.PlaceHolder,
            [
                new Vector2I(0, 0),
                new Vector2I(0, 1),
                new Vector2I(0, 2),
                new Vector2I(0, 3),
                new Vector2I(1, 0),
                new Vector2I(1, 1),
                new Vector2I(1, 2),
                new Vector2I(1, 3)
            ],
            5);

        PlayerDataRepository.PackagesInHolding.Add(package, _newPackageSpawnPosition);
        foreach (Vector2I pos in package.Dimensions) {
            PlayerDataRepository.HoldingGrid[pos.X, pos.Y] = package.PackageId;
        }


        // Init object on the counter
    }
}