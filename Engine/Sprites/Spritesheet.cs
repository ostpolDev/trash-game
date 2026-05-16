using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Sprites;

public class Spritesheet {

    public Texture2D Texture { get; private set; }
    public int Width { get { return Texture.Width; } }
    public int Height { get { return Texture.Height; } }
    public bool CanDraw { get { return Texture != null; } }

    public readonly int SpriteWidth;
    public readonly int SpriteHeight;

    private readonly string TexturePath;

    public Spritesheet(Texture2D texture, int spriteWidth = 0, int spriteHeight = 0) {
        Texture = texture;
        SpriteWidth = spriteWidth;
        SpriteHeight = spriteHeight;
    }

    public Spritesheet(string path, int spriteWidth = 0, int spriteHeight = 0) {
        SpriteWidth = spriteWidth;
        SpriteHeight = spriteHeight;
        TexturePath = path;
        if (BaseGame.HasLoadedContent) {
            Texture = BaseGame.Instance.Content.Load<Texture2D>(path);
        } else {
            BaseGame.Instance.OnLoadContent += Instance_OnLoadContent;
        }
    }

    public Sprite CreateSprite(Rectangle rectangle) {
        return new(this, rectangle);
    }

    public Sprite CreateSprite(int i) {
        return new(this, GetRectangleForSprite(i));
    }

    public Sprite CreateSprite(int x, int y) {
        return new(this, GetRectangleForSprite(x, y));
    }

    public Sprite CreateSprite(int x, int y, int width, int height) {
        return new(this, new Rectangle(x, y, width, height));
    }

    private void Instance_OnLoadContent(object sender, Events.LoadContentEventArgs e) {
        Texture = e.ContentManager.Load<Texture2D>(TexturePath);
        BaseGame.Instance.OnLoadContent -= Instance_OnLoadContent;
    }

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

    public void Draw(SpriteBatch spriteBatch, Rectangle destination, Rectangle source) {
        spriteBatch.Draw(Texture, destination, source, Color.White);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destination, Rectangle source, Color color) {
        spriteBatch.Draw(Texture, destination, source, color);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destination, int i, int w, int h) {
        (int x, int y) = ConvertCoordinates(i);
        Draw(spriteBatch, destination, x, y, w, h);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destination) {
        spriteBatch.Draw(Texture, destination, Color.White);
    }

}
