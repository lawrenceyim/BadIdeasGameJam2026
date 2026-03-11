public class StorageUtils {
    public static int[,] GetStorageGrid(PackageStorage.StorageMode mode) {
        return mode switch {
            PackageStorage.StorageMode.Holding => PlayerDataRepository.HoldingGrid,
            PackageStorage.StorageMode.Shipping => PlayerDataRepository.ShippingGrid,
            PackageStorage.StorageMode.Storage => PlayerDataRepository.StorageGrid,
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