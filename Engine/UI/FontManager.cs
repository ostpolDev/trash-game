using Engine.Debugging;
using FontStashSharp;
using System.IO;

namespace Engine.UI;

public class FontManager {

    private static readonly Logger Logger = Logger.Get("Fonts");

    public static FontSystem FontSystem { get; private set; }
    public static bool IsInitialized { get { return FontSystem != null; } }

    public static void LoadContent() {
        FontSystem = new();
    }

    public static void RegisterFont(string fontPath) {
        if (!File.Exists(fontPath)) {
            throw new FileNotFoundException("Font file could not be found", fontPath);
        }

        FontSystem.AddFont(File.ReadAllBytes(fontPath));
        Logger.Info($"Loaded font: {Path.GetFileName(fontPath)}");
    }

    public static async void RegisterFontAsync(string fontPath) {
        if (!File.Exists(fontPath)) {
            throw new FileNotFoundException("Font file could not be found", fontPath);
        }

        byte[] bytes = await File.ReadAllBytesAsync(fontPath);
        FontSystem.AddFont(bytes);
        Logger.Info($"Loaded font: {Path.GetFileName(fontPath)}");
    }

}
