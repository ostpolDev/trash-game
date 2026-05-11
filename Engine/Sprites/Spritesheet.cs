using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Sprites;

public class Spritesheet(Texture2D texture, int spriteWidth = 0, int spriteHeight = 0) {

    public readonly Texture2D Texture = texture;
    public int Width { get { return Texture.Width; } }
    public int Height { get { return Texture.Height; } }

    public readonly int SpriteWidth = spriteWidth;
    public readonly int SpriteHeight = spriteHeight;

    public (int, int) ConvertCoordinates(int i) {
        return (i % Width, i / Width);
    }

    public int ConvertCoordinates(int x, int y) {
        return (y * Width) + x;
    }

    public Rectangle GetRectangleForSprite(int i) {
        (int x, int y) = ConvertCoordinates(i);
        return GetRectangleForSprite(ConvertCoordinates(x, y));
    }

    public Rectangle GetRectangleForSprite(int x, int y) {
        return new Rectangle(x, y, SpriteWidth, SpriteHeight);
    }

    public static Rectangle GetRectangleForSprite(int x, int y, int w, int h) {
        return new Rectangle(x, y, w, h);
    }

    public Rectangle GetRectangleForSprite(int i, int w, int h) {
        (int x, int y) = ConvertCoordinates(i);
        return GetRectangleForSprite(x, y, w, h);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destination, int i) {
        (int x, int y) = ConvertCoordinates(i);
        Draw(spriteBatch, destination, x, y, SpriteWidth, SpriteHeight);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destination, int x, int y) {
        Draw(spriteBatch, destination, x, y, SpriteWidth, SpriteHeight);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destination, int x, int y, int w, int h) {
        spriteBatch.Draw(Texture, destination, new Rectangle(x, y, w, h), Color.White);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destination, int i, int w, int h) {
        (int x, int y) = ConvertCoordinates(i);
        Draw(spriteBatch, destination, x, y, w, h);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destination) {
        spriteBatch.Draw(Texture, destination, Color.White);
    }

}
