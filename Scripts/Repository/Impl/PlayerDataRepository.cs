using System.Collections.Generic;
using RepositorySystem;

public class PlayerDataRepository : IRepository {
    public static int StorageGridLength = 10;
    public List<Package> Packages { get; } = [];
    public int[,] StorageGrid { get; } = new int[StorageGridLength, StorageGridLength];
}