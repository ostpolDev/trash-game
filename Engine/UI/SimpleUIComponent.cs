using Engine.Sprites;
using Engine.UI.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI;

public class SimpleUIComponent : AbstractUIComponent {

    public readonly Sprite Sprite;
    public Color Color = Color.White;

    public SimpleUIComponent(Sprite sprite, int x = 0, int y = 0, int w = 0, int h = 0, UIAnchorPosition anchorPosition = UIAnchorPosition.TOP_LEFT) : base(x, y, w, h, anchorPosition) {
        Sprite = sprite;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {
        Sprite.Draw(spriteBatch, ScreenArea, Color);
    }

    public void SetSpriteUV(int x, int y, int width, int height) {
        Sprite.MoveUV(x, y, width, height);
    }

#if DEBUG
    protected override void CreateDebugRenderer() {
        DebugRenderer = new SimpleRenderer(this);
    }
#endif

}
