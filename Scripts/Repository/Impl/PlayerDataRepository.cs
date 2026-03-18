using System.Collections.Generic;
using Godot;
using RepositorySystem;

public class PlayerDataRepository : IRepository {
    public enum LevelStage {
        NotStarted,
        Day,
        Night,
        Over,
    }

    public const int HoldingGridColumns = 4;
    public const int HoldingGridRows = 4;
    public const int ShippingGridColumns = 6;
    public const int ShippingGridRows = 9;
    public const int StorageGridColumns = 8;
    public const int StorageGridRows = 9;

    // Vector2 is the position
    public static Dictionary<Package, Vector2> PackagesInStorage { get; } = [];
    public static Dictionary<Package, Vector2> PackagesInShipping { get; } = [];
    public static Dictionary<Package, Vector2> PackagesInHolding { get; } = [];

    // PackageID is the grid cell value
    public static int[,] HoldingGrid { get; } = new int[HoldingGridColumns, HoldingGridRows];
    public static int[,] ShippingGrid { get; } = new int[ShippingGridColumns, ShippingGridRows];
    public static int[,] StorageGrid { get; } = new int[StorageGridColumns, StorageGridRows];

    public static TickTimer LevelTimer { get; } = new();
    public static int DayLength = Engine.PhysicsTicksPerSecond * 60;
    public static int NightLength = Engine.PhysicsTicksPerSecond * 60;
    public static bool DayStarted { get; set; } = false;
    public static bool NightStarted { get; set; } = false;
    public static int NewPackageId { get; set; } = 1;
    public static LevelStage CurrentStage { get; set; } = LevelStage.NotStarted;

    public static void StartDay() {
        if (!DayStarted) {
            DayStarted = true;
            LevelTimer.StartFixedTimer(false, DayLength);
        }
    }

    public static void StartNight() {
        if (!NightStarted) {
            DayStarted = true;
            LevelTimer.StartFixedTimer(false, NightLength);
        }
    }
}