using Engine.UI.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI;

public class EmptyUIComponent(int x, int y, int w, int h) : AbstractUIComponent(x, y, w, h) {

    public EmptyUIComponent(int x, int y) : this(x, y, 0, 0) { }
    public EmptyUIComponent() : this(0, 0, 0, 0) { }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) { }

}
