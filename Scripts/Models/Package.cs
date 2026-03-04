using System.Collections.Generic;
using Godot;

public class Package {
    public int PackageId { get; }
    public TextureId TextureId { get; }
    public List<PackageLabel> RequiredLabels { get; } = [];
    public List<PackageLabel> GivenLabels { get; } = [];
    public List<Vector2I> Dimensions { get; } = [];
    public PackageRotation Rotation { get; set; } = PackageRotation.Zero;

    public Package(int packageId, TextureId textureId) {
        PackageId = packageId;
        TextureId = textureId;
    }
}