using Godot;
using System;
using InputSystem;
using RepositorySystem;
using ServiceSystem;

public partial class StorageView : Node2D, IInputState {
	private PlayerDataRepository _playerDataRepository;
	
	[Export]
	private PackageStorage _packageStorage;

	[Export]
	private PackageStorage _shippingStorage;

	public override void _Ready() {
		ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
		InputStateMachine inputStateMachine = serviceLocator.GetService<InputStateMachine>();
		inputStateMachine.SetState(this);
		RepositoryLocator repositoryLocator = serviceLocator.GetService<RepositoryLocator>();
		_playerDataRepository = repositoryLocator.GetRepository<PlayerDataRepository>(RepositoryName.PlayerData);

		_packageStorage.Position = new Vector2(-300, 0);
		_shippingStorage.Position = new Vector2(300, 0);
		_packageStorage.Initialize(new Vector2I(32, 32), PackageStorage.StorageMode.Storage);
		_shippingStorage.Initialize(new Vector2I(32, 32), PackageStorage.StorageMode.Shipping);
	}

	public void ProcessInput(InputEventDto eventDto) { }
	
	
	
	// private void _CreatePlaceholderPackage() {
	// 	PackageGO packageGo = (PackageGO)_placeholderPackage.Instantiate();
	// 	packageGo.Position = new Vector2(300, 0);
	// 	AddChild(packageGo);
	// 	packageGo.Hovered += _HandlePackageHover;
	// 	List<Vector2I> result = packageGo.HitboxPositions
	// 		.Select(v => (Vector2I)v)
	// 		.ToList();
	// 	_packages[packageGo] = new Package(1, TextureId.PlaceHolder, result);
	//
	// 	PackageGO packageGo1 = (PackageGO)_placeholderPackage.Instantiate();
	// 	packageGo1.Position = new Vector2(300, 300);
	// 	AddChild(packageGo1);
	// 	packageGo1.Hovered += _HandlePackageHover;
	// 	List<Vector2I> result1 = packageGo1.HitboxPositions
	// 		.Select(v => (Vector2I)v)
	// 		.ToList();
	// 	_packages[packageGo1] = new Package(2, TextureId.PlaceHolder, result);
	// }

}
