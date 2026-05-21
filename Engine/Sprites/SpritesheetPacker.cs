using Engine.Debugging;
using Engine.Utility;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Sprites;

public class SpritesheetPacker {

    private const int MAX_SIZE = 512;

    private static readonly Logger Logger = Logger.Get("Sprites");

    public static Spritesheet Pack(IEnumerable<TextureIdentifier> textures, GraphicsDevice graphicsDevice) {
        TextureIdentifier[] textureIdentifiers = [.. textures];
        int totalArea = textureIdentifiers.Sum(id => id.Texture.Width * id.Texture.Height);

        int size = 16;
        int finalSize = 0;
        while (size < MAX_SIZE) {
            if (size * size >= totalArea) {
                finalSize = size;
                break;
            }
            size *= 2;
        }

        if (finalSize == 0) {
            Logger.Error($"Failed to pack spritesheet. {textures.Count()} texture's area too big! {totalArea} > {MAX_SIZE * MAX_SIZE} ({MAX_SIZE} * {MAX_SIZE})");
            return null;
        }

        Spritesheet newSheet = new(graphicsDevice, finalSize, finalSize);
        Array.Sort(textureIdentifiers); // Sort by width

        foreach (var item in textureIdentifiers) {
            newSheet.BlitAndAddSprite(item.Texture, new(0, 0, item.Texture.Width, item.Texture.Height), item.Identifier);
        }

        return newSheet;
    }

    public struct TextureIdentifier(Texture2D texture, Identifier identifier) : IComparable<TextureIdentifier> {
        public Texture2D Texture = texture ?? throw new ArgumentNullException(nameof(texture));
        public Identifier Identifier = identifier;

        public readonly int CompareTo(TextureIdentifier other) {
            return other.Texture.Width - Texture.Width;
        }

    }

}
