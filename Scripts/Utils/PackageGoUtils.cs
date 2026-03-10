using System.Collections.Generic;
using System.Linq;
using Godot;

public class PackageGoUtils {
    public static PackageGO GenerateShape(Node parent, List<Vector2I> coords, Texture2D texture) {
        PackageGO package = new();
        parent.AddChild(package);

        int tileSize = 32;
        Dictionary<Vector2I, Sprite2D> sprites = [];

        foreach (Vector2I coord in coords) {
            Sprite2D sprite = new();
            sprite.Centered = false;
            sprite.Texture = texture;
            sprite.Position = coord * 32;
            package.AddChild(sprite);
            sprites[coord] = sprite;
        }

        package.SetHitboxPositions(
            coords.Select(v => new Vector2(v.X, v.Y)).ToArray()
        );
        package.SetTileSprites(sprites);
        package.Initialize("Utils");
        return package;
    }
}