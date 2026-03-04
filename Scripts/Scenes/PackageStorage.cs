#nullable enable
using Godot;
using System;
using System.Collections.Generic;
using InputSystem;
using ServiceSystem;

public partial class PackageStorage : Node2D, IInputState {
	[Export]
	private PackedScene _placeholderTile = null!;

	[Export]
	private PackedScene _placeholderPackage = null!;

	private readonly Dictionary<StorageTile, Vector2I> _tilePositions = [];

	private int _rows = 5;
	private int _columns = 5;
	private int _xOffset = 32;
	private int _yOffset = 32;

	private StorageTile?[,] _tiles = null!;
	private StorageTile? _currentTile;
	private PackageGO? _currentPackage;
	private Vector2I? _lastPackageTile;
	private PackageGO? _lastPackage;
	private bool _dragPackage = false;

	public override void _Ready() {
		ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
		InputStateMachine inputStateMachine = serviceLocator.GetService<InputStateMachine>();
		inputStateMachine.SetState(this);

		_tiles = new StorageTile[_rows, _columns];
		_CreateGrid();
		_CreatePlaceholderPackage();
	}

	public void ProcessInput(InputEventDto eventDto) {
		if (eventDto is MouseButtonPressedDto && _currentPackage != null) {
			GD.Print("Pressed");
			_currentPackage.SetOpacity(PackageGO.Opacity.Half);
			_dragPackage = true;
		}

		if (eventDto is MouseButtonReleasedDto) {
			GD.Print("Released");
			if (_dragPackage) {
				_currentPackage?.SetOpacity(PackageGO.Opacity.Full);
				_dragPackage = false;
			}
		}

		if (eventDto is MouseMotionDto mouseMotion) {
			if (!_dragPackage) {
				return;
			}

			if (_currentPackage is null) {
				_dragPackage = false;
				return;
			}

			_currentPackage.Position += mouseMotion.Relative;
		}
	}

	private void _CreatePlaceholderPackage() {
		PackageGO package = (PackageGO)_placeholderPackage.Instantiate();
		package.Position = new Vector2(300, 0);
		AddChild(package);
		package.Hovered += _HandlePackageHover;
	}

	private void _CreateGrid() {
		for (int row = 0; row < _rows; row++) {
			for (int column = 0; column < _columns; column++) {
				StorageTile tile = (_placeholderTile.Instantiate() as StorageTile)!;
				_tilePositions[tile] = new Vector2I(row, column);
				_tiles[row, column] = tile;
				AddChild(tile);
				tile.Position = new Vector2(column * _xOffset, row * _yOffset);
				tile.Hovered += _HandleStorageTileHover;
			}
		}
	}

	private void _ResetHighlights() {
		foreach (StorageTile tile in _tilePositions.Keys) {
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
			_currentPackage = package;
			_lastPackageTile = tilePosition;
			_lastPackage = package;
			return;
		}

		if (_lastPackage == package && tilePosition == _lastPackageTile) {
			_currentPackage = null;
		}
	}

	private void _HandleStorageTileHover(StorageTile tile, bool hovered) {
		if (hovered) {
			_currentTile = tile;
			// GD.Print($"New hovered tile. {_tilePositions[tile]}");
			return;
		}

		if (_currentTile == tile) {
			// GD.Print("No hovered tile");
			_currentTile = null;
		}
	}
}
