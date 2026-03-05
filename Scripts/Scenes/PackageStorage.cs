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

    private int _rows = 5;
    private int _columns = 5;
    private int _xOffset = 32;
    private int _yOffset = 32;

    private StorageTile?[,] _tiles = null!;
    private StorageTile? _currentStorageTile;
    private PackageGO? _currentPackage;
    private Vector2I _currentPackageTile;
    private bool _draggingPackage = false;
    private Vector2 _positionBeforeDrag;

    public override void _Ready() {
        ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        InputStateMachine inputStateMachine = serviceLocator.GetService<InputStateMachine>();
        inputStateMachine.SetState(this);
        RepositoryLocator repositoryLocator = serviceLocator.GetService<RepositoryLocator>();
        _playerDataRepository = repositoryLocator.GetRepository<PlayerDataRepository>(RepositoryName.PlayerData);

        _tiles = new StorageTile[_rows, _columns];
        _CreateGrid();

        // TODO: Replace with func to read from persistence storage and init packages
        _CreatePlaceholderPackage();
    }

    public void ProcessInput(InputEventDto eventDto) {
        if (eventDto is MouseButtonPressedDto && _currentPackage != null) {
            _positionBeforeDrag = _currentPackage.Position;
            _currentPackage.SetOpacity(PackageGO.Opacity.Half);
            _draggingPackage = true;
        }

        if (eventDto is MouseButtonReleasedDto) {
            if (_draggingPackage) {
                _currentPackage?.SetOpacity(PackageGO.Opacity.Full);
                _draggingPackage = false;
                _ClearAllHighlights();
                _SnapPackage();
            }
        }

        if (eventDto is MouseMotionDto mouseMotion) {
            if (!_draggingPackage) {
                return;
            }

            if (_currentPackage is null) {
                _draggingPackage = false;
                return;
            }

            _currentPackage.Position += mouseMotion.Relative;
        }
    }

    private void _SnapPackage() {
        if (_currentPackage is null) {
            return;
        }

        if (_currentStorageTile is null) {
            // TODO: Add func to snap packages to last valid position if new position is invalid
            return;
        }

        // TODO: Add validity check to see if position is occupied
        if (!_IsValidPackagePosition(_packages[_currentPackage])) {
            _currentPackage.Position = _positionBeforeDrag;
            return;
        }

        Vector2I offset = new(_currentPackageTile.X * 32, _currentPackageTile.Y * 32);
        // GD.Print($"{_storageTilePositions[_currentStorageTile]} {_currentPackageTile} {offset}");
        _currentPackage.Position = _currentStorageTile.Position - offset;
        // TODO: Save updated data 
    }

    private bool _IsValidPackagePosition(Package package) {
        // TODO: Compute which Vector2I to check
        List<Vector2I> positionsToCheck = _ComputeOverlappingTiles(_packages[_currentPackage], _storageTilePositions[_currentStorageTile], _currentPackageTile);
        GD.Print($"ISVALIDPACKAGE Storage tile {_storageTilePositions[_currentStorageTile]} Package Tile  {_currentPackageTile}");
        foreach (Vector2I position in positionsToCheck) {
            if (!_IsGridValidTile(position, _playerDataRepository.StorageGrid.GetLength(0), _playerDataRepository.StorageGrid.GetLength(1))) {
                GD.Print("Out of bounds");
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
        PackageGO package = (PackageGO)_placeholderPackage.Instantiate();
        package.Position = new Vector2(300, 0);
        AddChild(package);
        package.Hovered += _HandlePackageHover;
        List<Vector2I> result = package.HitboxPositions
            .Select(v => (Vector2I)v)
            .ToList();
        _packages[package] = new Package(1, TextureId.PlaceHolder, result);
    }

    private void _CreateGrid() {
        for (int row = 0; row < _rows; row++) {
            for (int column = 0; column < _columns; column++) {
                StorageTile tile = (_placeholderTile.Instantiate() as StorageTile)!;
                tile.Centered = false;
                tile.GetNode<Area2D>("Area2D").GetNode<CollisionShape2D>("CollisionShape2D").Position = new Vector2I(16, 16);
                _storageTilePositions[tile] = new Vector2I(row, column);
                _tiles[row, column] = tile;
                AddChild(tile);
                tile.Position = new Vector2(column * _xOffset, row * _yOffset);
                tile.Hovered += _HandleStorageTileHover;
            }
        }
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
            _currentPackage = package;
            _currentPackageTile = tilePosition;
            return;
        }

        if (package == _currentPackage && tilePosition == _currentPackageTile) {
            // GD.Print("Current package set to null");
            _currentPackage = null;
        }
    }

    private void _HandleStorageTileHover(StorageTile tile, bool hovered) {
        if (hovered) {
            _currentStorageTile = tile;
            // GD.Print($"New hovered tile. {_storageTilePositions[tile]}");
        } else if (_currentStorageTile == tile) {
            // GD.Print("No hovered tile");
            _currentStorageTile = null;
        }

        _ClearAllHighlights();
        if (!_draggingPackage) {
            return;
        }

        if (_IsValidPackagePosition(_packages[_currentPackage])) {
            _HighlightTiles(_currentPackage, StorageTile.Highlight.Green);
        } else {
            _HighlightTiles(_currentPackage, StorageTile.Highlight.Red);
        }
    }

    private void _HighlightTiles(PackageGO package, StorageTile.Highlight highlight) {
        List<Vector2I> tilesToHighlight = _ComputeOverlappingTiles(_packages[_currentPackage], _storageTilePositions[_currentStorageTile], _currentPackageTile);
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
        List<Vector2I> tiles = [];
        foreach (Vector2I position in package.Dimensions) {
            tiles.Add(storageTilePosition + position - packageTilePosition);
        }

        return tiles;
    }

    private bool _IsGridValidTile(Vector2I position, int rows, int columns) {
        return position.X >= 0 && position.X < rows && position.Y >= 0 && position.Y < columns;
    }
}