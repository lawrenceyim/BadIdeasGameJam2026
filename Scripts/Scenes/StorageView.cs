using System.Collections.Generic;
using System.Linq;
using Godot;
using InputSystem;
using ServiceSystem;

public partial class StorageView : Node2D, IInputState {
    [Export]
    private PackageStorage _holdingStorage;

    [Export]
    private PackageStorage _packageStorage;

    [Export]
    private PackageStorage _shippingStorage;

    // should put in Texture repo
    [Export]
    private Texture2D _placeholderPackageTileRed;

    [Export]
    private Texture2D _placeholder64By128Package;

    private readonly int NormalPackageZIndex = 5;
    private readonly int HoveredPackageZIndex = 10;
    private readonly Dictionary<PackageGO, Package> _packages = [];
    private PackageGO? _selectedPackage;
    private PackageStorage.StorageMode _selectedStorage = PackageStorage.StorageMode.None;
    private Vector2I _selectedStorageTile;
    private Vector2I _selectedPackageTile;
    private bool _draggingPackage = false;
    private Vector2 _positionBeforeDrag;
    private List<Vector2I> _hitboxesBeforeRotation = [];
    private PackageOrientation _orientationBeforeDrag = PackageOrientation.Up;
    private InputStateMachine _inputStateMachine;
    private SceneManager _sceneManager;


    public override void _Ready() {
        ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        _sceneManager = serviceLocator.GetService<SceneManager>();
        _inputStateMachine = serviceLocator.GetService<InputStateMachine>();
        _inputStateMachine.SetState(this);

        _holdingStorage.Initialize(TileInfo.TileSizeVector, PackageStorage.StorageMode.Holding);
        _packageStorage.Initialize(TileInfo.TileSizeVector, PackageStorage.StorageMode.Storage);
        _shippingStorage.Initialize(TileInfo.TileSizeVector, PackageStorage.StorageMode.Shipping);
        _holdingStorage.TileHovered += _TileHovered;
        _holdingStorage.StorageUnhovered += _StorageUnhovered;
        _packageStorage.TileHovered += _TileHovered;
        _packageStorage.StorageUnhovered += _StorageUnhovered;
        _shippingStorage.TileHovered += _TileHovered;
        _shippingStorage.StorageUnhovered += _StorageUnhovered;

        _CreatePlaceholderPackage();
    }

    public void ProcessInput(InputEventDto eventDto) {
        if (eventDto is KeyDto keyDto) {
            _HandleKeyPress(keyDto);
        }

        if (eventDto is MouseButtonPressedDto mouseButtonPressedDto && _selectedPackage != null) {
            _HandleMouseButtonPress(mouseButtonPressedDto);
        }

        if (eventDto is MouseButtonReleasedDto) {
            _HandleMouseButtonRelease();
        }

        if (eventDto is MouseMotionDto mouseMotion) {
            if (!_draggingPackage) {
                return;
            }

            if (_selectedPackage is null) {
                _draggingPackage = false;
                return;
            }

            _selectedPackage.Position += mouseMotion.Relative;
        }
    }

    private void _HandleMouseButtonPress(MouseButtonPressedDto mouseButtonPressedDto) {
        if (mouseButtonPressedDto.Identifier != "Left") {
            return;
        }

        _hitboxesBeforeRotation = _packages[_selectedPackage].Dimensions;
        _positionBeforeDrag = _selectedPackage.Position;
        _orientationBeforeDrag = _packages[_selectedPackage].Orientation;
        _selectedPackage.ZIndex = HoveredPackageZIndex;
        _selectedPackage.SetOpacity(PackageGO.Opacity.Half);
        _draggingPackage = true;
    }

