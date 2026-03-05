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
}