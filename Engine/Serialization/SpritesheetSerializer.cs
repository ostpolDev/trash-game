using Engine.Debugging;
using Engine.Sprites;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Engine.Serialization;

public class SpritesheetSerializer {

    public const string EXTENSION = ".stx";
    private static readonly Logger Logger = Logger.Get("Serialization");

    public static void WriteToFile(Spritesheet spritesheet, string filePath) {
        SerializableDictionary data = new();
        data.Put("sprite_width", spritesheet.SpriteWidth);
        data.Put("sprite_height", spritesheet.SpriteHeight);

        if (spritesheet.TexturePath != null)
            data.Put("texture_path", spritesheet.TexturePath);

        using FileStream stream = File.Create(filePath);
        SerializableDictionary.WriteToFile(stream, data);

        using BinaryWriter writer = new(stream);

        if (spritesheet.Texture == null) {
            writer.Write((byte)0);
            return;
        }
        writer.Write((byte)1);

        byte[] colors = new byte[spritesheet.Texture.Width * spritesheet.Texture.Height];
        spritesheet.Texture.GetData(colors);
        writer.Write(spritesheet.Texture.Width);
        writer.Write(spritesheet.Texture.Height);
        writer.Write(colors.Length);
        writer.Write(colors);
    }

    public static Spritesheet ReadFromFile(string filePath, GraphicsDevice graphicsDevice) {
        if (!File.Exists(filePath)) {
            Logger.Error($"Cannot read file at: {filePath}");
            return null;
        }

        using FileStream stream = File.OpenRead(filePath);
        SerializableDictionary data = SerializableDictionary.ReadFromFile(stream);

        using BinaryReader reader = new(stream);

        if (reader.ReadByte() == 0) {
            if (data.ContainsKey("texture_path")) {
                return new(data.GetString("texture_path"), data.GetInt("sprite_width"), data.GetInt("sprite_height"));
            } else {
                return new((Texture2D)null, data.GetInt("sprite_width"), data.GetInt("sprite_height"));
            }
        }

        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        int size = reader.ReadInt32();

        byte[] colors = reader.ReadBytes(size);

        Texture2D texture = new(graphicsDevice, width, height);
        texture.SetData(colors);

        return new(texture, data.GetInt("sprite_width"), data.GetInt("sprite_height"));
    }

}