    private void _HandleMouseButtonRelease() {
        if (_draggingPackage) {
            _selectedPackage?.SetOpacity(PackageGO.Opacity.Full);
            _selectedPackage.ZIndex = NormalPackageZIndex;
            _draggingPackage = false;
            _ClearAllHighlights();
            if (_selectedStorage is PackageStorage.StorageMode.None) {
                _SnapPackageBackToLastValidPosition();
                return;
            }

            PackageStorage storage = _GetPackageStorage(_selectedStorage);
            List<Vector2I> tiles = storage.ComputeOverlappingTiles(_packages[_selectedPackage], _selectedStorageTile, _selectedPackageTile);
            if (storage.PackagePositionIsValid(_packages[_selectedPackage], tiles)) {
                storage.MovePackage(_selectedPackage, _selectedPackageTile, _selectedStorageTile.X, _selectedStorageTile.Y);
                _RemovePackagePosition(_packages[_selectedPackage]);
                _SavePackagePosition(_packages[_selectedPackage], _selectedPackage, tiles, _selectedStorage);
                _UpdateStorageWeightDisplay();
            } else {
                _SnapPackageBackToLastValidPosition();
            }
        }
    }

    private void _SnapPackageBackToLastValidPosition() {
        Package package = _packages[_selectedPackage];
        _packages[_selectedPackage].Orientation = _orientationBeforeDrag;
        package.Dimensions = _hitboxesBeforeRotation;
        _selectedPackage.SetHitBoxAndRotateSprite(_hitboxesBeforeRotation, _orientationBeforeDrag);
        _selectedPackage.Position = _positionBeforeDrag;
    }

    private void _StorageUnhovered(PackageStorage.StorageMode mode) {
        _ClearAllHighlights();
        _selectedStorage = PackageStorage.StorageMode.None;
    }

    private void _TileHovered(PackageStorage.StorageMode mode, Vector2I tilePosition) {
        _selectedStorage = mode;
        _selectedStorageTile = tilePosition;

        if (!_draggingPackage) {
            return;
        }

        _HighlightTiles(_packages[_selectedPackage]);
    }

    private void _HighlightTiles(Package package) {
        _ClearAllHighlights();

        if (_selectedPackage is null || _selectedStorage == PackageStorage.StorageMode.None) {
            return;
        }

        PackageStorage storage = _GetPackageStorage(_selectedStorage);
        List<Vector2I> tilesToOccupy = storage.ComputeOverlappingTiles(package, _selectedStorageTile, _selectedPackageTile);
        bool isValidPosition = storage.PackagePositionIsValid(package, tilesToOccupy);
        storage.HighlightTiles(tilesToOccupy, isValidPosition ? StorageTile.Highlight.Green : StorageTile.Highlight.Red);
    }

    private void _HandleKeyPress(KeyDto keyDto) {
        if (keyDto.Pressed) {
            if (keyDto.Identifier == "Space") {
                _sceneManager.ChangeScene(SceneId.CustomerView, string.Empty);
            } else if (keyDto.Identifier == "Q") {
                _RotatePressed(false);
            } else if (keyDto.Identifier == "E") {
                _RotatePressed(true);
            }
        }
    }

    private void _RotatePressed(bool cw) {
        if (!_draggingPackage) {
            return;
        }

        _Rotate(_selectedPackage, cw);
        _HighlightTiles(_packages[_selectedPackage]);
    }

    private void _Rotate(PackageGO packageGo, bool clockWise) {
        Dictionary<Vector2I, Vector2I> previousToNewTiles = clockWise
            ? ShapeUtils.RotateCw(packageGo.HitboxPositions.Select(v => new Vector2I(v.X, v.Y)).ToList())
            : ShapeUtils.RotateCcw(packageGo.HitboxPositions.Select(v => new Vector2I(v.X, v.Y)).ToList());
        Package package = _packages[packageGo];
        package.Dimensions = previousToNewTiles.Values.ToList();
        // Add 4 to avoid a situation where rotation is 0, and turning CCW makes it -1, resulting in invalid enum
        package.Orientation = (PackageOrientation)(((int)package.Orientation + 4 + (clockWise ? 1 : -1)) % 4);
        packageGo.SetHitBoxAndRotateSprite(previousToNewTiles.Values.ToList(), package.Orientation);
    }

