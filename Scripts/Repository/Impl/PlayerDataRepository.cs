using System.Collections.Generic;
using RepositorySystem;

public class PlayerDataRepository : IRepository {
    public static int StorageGridLength = 10;
    public static int ShippingGridColumns = 5;
    public static int ShippingGridRows = 6;
    public List<Package> Packages { get; } = [];
    public int[,] StorageGrid { get; } = new int[StorageGridLength, StorageGridLength];
    public int[,] ShippingGrid { get; } = new int[ShippingGridColumns, ShippingGridRows];
}