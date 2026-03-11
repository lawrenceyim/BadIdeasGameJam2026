using System.Collections.Generic;
using RepositorySystem;

public class PlayerDataRepository : IRepository {
    public static int StorageGridColumns = 8;
    public static int StorageGridRows = 9;
    public static int ShippingGridColumns = 6;
    public static int ShippingGridRows = 9;
    public List<Package> Packages { get; } = [];
    public int[,] StorageGrid { get; } = new int[StorageGridColumns, StorageGridRows];
    public int[,] ShippingGrid { get; } = new int[ShippingGridColumns, ShippingGridRows];
}