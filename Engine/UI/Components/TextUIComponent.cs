using Engine.UI.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI;

public class TextUIComponent : AbstractUIComponent {

    public string Text { get; private set; }

    public TextUIComponent(int x, int y, string text = "") : base(x, y) {
        UpdateText(text);
    }

    public void UpdateText(string text) {
        Text = text;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {
        
    }

}
