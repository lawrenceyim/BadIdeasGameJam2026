using System.Collections.Generic;
using System.Linq;
using Godot;

public class PackageGoUtils {
    public static PackageGO GenerateShape(Node parent, List<Vector2I> coords, Texture2D texture) {
        PackageGO package = new();
        parent.AddChild(package);

        int tileSize = 32;

        foreach (Vector2I coord in coords) {
            Sprite2D sprite = new();
            sprite.Texture = texture;
            sprite.Position = coord * 32;
            package.AddChild(sprite);
        }

        package.SetHitboxPositions(
            coords.Select(v => new Vector2(v.X, v.Y)).ToArray()
        );
        return package;
    }
}