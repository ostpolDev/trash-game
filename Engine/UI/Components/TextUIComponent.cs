using Engine.Serialization;
using Engine.UI.Debugging;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace Engine.UI.Components;

public class TextUIComponent : AbstractUIComponent {

    public string Text { get; private set; }
    public string TextToRender { get; private set; }
    public int FontSize { get; private set; } = 18;
    public Color Color = Color.Black;
    private SpriteFontBase DynamicSpriteFont;
    public Vector2 Scale = Vector2.One;
    public float CharacterSpacing = 0f;
    public float LineSpacing = 0f;
    public TextStyle TextStyle = TextStyle.None;
    public TextAlignment Alignment { get; protected set; } = TextAlignment.LEFT;

    public TextWrapMode WrapMode { get; protected set; } = TextWrapMode.NONE;
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
        return Measure(TextToRender);
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
        WrapMode = (TextWrapMode)dictionary.GetByte("wrap");
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
        dictionary.Put("wrap", (byte)WrapMode);
        dictionary.Put("align", (byte)Alignment);
    }

    public void SetAlignment(TextAlignment alignment) {
        Alignment = alignment;
        UpdateTextRendering();
    }

    public void SetWrapMode(TextWrapMode mode) {
        WrapMode = mode;
        UpdateTextRendering();
    }

    public override void SetArea(int w, int h, bool updateChildren = true) {
        base.SetArea(w, h, updateChildren);
        UpdateTextRendering();
    }

    public override void SetPosition(int x, int y) {
        base.SetPosition(x, y);
        UpdateTextRendering();
    }

    public void UpdateTextRendering() {
        TextToRender = WrapMode == TextWrapMode.NONE ? Text : WrapText(Text);
        Vector2 size = Measure();
        LocalArea = new(LocalArea.X, LocalArea.Y, (int)size.X, (int)size.Y);
        RecalculateScreenPosition();
    }

    protected override void OnParentSizeUpdate(bool recrusive = true) {
        base.OnParentSizeUpdate(recrusive);
        UpdateTextRendering();
    }
    
    protected string WrapText(string text) {
        StringBuilder builder = new();

        switch (WrapMode) {
            case TextWrapMode.NONE:
                return text;
            case TextWrapMode.WORDS:
                WrapWords(builder, text);
                break;
            case TextWrapMode.CHARACTERS:
                WrapCharacters(builder, text);
                break;
            default:
                break;
        }

        return builder.ToString();
    }

    private int GetMaxTextX() {
        Rectangle bounds = GetScreenBounds();
        int xDiff = ScreenArea.X - bounds.X;
        return bounds.Width - xDiff;
    }

    private void WrapWords(StringBuilder builder, string text) {
        int maxWidth = GetMaxTextX();
        string[] words = text.Split(' ');

        for (int i = 0; i < words.Length; i++) {
            builder.Append(words[i]);
            builder.Append(' ');

            if (i > 0 && DynamicSpriteFont.MeasureString(builder).X > maxWidth) {
                builder.Remove(builder.Length - words[i].Length - 1, words[i].Length);
                builder.Append('\n').Append(words[i]).Append(' ');
            }
        }
    }

    private void WrapCharacters(StringBuilder builder, string text) {
        int maxWidth = GetMaxTextX();

        for (int i = 0; i < text.Length; i++) {
            builder.Append(text[i]);
            if (DynamicSpriteFont.MeasureString(builder).X > maxWidth) {
                builder.Remove(i, 1);
                builder.Append('\n').Append(text[i]);
            }
        }
    }

#if DEBUG
    protected override void CreateDebugRenderer() {
        DebugRenderer = new TextRenderer(this);
    }
#endif

    public enum TextAlignment {
        LEFT, CENTER, RIGHT
    }

    public enum TextWrapMode {
        NONE, WORDS, CHARACTERS
    }

} 
