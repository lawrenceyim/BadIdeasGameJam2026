using System.Collections.Generic;
using Godot;

public class Package {
    private List<PackageLabel> _requiredLabels = [];
    private List<PackageLabel> _givenLabels = [];
    private List<Vector2I> _dimensions = [];
    private PackageRotation _rotation = PackageRotation.Zero;

    public void SetRequiredLabels(List<PackageLabel> labels) {
        _requiredLabels = labels;
    }

    public void SetDimensions(List<Vector2I> dimensions) {
        _dimensions = dimensions;
    }

    public void SetRotation(PackageRotation rotation) {
        _rotation = rotation;
    }

    public PackageRotation GetRotation() {
        return _rotation;
    }
}