#nullable enable
using System;
using System.Collections.Generic;
using Godot;
using RepositorySystem;
using ServiceSystem;

/*
 * TODO: Refactor to be modular so multiple storages can be created for UI
 * To refactor:
 * - offsets // don't hardcode
 * - ability to decide which storage to check against in player repository
 */
public partial class PackageStorage : Node2D {
    public enum StorageMode {
        None,
        Storage,
        Shipping,
    }

    public event Action<StorageMode, Vector2I> TileHovered;
    public event Action<StorageMode> StorageUnhovered;

    [Export]
    private PackedScene _placeholderTile = null!;

    private readonly Dictionary<StorageTile, Vector2I> _storageTilePositions = [];
    private StorageTile?[,] _tiles = null!;
    private StorageMode _mode;
    private Vector2I _tileSize;
    private PlayerDataRepository _playerDataRepository;
    private int _columns;
    private int _rows;


    public override void _Ready() {
        ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        RepositoryLocator repositoryLocator = serviceLocator.GetService<RepositoryLocator>();
        _playerDataRepository = repositoryLocator.GetRepository<PlayerDataRepository>(RepositoryName.PlayerData);
    }

    public void Initialize(Vector2I tileSize, StorageMode storageMode) {
        _mode = storageMode;
        _tileSize = tileSize;
        // TODO: DI of grid lengths based on which storage to check for
        _columns = storageMode == StorageMode.Shipping ? PlayerDataRepository.ShippingGridColumns : PlayerDataRepository.StorageGridColumns;
        _rows = storageMode == StorageMode.Shipping ? PlayerDataRepository.ShippingGridRows : PlayerDataRepository.StorageGridRows;
        _tiles = new StorageTile[_columns, _rows];
        _CreateGrid(_columns, _rows, tileSize);

        // TODO: func to read from persistence storage and init packages
    }

    // Assume valid position
    public void MovePackage(PackageGO package, Vector2I packageTilePosition, int storageColumn, int storageRow) {
        Vector2I offset = packageTilePosition * _tileSize;
        package.Position = _tiles[storageColumn, storageRow]!.GlobalPosition - offset;
        // TODO: Save updated data 
    }

    private void _CreateGrid(int columns, int rows, Vector2I tileSize) {
        for (int row = 0; row < rows; row++) {
            for (int column = 0; column < columns; column++) {
                StorageTile tile = (_placeholderTile.Instantiate() as StorageTile)!;
                tile.Centered = false;
                tile.GetNode<Area2D>("Area2D").GetNode<CollisionShape2D>("CollisionShape2D").Position = tileSize / 2;
                _storageTilePositions[tile] = new Vector2I(column, row);
                _tiles[column, row] = tile;
                AddChild(tile);
                tile.Position = new Vector2I(column * tileSize.X, row * tileSize.Y);
                tile.Hovered += t => TileHovered?.Invoke(_mode, _storageTilePositions[t]);
            }
        }

        Area2D exitHitbox = new();
        exitHitbox.Position = new Vector2I(columns * tileSize.X, rows * tileSize.Y) / 2;
        AddChild(exitHitbox);
        CollisionShape2D exitHitboxShape = new();
        exitHitbox.AddChild(exitHitboxShape);
        RectangleShape2D rectShape = new();
        rectShape.Size = new Vector2I(columns * tileSize.X, rows * tileSize.Y);
        exitHitboxShape.Shape = rectShape;
        exitHitbox.MouseExited += () => StorageUnhovered?.Invoke(_mode);
    }

    public void ResetHighlights() {
        foreach (StorageTile tile in _storageTilePositions.Keys) {
            tile.SetHighlight(StorageTile.Highlight.None);
        }
    }

    public void HighlightTiles(List<Vector2I> tilesToHighlight, StorageTile.Highlight highlight) {
        foreach (Vector2I position in tilesToHighlight) {
            if (!_IsInsideGrid(position, _columns, _rows)) {
                continue;
            }

            _tiles[position.X, position.Y]?.SetHighlight(highlight);
        }
    }

    public void ClearAllHighlights() {
        foreach (StorageTile tile in _storageTilePositions.Keys) {
            tile.SetHighlight(StorageTile.Highlight.None);
        }
    }

    public List<Vector2I> ComputeOverlappingTiles(Package package, Vector2I storageTilePosition, Vector2I packageTilePosition) {
        List<Vector2I> tiles = [];
        foreach (Vector2I position in package.Dimensions) {
            tiles.Add(storageTilePosition + position - packageTilePosition);
        }

        return tiles;
    }

    public bool PackagePositionIsValid(Package package, List<Vector2I> positionsToCheck) {
        int[,] grid = _mode == StorageMode.Shipping ? _playerDataRepository.ShippingGrid : _playerDataRepository.StorageGrid;

        foreach (Vector2I position in positionsToCheck) {
            if (!_IsInsideGrid(position, _columns, _rows)) {
                return false;
            }

            if (grid[position.X, position.Y] != 0 && grid[position.X, position.Y] != package.PackageId) {
                GD.Print($"Grid {position} is {grid[position.X, position.Y]}");
                return false;
            }
        }

        return true;
    }

    private bool _IsInsideGrid(Vector2I position, int columns, int rows) {
        return position.X >= 0 && position.X < columns && position.Y >= 0 && position.Y < rows;
    }
}