using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Sprites;

public struct Sprite(Spritesheet sheet, Rectangle rectangle) {

    public readonly Spritesheet Sheet = sheet;
    public Rectangle SourceRectangle { get; private set; } = rectangle;

    public readonly void Draw(SpriteBatch spriteBatch, Rectangle destination) {
        Sheet.Draw(spriteBatch, destination, SourceRectangle);
    }

    public readonly void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color) {
        Sheet.Draw(spriteBatch, destination, SourceRectangle, color);
    }

    public void MoveUV(int x, int y) {
        MoveUV(new Rectangle(x, y, SourceRectangle.Width, SourceRectangle.Height));
    }

    public void MoveUV(int x, int y, int width, int height) {
        MoveUV(new Rectangle(x, y, width, height));
    }

    public void MoveUV(Rectangle rect) {
        SourceRectangle = rect;
    }

}
