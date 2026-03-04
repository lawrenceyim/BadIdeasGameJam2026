using Godot;
using System;
using System.Collections.Generic;

public partial class PackageGO : Sprite2D {
	[Export]
	public Vector2[] HitboxPositions { get; private set; } = [];

	private readonly Dictionary<Area2D, Vector2I> _hitboxes = [];
	public event Action<PackageGO, bool, Vector2I> Hovered;

	public override void _Ready() {
		foreach (Vector2I pos in HitboxPositions) {
			Area2D area = new();
			AddChild(area);
			area.Position = new Vector2I(32 * pos.X, 32 * pos.Y);
			CollisionShape2D shape = new CollisionShape2D();
			area.AddChild(shape);
			RectangleShape2D rectShape = new RectangleShape2D();
			rectShape.Size = new Vector2(32, 32);
			shape.Shape = rectShape;
			_hitboxes.Add(area, pos);
		}

		foreach (KeyValuePair<Area2D, Vector2I> kvp in _hitboxes) {
			kvp.Key.MouseEntered += () => Hovered?.Invoke(this, true, _hitboxes[kvp.Key]);
			kvp.Key.MouseExited += () => Hovered?.Invoke(this, false, _hitboxes[kvp.Key]);
		}
	}

	public enum Opacity {
		Full,
		Half
	}

	public void SetOpacity(Opacity opacity) {
		SelfModulate = new Color(1, 1, 1, opacity == Opacity.Full ? 1 : .5f);
	}
}
