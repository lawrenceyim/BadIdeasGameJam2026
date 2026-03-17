using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

public partial class ShapeUtilsTest : Node {
    [Export]
    private Texture2D _tileTexture;

    public override void _Ready() {
        Dictionary<Vector2I, Vector2I> shape = new() {
            [new Vector2I(0, 0)] = new Vector2I(0, 0),
            [new Vector2I(1, 0)] = new Vector2I(1, 0),
            [new Vector2I(1, 1)] = new Vector2I(1, 1),
            [new Vector2I(1, 2)] = new Vector2I(1, 2),
        };

        for (int i = 0; i < 4; i++) {
            GD.Print($"Rotated CW {i + 1} times");
            shape = ShapeUtils.RotateCw(shape.Values.ToList());
            PrintShape(shape);
        }


        for (int i = 0; i < 4; i++) {
            GD.Print($"Rotated CWW {i + 1} times");
            shape = ShapeUtils.RotateCcw(shape.Values.ToList());
            PrintShape(shape);
        }

        PackageGO go = PackageGoUtils.GenerateShape(this, shape.Values.ToList(), _tileTexture, PackageOrientation.Up);
    }

    void PrintShape(Dictionary<Vector2I, Vector2I> shape) {
        HashSet<Vector2I> tiles = new(shape.Values.ToArray());

        int minX = shape.Values.Min(p => p.X);
        int maxX = shape.Values.Max(p => p.X);
        int minY = shape.Values.Min(p => p.Y);
        int maxY = shape.Values.Max(p => p.Y);

        int width = maxX - minX + 1;
        int height = maxY - minY + 1;

        for (int y = 0; y < height; y++) {
            StringBuilder sb = new();

            for (int x = 0; x < width; x++) {
                Vector2I pos = new(x + minX, y + minY);
                sb.Append(tiles.Contains(pos) ? "O" : "X");
            }

            GD.Print(sb.ToString());
        }
    }
}