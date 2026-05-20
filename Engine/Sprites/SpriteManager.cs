using Engine.Debugging;
using Engine.Utility;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Sprites;

public class SpriteManager {

    private static readonly Logger Logger = Logger.Get("Sprites");

    private readonly List<Spritesheet> Sheets = [];
    private readonly Dictionary<Identifier, Spritesheet> SpriteToSheetLookup = [];

    private ContentManager ContentManager;

    public void LoadContent(ContentManager contentManager, IEnumerable<string> sheetPaths) {
        ContentManager = contentManager;
        foreach (string item in sheetPaths) {
            LoadSpritesheetFromPath(item);
        }
    }

    private Spritesheet LoadSpritesheetFromPath(string path) {
        try {

            Spritesheet sheet = Spritesheet.FromFile(path, BaseGame.Instance.GraphicsDevice);
            Sheets.Add(sheet);

            foreach (var item in sheet.SpriteLookup) {
                SpriteToSheetLookup[item.Key] = sheet;
            }

            return sheet;

        } catch (System.Exception ex) {
            Logger.Exception(ex);
            return null;
        }
    }

    public void UnloadSheet(string uid) {
        Spritesheet sheet = Sheets.Single(sheet => sheet.UID == uid);
        if (sheet == null) {
            Logger.Warning($"Cannot unload spritesheet {uid} as it does not seem to be loaded");
            return;
        }

        ContentManager.UnloadAsset(sheet.Texture.Name);
        Sheets.Remove(sheet);
    }

    public Spritesheet LoadSheet(string filePath) {
        return LoadSpritesheetFromPath(filePath);
    }

    public Spritesheet GetLoadedById(string id) {
        return Sheets.Single(sheet => sheet.UID == id);
    }

    public bool IsSheetLoaded(string id) {
        return GetLoadedById(id) != null;
    }

    public Sprite GetSprite(Identifier identifier) {
        if (!SpriteToSheetLookup.TryGetValue(identifier, out Spritesheet sheet)) {
            return null;
        }
        return sheet.Get(identifier);
    }

    public bool HasSprite(Identifier identifier) {
        return SpriteToSheetLookup.ContainsKey(identifier);
    }

}
