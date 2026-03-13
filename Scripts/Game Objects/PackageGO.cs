using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PackageGO : Node2D {
    public enum Opacity {
        Full,
        Half
    }

    public event Action<PackageGO, bool, Vector2I> Hovered;

    public List<Vector2I> HitboxPositions { get; private set; } = [];

    [Export]
    private Texture2D _placeholderTile;

    private readonly Dictionary<Area2D, Vector2I> _hitboxes = [];
    private Sprite2D _sprite;

    public override void _Ready() {
        _sprite = new Sprite2D();
        AddChild(_sprite);
        Initialize("READY");
    }

    public void Initialize(string context) {
        // GD.Print($"Initialize called from {context}.");
        if (HitboxPositions is null || HitboxPositions.Count == 0) {
            // GD.Print($"Hitbox positions is null or empty. Ending Initialize");
            return;
        }

        // GD.Print($"Hitbox positions count: {HitboxPositions.Count}");
        foreach (Vector2I pos in HitboxPositions) {
            Area2D area = new();
            AddChild(area);
            area.Position = new Vector2I(TileInfo.TileSize * pos.X, TileInfo.TileSize * pos.Y) + new Vector2I(TileInfo.TileSize, TileInfo.TileSize) / 2;
            CollisionShape2D shape = new();
            area.AddChild(shape);
            RectangleShape2D rectShape = new();
            rectShape.Size = new Vector2(32, 32);
            shape.Shape = rectShape;
            _hitboxes.Add(area, pos);
        }

        foreach (KeyValuePair<Area2D, Vector2I> kvp in _hitboxes) {
            kvp.Key.MouseEntered += () => Hovered?.Invoke(this, true, _hitboxes[kvp.Key]);
            kvp.Key.MouseExited += () => Hovered?.Invoke(this, false, _hitboxes[kvp.Key]);
        }
    }
    
    public void SetHitBoxAndRotateSprite(List<Vector2I> positions, PackageRotation packageRotation) {
        _SetHitboxPositions(positions);   
        _RotateSprite(packageRotation);
    }
    
    // New hitbox positions must be set first
    private void _RotateSprite(PackageRotation packageRotation) {
        _sprite.Rotation = Mathf.DegToRad(packageRotation switch {
            PackageRotation.Zero => 0,
            PackageRotation.Ninety => 90,
            PackageRotation.OneEighty => 180,
            PackageRotation.TwoSeventy => 270
        });
        _sprite.Position = _ComputeSpritePosition();
    }

    private void _SetHitboxPositions(List<Vector2I> positions) {
        HitboxPositions = positions;
        int i = 0;
        foreach (Area2D area in _hitboxes.Keys) {
            Vector2I pos = positions[i++];
            _hitboxes[area] = pos;
            area.Position = new Vector2I(TileInfo.TileSize * pos.X, TileInfo.TileSize * pos.Y) + new Vector2I(TileInfo.TileSize, TileInfo.TileSize) / 2;
        }
    }

    // Cannot set sprite as centered because it skews the rotation
    // Hitbox positions must be set first
    public void SetTexture(Texture2D texture) {
        _sprite.Texture = texture;
        GD.Print($"Sprite positioned at {_ComputeSpritePosition()}.");
        _sprite.Position = _ComputeSpritePosition();
    }

    private Vector2I _ComputeSpritePosition() {
        int maxX = HitboxPositions.Max(pos => pos.X);
        int maxY = HitboxPositions.Max(pos => pos.Y);
        return new Vector2I(maxX * TileInfo.TileSize, maxY * TileInfo.TileSize) / 2 + new Vector2I(TileInfo.TileSize, TileInfo.TileSize) / 2;
    }

    public void SetOpacity(Opacity opacity) {
        Modulate = new Color(1, 1, 1, opacity == Opacity.Full ? 1 : .5f);
    }
}