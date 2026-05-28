using Engine.Debugging;
using Engine.Serialization;
using Engine.Sprites;
using Engine.UI.Debugging;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI.Components;

public class SimpleUIComponent : AbstractUIComponent {

    public Sprite Sprite { get; private set; }
    public Color Color = Color.White;

    public SimpleUIComponent() : base(0, 0, 0, 0, UIAnchorPosition.TOP_LEFT) { }

    public SimpleUIComponent(Sprite sprite, int x = 0, int y = 0, int w = 0, int h = 0, UIAnchorPosition anchorPosition = UIAnchorPosition.TOP_LEFT) : base(x, y, w, h, anchorPosition) {
        Sprite = sprite;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {
        Sprite.Draw(spriteBatch, ScreenArea, Color);
    }

    public void SetSpriteUV(int x, int y, int width, int height) {
        Sprite.MoveUV(x, y, width, height);
    }


    public override void LoadData(SerializableDictionary dictionary) {
        base.LoadData(dictionary);
        Color = dictionary.GetColor("color");
        Identifier spriteId = dictionary.GetIdentifier("sprite");
        Sprite = BaseGame.Instance.SpriteManager.GetSprite(spriteId);

        if (Sprite == null) {
            Logger.Shared.Error($"Failed to load sprite {spriteId} for UI component. Disabling component {UID} / {GetType().Name}.");
            SetEnabled(false);
        }
    }

    public override void WriteData(SerializableDictionary dictionary) {
        base.WriteData(dictionary);
        dictionary.Put("sprite", Sprite.Identifier);
        dictionary.Put("color", Color);
    }


#if DEBUG
    protected override void CreateDebugRenderer() {
        DebugRenderer = new SimpleRenderer(this);
    }

#endif

}
