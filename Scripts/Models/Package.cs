using System.Collections.Generic;
using Godot;

public class Package {
    public int PackageId { get; }
    public TextureId TextureId { get; }
    public List<Vector2I> Dimensions { get; set; }
    public List<PackageLabel> RequiredLabels { get; } = [];
    public List<PackageLabel> GivenLabels { get; } = [];
    public int Weight { get; set; }
    public PackageOrientation Orientation { get; set; } = PackageOrientation.Up;

    public Package(int packageId, TextureId textureId, List<Vector2I> dimensions, int weight) {
        PackageId = packageId;
        TextureId = textureId;
        Dimensions = dimensions;
        Weight = weight;
    }
}