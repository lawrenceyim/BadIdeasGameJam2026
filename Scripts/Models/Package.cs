using System.Collections.Generic;
using Godot;

public class Package {
    public int PackageId { get; }
    public TextureId TextureId { get; }
    public List<Vector2I> Dimensions { get; }
    public List<PackageLabel> RequiredLabels { get; } = [];
    public List<PackageLabel> GivenLabels { get; } = [];
    public PackageRotation Rotation { get; set; } = PackageRotation.Zero;

    public Package(int packageId, TextureId textureId, List<Vector2I> dimensions) {
        PackageId = packageId;
        TextureId = textureId;
        Dimensions = dimensions;
    }
}