using System.Collections.Generic;
using Godot;

public class StorageUtils {
    public static List<int[,]> StorageGrids { get; } = [
        PlayerDataRepository.HoldingGrid,
        PlayerDataRepository.ShippingGrid,
        PlayerDataRepository.StorageGrid,
    ];

    public static List<Dictionary<Package, Vector2>> Storages { get; } = [
        PlayerDataRepository.PackagesInHolding,
        PlayerDataRepository.PackagesInShipping,
        PlayerDataRepository.PackagesInStorage,
    ];

    public static int[,] GetStorageGrid(PackageStorage.StorageMode mode) {
        return mode switch {
            PackageStorage.StorageMode.Holding => PlayerDataRepository.HoldingGrid,
            PackageStorage.StorageMode.Shipping => PlayerDataRepository.ShippingGrid,
            PackageStorage.StorageMode.Storage => PlayerDataRepository.StorageGrid,
        };
    }

    public static Dictionary<Package, Vector2> GetPackageDict(PackageStorage.StorageMode mode) {
        return mode switch {
            PackageStorage.StorageMode.Holding => PlayerDataRepository.PackagesInHolding,
            PackageStorage.StorageMode.Shipping => PlayerDataRepository.PackagesInShipping,
            PackageStorage.StorageMode.Storage => PlayerDataRepository.PackagesInStorage,
        };
    }

    public static int GetStorageColumns(PackageStorage.StorageMode mode) {
        return mode switch {
            PackageStorage.StorageMode.Holding => PlayerDataRepository.HoldingGridColumns,
            PackageStorage.StorageMode.Shipping => PlayerDataRepository.ShippingGridColumns,
            PackageStorage.StorageMode.Storage => PlayerDataRepository.StorageGridColumns,
        };
    }

    public static int GetStorageRows(PackageStorage.StorageMode mode) {
        return mode switch {
            PackageStorage.StorageMode.Holding => PlayerDataRepository.HoldingGridRows,
            PackageStorage.StorageMode.Shipping => PlayerDataRepository.ShippingGridRows,
            PackageStorage.StorageMode.Storage => PlayerDataRepository.StorageGridRows,
        };
    }
}