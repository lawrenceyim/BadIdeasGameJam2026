#nullable enable
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using InputSystem;
using RepositorySystem;
using ServiceSystem;

public partial class PackageStorage : Node2D, IInputState {
	[Export]
	private PackedScene _placeholderTile = null!;

	[Export]
	private PackedScene _placeholderPackage = null!;

	private readonly Dictionary<StorageTile, Vector2I> _storageTilePositions = [];
	private readonly Dictionary<PackageGO, Package> _packages = [];
	private PlayerDataRepository _playerDataRepository;

	private int _xOffset = 32;
	private int _yOffset = 32;

	private StorageTile?[,] _tiles = null!;
	private StorageTile? _selectedStorageTile;
	private PackageGO? _selectedPackage;
	private Vector2I _selectedPackageTile;
	private bool _draggingPackage = false;
	private Vector2 _positionBeforeDrag;

	public override void _Ready() {
		ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
		InputStateMachine inputStateMachine = serviceLocator.GetService<InputStateMachine>();
		inputStateMachine.SetState(this);
		RepositoryLocator repositoryLocator = serviceLocator.GetService<RepositoryLocator>();
		_playerDataRepository = repositoryLocator.GetRepository<PlayerDataRepository>(RepositoryName.PlayerData);
		
		_tiles = new StorageTile[PlayerDataRepository.StorageGridLength, PlayerDataRepository.StorageGridLength];
		_CreateGrid(PlayerDataRepository.StorageGridLength, PlayerDataRepository.StorageGridLength, _xOffset, _yOffset);

		// TODO: Replace with func to read from persistence storage and init packages
		_CreatePlaceholderPackage();
	}

	public void ProcessInput(InputEventDto eventDto) {
		if (eventDto is MouseButtonPressedDto && _selectedPackage != null) {
			_positionBeforeDrag = _selectedPackage.Position;
			_selectedPackage.SetOpacity(PackageGO.Opacity.Half);
			_draggingPackage = true;
		}

		if (eventDto is MouseButtonReleasedDto) {
			if (_draggingPackage) {
				_selectedPackage?.SetOpacity(PackageGO.Opacity.Full);
				_draggingPackage = false;
				_ClearAllHighlights();
				_SnapPackage();
			}
		}

		if (eventDto is MouseMotionDto mouseMotion) {
			if (!_draggingPackage) {
				return;
			}

			if (_selectedPackage is null) {
				_draggingPackage = false;
				return;
			}

			_selectedPackage.Position += mouseMotion.Relative;
		}
	}

	private void _SnapPackage() {
		if (_selectedPackage is null) {
			return;
		}

		if (_selectedStorageTile is null) {
			// TODO: Add func to snap packages to last valid position if new position is invalid
			return;
		}

		// TODO: Add validity check to see if position is occupied
		if (!_IsValidPackagePosition(_packages[_selectedPackage])) {
			_selectedPackage.Position = _positionBeforeDrag;
			return;
		}

		Vector2I offset = new(_selectedPackageTile.X * 32, _selectedPackageTile.Y * 32);
		GD.Print($"Current storage tile position {_storageTilePositions[_selectedStorageTile]} Current package tile {_selectedPackageTile} {offset}");
		_selectedPackage.Position = _selectedStorageTile.Position - offset;
		// TODO: Save updated data 
	}

	private bool _IsValidPackagePosition(Package package) {
		// TODO: Compute which Vector2I to check
		if (_selectedStorageTile is null) {
			GD.Print("No storage tile selected");
			return false;
		}

		if (!_storageTilePositions.ContainsKey(_selectedStorageTile)) {
			GD.Print($"Current storage tile position {_selectedStorageTile} does not exist");
			return false;
		}

		List<Vector2I> positionsToCheck = _ComputeOverlappingTiles(_packages[_selectedPackage], _storageTilePositions[_selectedStorageTile], _selectedPackageTile);
		GD.Print($"ISVALIDPACKAGE Storage tile {_storageTilePositions[_selectedStorageTile]} Package Tile  {_selectedPackageTile}");
		foreach (Vector2I position in positionsToCheck) {
			if (!_IsGridValidTile(position, _playerDataRepository.StorageGrid.GetLength(0), _playerDataRepository.StorageGrid.GetLength(1))) {
				return false;
			}
		}

		// foreach (Vector2I position in positionsToCheck) {
		//     if (_playerDataRepository.StorageGrid[position.X, position.Y] != 0 ||
		//         _playerDataRepository.StorageGrid[position.X, position.Y] != package.PackageId) {
		//         return false;
		//     }
		// }

		return true;
	}

