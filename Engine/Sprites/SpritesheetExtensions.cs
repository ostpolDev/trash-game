using Engine.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Sprites;

public static class SpritesheetExtensions {

    public static void WriteToFile(this Spritesheet spritesheet, string filePath) {
        SpritesheetSerializer.WriteToFile(spritesheet, filePath);
    }

    public static Spritesheet CreateSpritesheet(this Texture2D texture, int spriteWidth, int spriteHeight) {
        return new(texture, null, spriteWidth, spriteHeight);
    }

}
