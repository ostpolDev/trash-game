using Engine.Serialization;

namespace Engine.Sprites;

public static class SpritesheetExtensions {

    public static void WriteToFile(this Spritesheet spritesheet, string filePath) {
        SpritesheetSerializer.WriteToFile(spritesheet, filePath);
    }

}
