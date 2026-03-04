using System.Collections.Generic;
using RepositorySystem;

public class PlayerDataRepository : IRepository {
    private const int StorageGridLength = 5;
    public List<Package> Packages { get; } = [];
    public int[,] StorageGrid { get; } = new int[StorageGridLength, StorageGridLength];
}