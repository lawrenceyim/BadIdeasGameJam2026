using System.Collections.Generic;
using Godot;
using RepositorySystem;

public class PlayerDataRepository : IRepository {
    public static int HoldingGridColumns = 4;
    public static int HoldingGridRows = 3;
    public static int ShippingGridColumns = 6;
    public static int ShippingGridRows = 9;
    public static int StorageGridColumns = 8;
    public static int StorageGridRows = 9;
    public static Dictionary<Package, Vector2> PackagesInStorage { get; } = [];
    public static Dictionary<Package, Vector2> PackagesInShipping { get; } = [];
    public static Dictionary<Package, Vector2> PackagesInHolding { get; } = [];
    public static int[,] HoldingGrid { get; } = new int[HoldingGridColumns, HoldingGridRows];
    public static int[,] ShippingGrid { get; } = new int[ShippingGridColumns, ShippingGridRows];
    public static int[,] StorageGrid { get; } = new int[StorageGridColumns, StorageGridRows];
}