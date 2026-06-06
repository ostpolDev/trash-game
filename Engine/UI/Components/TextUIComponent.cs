using Engine.Serialization;
using Engine.UI.Debugging;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI.Components;

public class TextUIComponent : AbstractUIComponent {

    public string Text { get; private set; }
    public string TextToRender { get; private set; }
    public int FontSize { get; private set; } = 18;
    public Color Color = Color.White;
    private SpriteFontBase DynamicSpriteFont;
    public Vector2 Scale = Vector2.One;
    public float CharacterSpacing = 0f;
    public float LineSpacing = 0f;
    public TextStyle TextStyle = TextStyle.None;
    public TextAlignment Alignment { get; protected set; } = TextAlignment.LEFT;

    public bool WrapText = false;
    public bool RTL = false;

    public TextUIComponent() : base(0, 0) { }

    public TextUIComponent(int x, int y, string text = "", int fontSize = 18) : base(x, y) {
        UpdateText(text, fontSize);
    }

    public void UpdateText() {
        UpdateText(Text, FontSize);
    }

    public void UpdateText(string text) {
        Text = text ?? "";
        UpdateTextRendering();
    }

    public void UpdateText(string text, int fontSize) {
        Text = text ?? "";
        UpdateFontSize(fontSize);
    }

    public void UpdateFontSize(int size) {
        FontSize = size;
        DynamicSpriteFont = FontManager.FontSystem.GetFont(FontSize);
        UpdateTextRendering();
    }

    public Vector2 Measure() {
        return Measure(Text);
    }

    public Vector2 Measure(string text) {
        return DynamicSpriteFont.MeasureString(text, Scale, CharacterSpacing, LineSpacing);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {
        spriteBatch.DrawString(DynamicSpriteFont, TextToRender, GetScreenPosition(), Color, 0f, default, Scale, 0, CharacterSpacing, LineSpacing, TextStyle);
    }

    public override void ReadData(SerializableDictionary dictionary) {
        base.ReadData(dictionary);
        Text = dictionary.GetString("text");
        Color = dictionary.GetColor("color");
        Scale = dictionary.GetVector2("scale");
        CharacterSpacing = dictionary.GetFloat("char_s");
        LineSpacing = dictionary.GetFloat("line_s");
        TextStyle = (TextStyle)dictionary.GetInt("style", (int)TextStyle.None);
        RTL = dictionary.GetBool("rtl");
        WrapText = dictionary.GetBool("wrap");
        Alignment = (TextAlignment)dictionary.GetByte("align");

        UpdateText();
        SetAlignment(Alignment);
    }

    public override void WriteData(SerializableDictionary dictionary) {
        base.WriteData(dictionary);
        dictionary.Put("text", Text);
        dictionary.Put("color", Color);
        dictionary.Put("scale", Scale);
        dictionary.Put("char_s", CharacterSpacing);
        dictionary.Put("line_s", LineSpacing);
        dictionary.Put("style", (int)TextStyle);
        dictionary.Put("rtl", RTL);
        dictionary.Put("wrap", WrapText);
        dictionary.Put("align", (byte)Alignment);
    }

    public void SetAlignment(TextAlignment alignment) {
        Alignment = alignment;
        UpdateTextRendering();
    }

    public void UpdateTextRendering() {
        TextToRender = Text;
    }

#if DEBUG
    protected override void CreateDebugRenderer() {
        DebugRenderer = new TextRenderer(this);
    }
#endif

    public enum TextAlignment {
        LEFT, CENTER, RIGHT
    }

} 
