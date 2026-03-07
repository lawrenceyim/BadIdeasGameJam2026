using System;
using Godot;

public partial class StorageTile : Sprite2D {
    public enum Highlight {
        Green,
        Red,
        None
    };

    public event Action<StorageTile> Hovered;

    [Export]
    private Area2D _hitbox;

    public override void _Ready() {
        _hitbox.MouseEntered += () => Hovered?.Invoke(this);
    }

    public void SetHighlight(Highlight highlight) {
        switch (highlight) {
            case Highlight.Green:
                SelfModulate = new Color(0.2f, 0.2f, 0.2f, .5f);
                break;
        }

        SelfModulate = highlight switch {
            Highlight.Green => new Color(0, 1, 0, .5f),
            Highlight.Red => new Color(1, 0, 0, .5f),
            Highlight.None => new Color(1, 1, 1, 1),
        };
    }
}