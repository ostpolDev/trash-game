using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Sprites;

public class Sprite(Spritesheet sheet, Rectangle rectangle) {

    public readonly Spritesheet Sheet = sheet;
    public readonly Rectangle SourceRectangle = rectangle;

    public void Draw(SpriteBatch spriteBatch, Rectangle destination) {
        Sheet.Draw(spriteBatch, destination, SourceRectangle);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color) {
        Sheet.Draw(spriteBatch, destination, SourceRectangle, color);
    }

}
