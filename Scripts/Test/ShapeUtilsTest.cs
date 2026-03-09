using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

public partial class ShapeUtilsTest : Node {
    public override void _Ready() {
        List<Vector2I> shape = [
            new(0, 0),
            new(1, 0),
            new(1, 1),
            new(1, 2),
        ];

        PrintShape(shape);

        for (int i = 0; i < 4; i++) {
            GD.Print($"Rotated CW {i + 1} times");
            shape = ShapeUtils.RotateCw(shape);
            PrintShape(shape);
        }


        for (int i = 0; i < 4; i++) {
            GD.Print($"Rotated CWW {i + 1} times");
            shape = ShapeUtils.RotateCcw(shape);
            PrintShape(shape);
        }
    }

    void PrintShape(List<Vector2I> shape) {
        HashSet<Vector2I> tiles = new(shape);

        int minX = shape.Min(p => p.X);
        int maxX = shape.Max(p => p.X);
        int minY = shape.Min(p => p.Y);
        int maxY = shape.Max(p => p.Y);

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