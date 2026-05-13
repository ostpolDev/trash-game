using Engine.Sprites;
using Engine.UI.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI;

public class SimpleUIComponent : AbstractUIComponent {

    public readonly Spritesheet Spritesheet;
    public Rectangle SourceRectangle;

    public SimpleUIComponent(Spritesheet spritesheet, Rectangle source, int x = 0, int y = 0, int w = 0, int h = 0, UIAnchorPosition anchorPosition = UIAnchorPosition.TOP_LEFT) : base(x, y, w, h, anchorPosition) {
        Spritesheet = spritesheet;
        SourceRectangle = source;
    }


    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {
        Spritesheet.Draw(spriteBatch, ScreenArea, SourceRectangle);
    }

    public void SetSourceRectangle(int x, int y, int width, int height) {
        SourceRectangle = new(x, y, width, height);
    }

    protected override void CreateDebugRenderer() {
        DebugRenderer = new SimpleRenderer(this);
    }

}
