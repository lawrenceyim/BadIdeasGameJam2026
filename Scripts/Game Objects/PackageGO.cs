using Godot;
using System;
using System.Collections.Generic;

public partial class PackageGO : Sprite2D {
    public enum Opacity {
        Full,
        Half
    }

    public event Action<PackageGO, bool, Vector2I> Hovered;

    [Export]
    public Vector2[] HitboxPositions { get; private set; } = [];

    [Export]
    private Texture2D _placeholderTile;

    private readonly Dictionary<Area2D, Vector2I> _hitboxes = [];
    public Dictionary<Vector2I, Sprite2D> TileSprites { get; set; }

    public override void _Ready() {
        GD.Print("PackageGO Ready");
        Initialize("READY");
    }

    public void Initialize(string context) {
        GD.Print($"Initialize called from {context}.");
        if (HitboxPositions is null || HitboxPositions.IsEmpty()) {
            GD.Print($"Hitbox positions is null or empty. Ending Initialize");
            return;
        }

        GD.Print($"Hitbox positions count: {HitboxPositions.Length}");
        foreach (Vector2I pos in HitboxPositions) {
            Area2D area = new();
            AddChild(area);
            area.Position = new Vector2I(32 * pos.X + 16, 32 * pos.Y + 16);
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

    public void SetHitboxPositions(Vector2[] positions) {
        HitboxPositions = positions;
    }

    public void SetTileSprites(Dictionary<Vector2I, Sprite2D> tiles) {
        TileSprites = tiles;
    }

    public void SetOpacity(Opacity opacity) {
        SelfModulate = new Color(1, 1, 1, opacity == Opacity.Full ? 1 : .5f);
    }
}