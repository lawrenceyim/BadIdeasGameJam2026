using System.Collections.Generic;
using RepositorySystem;

public class PlayerDataRepository : IRepository {
    public List<Package> Packages { get; } = [];
    public List<Package> PackagesToStore { get; } = [];
    public List<Package> PackagesToShip { get; } = [];
    public int[,] StorageGrid { get; } = new int[10, 10];
}