using System.Collections.Generic;
using Godot;

public class PackageGoUtils {
    public static PackageGO GenerateShape(Node parent, List<Vector2I> coords, Texture2D texture, PackageRotation rotation) {
        PackageGO package = new();
        parent.AddChild(package);

        // Dictionary<Vector2I, Sprite2D> sprites = [];
        //
        // foreach (Vector2I coord in coords) {
        //     Sprite2D sprite = new();
        //     sprite.Centered = false;
        //     sprite.Texture = texture;
        //     sprite.Position = coord * 32;
        //     package.AddChild(sprite);
        //     sprites[coord] = sprite;
        // }

        package.SetHitBoxAndRotateSprite(coords, rotation);
        // package.SetTileSprites(sprites);
        package.Initialize("Utils");
        package.SetTexture(texture);
        return package;
    }
}