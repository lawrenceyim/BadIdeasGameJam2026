using System.Collections.Generic;
using System.Linq;
using Godot;

public class ShapeUtils {
    // Not sure if they should be reversed since Y is downward in Godot
    public static Dictionary<Vector2I, Vector2I> RotateCw(List<Vector2I> shape) {
        int minX = shape.Min(p => p.X);
        int minY = shape.Min(p => p.Y);
        int maxY = shape.Max(p => p.Y);

        int height = maxY - minY + 1;

        Dictionary<Vector2I, Vector2I> previousToNew = [];

        foreach (Vector2I p in shape) {
            int x = p.X - minX;
            int y = p.Y - minY;

            int newX = height - 1 - y;
            int newY = x;

            previousToNew[p] = new Vector2I(newX, newY);
        }

        return previousToNew;
    }

    public static Dictionary<Vector2I, Vector2I> RotateCcw(List<Vector2I> shape) {
        int minX = shape.Min(p => p.X);
        int maxX = shape.Max(p => p.X);
        int minY = shape.Min(p => p.Y);

        int width = maxX - minX + 1;

        Dictionary<Vector2I, Vector2I> previousToNew = [];

        foreach (Vector2I p in shape) {
            int x = p.X - minX;
            int y = p.Y - minY;

            int newX = y;
            int newY = width - 1 - x;

            previousToNew[p] = new Vector2I(newX, newY);
        }

        return previousToNew;
    }
}