    private void _SavePackagePosition(Package package, PackageGO packageGo, List<Vector2I> tiles, PackageStorage.StorageMode storageMode) {
        int[,] grid = StorageUtils.GetStorageGrid(storageMode);
        foreach (Vector2I tile in tiles) {
            grid[tile.X, tile.Y] = package.PackageId;
        }

        Dictionary<Package, Vector2> storage = StorageUtils.GetPackageDict(storageMode);
        storage[package] = packageGo.Position;

        GD.Print($"Saved package {package.PackageId} in {storageMode} \n{string.Join("\n",
            Enumerable.Range(0, grid.GetLength(1))
                .Select(y => string.Join(" ",
                    Enumerable.Range(0, grid.GetLength(0))
                        .Select(x => grid[x, y])
                ))
        )}");
    }

    private void _RemovePackagePosition(Package package) {
        foreach (int[,] storage in StorageUtils.StorageGrids) {
            for (int x = 0; x < storage.GetLength(0); x++) {
                for (int y = 0; y < storage.GetLength(1); y++) {
                    if (storage[x, y] == package.PackageId)
                        storage[x, y] = 0;
                }
            }
        }

        foreach (Dictionary<Package, Vector2> storage in StorageUtils.Storages) {
            storage.Remove(package);
        }
    }

    private void _ClearAllHighlights() {
        _holdingStorage.ClearAllHighlights();
        _packageStorage.ClearAllHighlights();
        _shippingStorage.ClearAllHighlights();
    }

    private void _UpdateStorageWeightDisplay() {
        // TODO: update a UI
        int weightOfShipping = StorageUtils.GetTotalWeight(PackageStorage.StorageMode.Shipping);
        int weightOfStorage = StorageUtils.GetTotalWeight(PackageStorage.StorageMode.Storage);
    }

    private void _HandlePackageHover(PackageGO package, bool hovered, Vector2I tilePosition) {
        if (_draggingPackage) {
            return;
        }

        // GD.Print($"HandlePackageHover {_packages[package].PackageId} {hovered}");
        if (hovered) {
            // GD.Print($"Package hovered: {_packages[package].PackageId} {tilePosition}");
            _selectedPackage = package;
            _selectedPackageTile = tilePosition;
            return;
        }

        if (package == _selectedPackage && tilePosition == _selectedPackageTile) {
            _selectedPackage = null;
        }
    }

    private void _ExitStorageTile() {
        _selectedStorage = PackageStorage.StorageMode.None;
    }

    private void _InitPackages() {
        foreach (Dictionary<Package, Vector2> packages in StorageUtils.Storages) {
            foreach (KeyValuePair<Package, Vector2> kvp in packages) {
                // TODO: Create packages
            }
        }
    }

    private void _CreatePlaceholderPackage() {
        PackageGO one = PackageGoUtils.GenerateShape(
            this,
            [
                new Vector2I(0, 0),
                new Vector2I(0, 1),
                new Vector2I(0, 2),
                new Vector2I(0, 3),
                new Vector2I(1, 0),
                new Vector2I(1, 1),
                new Vector2I(1, 2),
                new Vector2I(1, 3)
            ],
            _placeholder64By128Package,
            PackageOrientation.Up);
        _AddPackageGo(one, 1, new Vector2I(300, 0));

        PackageGO two = PackageGoUtils.GenerateShape(
            this,
            [
                new Vector2I(0, 0),
                new Vector2I(0, 1),
                new Vector2I(0, 2),
                new Vector2I(0, 3),
                new Vector2I(1, 0),
                new Vector2I(1, 1),
                new Vector2I(1, 2),
                new Vector2I(1, 3)
            ],
            _placeholder64By128Package,
            PackageOrientation.Up);
        _AddPackageGo(two, 2, new Vector2I(300, 300));
    }

    private void _AddPackageGo(PackageGO packageGo, int packageId, Vector2I position) {
        packageGo.Position = position;
        _packages[packageGo] = new Package(packageId, TextureId.PlaceHolder, packageGo.HitboxPositions.Select(v => (Vector2I)v).ToList(), 10);
        packageGo.Hovered += _HandlePackageHover;
    }

    private PackageStorage _GetPackageStorage(PackageStorage.StorageMode storageMode) {
        return storageMode switch {
            PackageStorage.StorageMode.Holding => _holdingStorage,
            PackageStorage.StorageMode.Shipping => _shippingStorage,
            PackageStorage.StorageMode.Storage => _packageStorage,
        };
    }
}