using System.Collections.Generic;
using RepositorySystem;

public class PlayerDataRepository : IRepository {
    public List<Package> Packages { get; } = [];
    public int[,] StorageGrid { get; } = new int[10, 10];
}