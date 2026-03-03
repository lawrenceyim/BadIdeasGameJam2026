#nullable enable
using Godot;
using System;
using System.Collections.Generic;

public partial class PackageStorage : Node2D {
    [Export]
    private PackedScene _placeholderTile = null!;

    private readonly Dictionary<StorageTile, Vector2I> _tilePositions = [];

    private int _rows = 5;
    private int _columns = 5;
    private int _xOffset = 32;
    private int _yOffset = 32;

    private Sprite2D?[,] _tiles = null!;
    private StorageTile? _currentTile;


    public override void _Ready() {
        _tiles = new Sprite2D[_rows, _columns];

        for (int row = 0; row < _rows; row++) {
            for (int column = 0; column < _columns; column++) {
                StorageTile tile = (_placeholderTile.Instantiate() as StorageTile)!;
                _tilePositions[tile] = new Vector2I(row, column);
                _tiles[row, column] = tile;
                AddChild(tile);
                tile.Position = new Vector2(column * _xOffset, row * _yOffset);
                tile.Hovered += _HandleHover;
            }
        }
    }

    private void _HandleHover(StorageTile tile, bool hovered) {
        if (hovered) {
            _currentTile = tile;
            GD.Print($"New hovered tile. {_tilePositions[tile]}");
            return;
        }

        if (_currentTile == tile) {
            GD.Print("No hovered tile");
            _currentTile = null;
        }
    }
}