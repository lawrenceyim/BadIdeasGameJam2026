using System.Collections.Generic;
using RepositorySystem;

public class PlayerDataRepository : IRepository {
    public List<Package> Packages = [];
    public List<Package> PackagesToStore = [];
    public List<Package> PackagesToShip = [];
}