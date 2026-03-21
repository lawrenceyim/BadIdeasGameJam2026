using System;
using System.Collections.Generic;
using Godot;

public class PackageUtil {
    private static Random _rng = new Random();

    private static readonly Dictionary<int, List<Vector2I>> _packageDimensions = new() {
        {
            1,
            [
                new Vector2I(0, 0),
                new Vector2I(0, 1),
                new Vector2I(1, 0),
                new Vector2I(1, 1),
                new Vector2I(2, 0),
                new Vector2I(2, 1),
                new Vector2I(3, 0),
                new Vector2I(3, 1)
            ]
        }, {
            2,
            [
                new Vector2I(0, 0),
                new Vector2I(0, 1),
                new Vector2I(1, 0),
                new Vector2I(1, 1),
                new Vector2I(2, 0),
                new Vector2I(2, 1)
            ]
        }, {
            3,
            [
                new Vector2I(0, 1),
                new Vector2I(1, 0),
                new Vector2I(1, 1),
                new Vector2I(2, 1)
            ]
        }, {
            4,
            [
                new Vector2I(0, 0),
                new Vector2I(0, 1),
                new Vector2I(0, 2),
                new Vector2I(0, 3)
            ]
        }, {
            5,
            [
                new Vector2I(0, 0),
                new Vector2I(0, 1),
                new Vector2I(1, 0),
                new Vector2I(1, 1)
            ]
        }, {
            6,
            [
                new Vector2I(0, 0),
                new Vector2I(1, 0),
                new Vector2I(1, 1),
                new Vector2I(1, 2)
            ]
        }, {
            7,
            [
                new Vector2I(0, 0),
                new Vector2I(0, 1),
                new Vector2I(1, 1),
                new Vector2I(1, 2)
            ]
        }, {
            8,
            [
                new Vector2I(0, 0),
                new Vector2I(0, 1),
                new Vector2I(0, 2)
            ]
        }, {
            9,
            [
                new Vector2I(0, 1),
                new Vector2I(1, 0),
                new Vector2I(1, 1)
            ]
        }, {
            10,
            [
                new Vector2I(0, 0),
                new Vector2I(0, 1)
            ]
        }, {
            11,
            [
                new Vector2I(0, 2),
                new Vector2I(1, 1),
                new Vector2I(1, 2),
                new Vector2I(2, 0),
                new Vector2I(2, 1)
            ]
        }, {
            12,
            [
                new Vector2I(0, 0),
                new Vector2I(1, 0),
                new Vector2I(2, 0),
                new Vector2I(3, 0)
            ]
        }, {
            13,
            [
                new Vector2I(0, 0),
                new Vector2I(0, 1),
                new Vector2I(0, 2),
                new Vector2I(1, 2)
            ]
        }, {
            14,
            [
                new Vector2I(0, 0),
                new Vector2I(0, 1),
                new Vector2I(0, 2),
                new Vector2I(0, 3),
                new Vector2I(1, 2),
                new Vector2I(1, 3)
            ]
        }, {
            15,
            [
                new Vector2I(0, 0)
            ]
        },
    };

    public static int GenerateRandomPackageId() {
        return _rng.Next(1, 16);
    }

    public static TextureId GetPlaceholderPackageMiniId(int id) {
        return id switch {
            1 => TextureId.PlaceHolderPackageMini1,
            2 => TextureId.PlaceHolderPackageMini2,
            3 => TextureId.PlaceHolderPackageMini3,
            4 => TextureId.PlaceHolderPackageMini4,
            5 => TextureId.PlaceHolderPackageMini5,
            6 => TextureId.PlaceHolderPackageMini6,
            7 => TextureId.PlaceHolderPackageMini7,
            8 => TextureId.PlaceHolderPackageMini8,
            9 => TextureId.PlaceHolderPackageMini9,
            10 => TextureId.PlaceHolderPackageMini10,
            11 => TextureId.PlaceHolderPackageMini11,
            12 => TextureId.PlaceHolderPackageMini12,
            13 => TextureId.PlaceHolderPackageMini13,
            14 => TextureId.PlaceHolderPackageMini14,
            15 => TextureId.PlaceHolderPackageMini15
        };
    }

    public static List<Vector2I> GetDimensions(int id) {
        return _packageDimensions[id];
    }
}