	private void _CreatePlaceholderPackage() {
		PackageGO packageGo = (PackageGO)_placeholderPackage.Instantiate();
		packageGo.Position = new Vector2(300, 0);
		AddChild(packageGo);
		packageGo.Hovered += _HandlePackageHover;
		List<Vector2I> result = packageGo.HitboxPositions
			.Select(v => (Vector2I)v)
			.ToList();
		_packages[packageGo] = new Package(1, TextureId.PlaceHolder, result);
	}

	private void _CreateGrid(int columns, int rows, int xOffset, int yOffset) {
		for (int row = 0; row < rows; row++) {
			for (int column = 0; column < columns; column++) {
				StorageTile tile = (_placeholderTile.Instantiate() as StorageTile)!;
				tile.Centered = false;
				tile.GetNode<Area2D>("Area2D").GetNode<CollisionShape2D>("CollisionShape2D").Position = new Vector2I(xOffset, yOffset) / 2;
				_storageTilePositions[tile] = new Vector2I(column, row);
				_tiles[column, row] = tile;
				AddChild(tile);
				tile.Position = new Vector2(column * xOffset, row * yOffset);
				tile.Hovered += _HandleStorageTileHover;
			}
		}

		Area2D exitHitbox = new();
		exitHitbox.Position = new Vector2I(columns * xOffset, rows * yOffset) / 2;
		AddChild(exitHitbox);
		CollisionShape2D exitHitboxShape = new();
		exitHitbox.AddChild(exitHitboxShape);
		RectangleShape2D rectShape = new();
		rectShape.Size = new Vector2I(columns * xOffset, rows * yOffset);
		exitHitboxShape.Shape = rectShape;
		exitHitbox.MouseExited += _ExitStorageTile;
	}

	private void _ResetHighlights() {
		foreach (StorageTile tile in _storageTilePositions.Keys) {
			tile.SetHighlight(StorageTile.Highlight.None);
		}
	}

	private void _HandlePackageHover(PackageGO package, bool hovered, Vector2I tilePosition) {
		// // GD.Print($"Hovered {hovered} {tilePosition}");
		// // Safety measure for cases where someone may alt-tab or something so package is no longer hovered
		// _currentPackage?.SetOpacity(PackageGO.Opacity.Full);
		// _dragPackage = false;

		// hovered false is called after hover entered when mouse cursor enters new tile?
		if (hovered) {
			// GD.Print($"{package} {hovered} {tilePosition}");
			_selectedPackage = package;
			_selectedPackageTile = tilePosition;
			return;
		}

		if (package == _selectedPackage && tilePosition == _selectedPackageTile) {
			// GD.Print("Current package set to null");
			_selectedPackage = null;
		}
	}

	private void _ExitStorageTile() {
		GD.Print("Exit storage");
		_selectedStorageTile = null;
	}

	private void _HandleStorageTileHover(StorageTile tile, bool hovered) {
		if (hovered) {
			_selectedStorageTile = tile;
		}

		_ClearAllHighlights();
		if (!_draggingPackage) {
			return;
		}

		if (_selectedPackage is null) {
			GD.Print("Current package is null");
			return;
		}

		GD.Print($"Current package tile {_selectedPackage}");
		if (_IsValidPackagePosition(_packages[_selectedPackage])) {
			_HighlightTiles(_packages[_selectedPackage], StorageTile.Highlight.Green);
		} else {
			_HighlightTiles(_packages[_selectedPackage], StorageTile.Highlight.Red);
		}
	}

	private void _HighlightTiles(Package package, StorageTile.Highlight highlight) {
		if (_selectedStorageTile is null) {
			GD.Print("_HighlightTiles selectedStorageTile is null");
			return;
		}

		List<Vector2I> tilesToHighlight = _ComputeOverlappingTiles(package, _storageTilePositions[_selectedStorageTile], _selectedPackageTile);
		foreach (Vector2I position in tilesToHighlight) {
			if (!_IsGridValidTile(position, _playerDataRepository.StorageGrid.GetLength(0), _playerDataRepository.StorageGrid.GetLength(1))) {
				continue;
			}

			_tiles[position.X, position.Y]?.SetHighlight(highlight);
		}
	}

	private void _ClearAllHighlights() {
		foreach (StorageTile tile in _storageTilePositions.Keys) {
			tile.SetHighlight(StorageTile.Highlight.None);
		}
	}

	private List<Vector2I> _ComputeOverlappingTiles(Package package, Vector2I storageTilePosition, Vector2I packageTilePosition) {
		GD.Print($"Compute overlapping tiles storage tile {storageTilePosition} package tile {packageTilePosition}");
		List<Vector2I> tiles = [];
		foreach (Vector2I position in package.Dimensions) {
			tiles.Add(storageTilePosition + position - packageTilePosition);
		}

		return tiles;
	}

	private bool _IsGridValidTile(Vector2I position, int rows, int columns) {
		return position.X >= 0 && position.X < columns && position.Y >= 0 && position.Y < rows;
	}
}
