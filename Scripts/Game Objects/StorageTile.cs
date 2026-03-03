using System;
using Godot;

public partial class StorageTile : Sprite2D {
    public event Action<StorageTile, bool> Hovered;

    [Export]
    private Area2D _hitbox;

    public override void _Ready() {
        _hitbox.MouseEntered += () => Hovered?.Invoke(this, true);
        _hitbox.MouseExited += () => Hovered?.Invoke(this, false);
    }
}