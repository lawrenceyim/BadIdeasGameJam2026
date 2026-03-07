using System.Collections.Generic;
using System.Linq;
using Godot;
using InputSystem;
using RepositorySystem;
using ServiceSystem;

public partial class StorageView : Node2D, IInputState {
    private PlayerDataRepository _playerDataRepository;

    [Export]
    private PackageStorage _packageStorage;

    [Export]
    private PackageStorage _shippingStorage;

    [Export]
    private PackedScene _placeholderPackage;

    private readonly Dictionary<PackageGO, Package> _packages = [];
    private PackageGO? _selectedPackage;
    private PackageStorage.StorageMode _selectedStorage;
    private Vector2I _selectedStorageTile;
    private Vector2I _selectedPackageTile;
    private bool _draggingPackage = false;
    private Vector2 _positionBeforeDrag;

    public override void _Ready() {
        ServiceLocator serviceLocator = GetNode<ServiceLocator>(ServiceLocator.AutoloadPath);
        InputStateMachine inputStateMachine = serviceLocator.GetService<InputStateMachine>();
        inputStateMachine.SetState(this);
        RepositoryLocator repositoryLocator = serviceLocator.GetService<RepositoryLocator>();
        _playerDataRepository = repositoryLocator.GetRepository<PlayerDataRepository>(RepositoryName.PlayerData);

        _packageStorage.Position = new Vector2(-300, 0);
        _shippingStorage.Position = new Vector2(300, 0);
        _packageStorage.Initialize(new Vector2I(32, 32), PackageStorage.StorageMode.Storage);
        _shippingStorage.Initialize(new Vector2I(32, 32), PackageStorage.StorageMode.Shipping);
        _packageStorage.TileHovered += _TileHovered;
        _packageStorage.StorageUnhovered += _StorageUnhovered;
        _shippingStorage.TileHovered += _TileHovered;
        _shippingStorage.StorageUnhovered += _StorageUnhovered;

        _CreatePlaceholderPackage();
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

        _ClearAllHighlights();
        PackageStorage storage = mode == PackageStorage.StorageMode.Shipping ? _shippingStorage : _packageStorage;
        List<Vector2I> tilesToOccupy = storage.ComputeOverlappingTiles(_packages[_selectedPackage], _selectedStorageTile, _selectedPackageTile);
        bool isValidPosition = storage.PackagePositionIsValid(_packages[_selectedPackage], tilesToOccupy);
        storage.HighlightTiles(tilesToOccupy, isValidPosition ? StorageTile.Highlight.Green : StorageTile.Highlight.Red);
    }

    public void ProcessInput(InputEventDto eventDto) {
        if (eventDto is MouseButtonPressedDto && _selectedPackage != null) {
            _positionBeforeDrag = _selectedPackage.Position;
            _selectedPackage.SetOpacity(PackageGO.Opacity.Half);
            _draggingPackage = true;
        }

        if (eventDto is MouseButtonReleasedDto) {
            if (_draggingPackage) {
                _selectedPackage?.SetOpacity(PackageGO.Opacity.Full);
                _draggingPackage = false;
                _ClearAllHighlights();
                PackageStorage storage = _selectedStorage == PackageStorage.StorageMode.Shipping ? _shippingStorage : _packageStorage;
                List<Vector2I> tiles = storage.ComputeOverlappingTiles(_packages[_selectedPackage], _selectedStorageTile, _selectedPackageTile);
                if (storage.PackagePositionIsValid(_packages[_selectedPackage], tiles)) {
                    storage.MovePackage(_selectedPackage, _selectedPackageTile, _selectedStorageTile.X, _selectedStorageTile.Y);
                    _SavePackagePosition(_packages[_selectedPackage], tiles, _selectedStorage);
                } else {
                    _selectedPackage.Position = _positionBeforeDrag;
                }
            }
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

    private void _SavePackagePosition(Package package, List<Vector2I> tiles, PackageStorage.StorageMode storageMode) {
        int[,] grid = storageMode == PackageStorage.StorageMode.Shipping ? _playerDataRepository.ShippingGrid : _playerDataRepository.StorageGrid;
        foreach (Vector2I tile in tiles) {
            grid[tile.X, tile.Y] = package.PackageId;
        }

        GD.Print($"Saved package {package.PackageId}.\n{string.Join("\n",
            Enumerable.Range(0, grid.GetLength(0))
                .Select(y => string.Join(" ",
                    Enumerable.Range(0, grid.GetLength(1))
                        .Select(x => grid[x, y])
                ))
        )}");
    }

    private void _ClearAllHighlights() {
        _packageStorage.ClearAllHighlights();
        _shippingStorage.ClearAllHighlights();
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
        GD.Print("Exit storage");
        _selectedStorage = PackageStorage.StorageMode.None;
    }


    private void _CreatePlaceholderPackage() {
        PackageGO packageGo = (PackageGO)_placeholderPackage.Instantiate();
        packageGo.Position = new Vector2(300, 0);
        AddChild(packageGo);
        packageGo.Hovered += _HandlePackageHover;
        List<Vector2I> result = packageGo.HitboxPositions
            .Select(v => (Vector2I)v)
            .ToList();
        _packages[packageGo] = new Package(1, TextureId.PlaceHolder, result);

        PackageGO packageGo1 = (PackageGO)_placeholderPackage.Instantiate();
        packageGo1.Position = new Vector2(300, 300);
        AddChild(packageGo1);
        packageGo1.Hovered += _HandlePackageHover;
        List<Vector2I> result1 = packageGo1.HitboxPositions
            .Select(v => (Vector2I)v)
            .ToList();
        _packages[packageGo1] = new Package(2, TextureId.PlaceHolder, result);
    }
}