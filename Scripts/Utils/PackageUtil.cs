using System;
using System.Collections.Generic;
using Godot;

public class PackageUtil {
    private static Random _rng = new Random();

    #region Package Data

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

    private static readonly int[] Weights = {
        5, 7, 5, 9, 12,
        5, 13, 6, 3, 7,
        13, 15, 8, 15, 20
    };

    private static readonly TextureId[] PackageTextureId = {
        TextureId.PlaceHolderPackageMini1,
        TextureId.PlaceHolderPackageMini2,
        TextureId.PlaceHolderPackageMini3,
        TextureId.PlaceHolderPackageMini4,
        TextureId.PlaceHolderPackageMini5,
        TextureId.PlaceHolderPackageMini6,
        TextureId.PlaceHolderPackageMini7,
        TextureId.PlaceHolderPackageMini8,
        TextureId.PlaceHolderPackageMini9,
        TextureId.PlaceHolderPackageMini10,
        TextureId.PlaceHolderPackageMini11,
        TextureId.PlaceHolderPackageMini12,
        TextureId.PlaceHolderPackageMini13,
        TextureId.PlaceHolderPackageMini14,
        TextureId.PlaceHolderPackageMini15
    };

    #endregion

    public static int GenerateRandomPackageId() {
        return _rng.Next(1, 16);
    }

    public static TextureId GetPlaceholderPackageMiniId(int id) {
        return PackageTextureId[id - 1];
    }

    public static List<Vector2I> GetDimensions(int id) {
        return _packageDimensions[id];
    }

    public static int GetWeight(int key) {
        return Weights[key];
    }
}