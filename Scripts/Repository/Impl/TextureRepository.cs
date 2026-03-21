using Godot;
using Godot.Collections;
using RepositorySystem;

public partial class TextureRepository : Node, IAutoload, IRepository {
    public static string AutoloadPath => "/root/TextureRepository";

    [Export]
    private Dictionary<TextureId, Texture2D> _textures;

    public Texture2D GetTexture(TextureId id) {
        return _textures[id];
    }
}

public enum TextureId {
    // Ingredients
    PlaceHolder = 0,
    PlaceHolderPackageMini1 = 100_001,
    PlaceHolderPackageMini2 = 100_002,
    PlaceHolderPackageMini3 = 100_003,
    PlaceHolderPackageMini4 = 100_004,
    PlaceHolderPackageMini5 = 100_005,
    PlaceHolderPackageMini6 = 100_006,
    PlaceHolderPackageMini7 = 100_007,
    PlaceHolderPackageMini8 = 100_008,
    PlaceHolderPackageMini9 = 100_009,
    PlaceHolderPackageMini10 = 100_010,
    PlaceHolderPackageMini11 = 100_011,
    PlaceHolderPackageMini12 = 100_012,
    PlaceHolderPackageMini13 = 100_013,
    PlaceHolderPackageMini14 = 100_014,
    PlaceHolderPackageMini15 = 100_015,
}