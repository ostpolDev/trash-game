using Engine.Serialization;
using Engine.Sprites;
using Engine.UI.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.UI.Components;

public class NineSliceComponent : SimpleUIComponent {

    const int SLICES = 9;

    public Rectangle SliceRectangle { get; private set; }

    private readonly Rectangle[] Slices = new Rectangle[SLICES * 2];

    public NineSliceComponent() : base() { }

    public NineSliceComponent(Sprite sprite, int x = 0, int y = 0, int w = 0, int h = 0, UIAnchorPosition anchorPosition = UIAnchorPosition.TOP_LEFT) : base(sprite, x, y, w, h, anchorPosition) { }

    public void SetSliceArea(Rectangle rectangle) {
        SliceRectangle = rectangle;
        RecalculateSlice();
    }

    public void RecalculateSlice() {
        Rectangle origin = Sprite.SourceRectangle;

        // UV Slices
        Slices[(int)UIAnchorPosition.TOP_LEFT] =        new(origin.X, origin.Y, SliceRectangle.X, SliceRectangle.Y);
        Slices[(int)UIAnchorPosition.TOP_CENTER] =      new(origin.X + GetSlice(UIAnchorPosition.TOP_LEFT).Width, origin.Y + GetSlice(UIAnchorPosition.TOP_LEFT).Height, SliceRectangle.Width, SliceRectangle.Y);
        Slices[(int)UIAnchorPosition.TOP_RIGHT] =       new(origin.X + GetSlice(UIAnchorPosition.TOP_LEFT).Width + GetSlice(UIAnchorPosition.TOP_CENTER).Width, origin.Y + GetSlice(UIAnchorPosition.TOP_LEFT).Height, SliceRectangle.X + SliceRectangle.Width - origin.Width, SliceRectangle.Y);

        Slices[(int)UIAnchorPosition.CENTER_LEFT] =     new(origin.X, origin.Y + GetSlice(UIAnchorPosition.TOP_LEFT).Height, SliceRectangle.X, SliceRectangle.Height);
        Slices[(int)UIAnchorPosition.CENTER_CENTER] =   new(origin.X + GetSlice(UIAnchorPosition.TOP_LEFT).Width, origin.Y + GetSlice(UIAnchorPosition.TOP_LEFT).Height, SliceRectangle.Width, SliceRectangle.Height);
        Slices[(int)UIAnchorPosition.CENTER_RIGHT] =    new(origin.X + GetSlice(UIAnchorPosition.TOP_LEFT).Width, origin.Y + GetSlice(UIAnchorPosition.TOP_LEFT).Width + GetSlice(UIAnchorPosition.CENTER_CENTER).Width, SliceRectangle.X + SliceRectangle.Width - origin.Width, SliceRectangle.Height);

        Slices[(int)UIAnchorPosition.BOTTOM_LEFT] =     new(origin.X, origin.Y + GetSlice(UIAnchorPosition.TOP_LEFT).Height + GetSlice(UIAnchorPosition.CENTER_LEFT).Height, SliceRectangle.X, origin.Height - GetSlice(UIAnchorPosition.TOP_LEFT).Height - GetSlice(UIAnchorPosition.CENTER_LEFT).Height);
        Slices[(int)UIAnchorPosition.BOTTOM_CENTER] =   new(origin.X + GetSlice(UIAnchorPosition.BOTTOM_LEFT).Width, GetSlice(UIAnchorPosition.BOTTOM_LEFT).Y, SliceRectangle.Width, GetSlice(UIAnchorPosition.BOTTOM_LEFT).Height);
        Slices[(int)UIAnchorPosition.BOTTOM_RIGHT] =    new(origin.X + GetSlice(UIAnchorPosition.BOTTOM_LEFT).Width + GetSlice(UIAnchorPosition.BOTTOM_CENTER).Width, GetSlice(UIAnchorPosition.BOTTOM_LEFT).Y, origin.Width - GetSlice(UIAnchorPosition.BOTTOM_CENTER).Width - GetSlice(UIAnchorPosition.BOTTOM_LEFT).Width, GetSlice(UIAnchorPosition.BOTTOM_LEFT).Height);

        // Screen Slices
        Slices[(int)UIAnchorPosition.TOP_LEFT + SLICES] =       new(ScreenArea.X, ScreenArea.Y, GetSlice(UIAnchorPosition.TOP_LEFT).Width, GetSlice(UIAnchorPosition.TOP_LEFT).Height);
        Slices[(int)UIAnchorPosition.TOP_CENTER + SLICES] =     new(ScreenArea.X + GetSlice(UIAnchorPosition.TOP_LEFT).Width, ScreenArea.Y, ScreenArea.Width - GetSlice(UIAnchorPosition.TOP_LEFT).Width - GetSlice(UIAnchorPosition.TOP_RIGHT).Width, GetSlice(UIAnchorPosition.TOP_CENTER).Height);
        Slices[(int)UIAnchorPosition.TOP_RIGHT + SLICES] =      new(ScreenArea.X + GetSlice(UIAnchorPosition.TOP_LEFT).Width + GetScreenSlice(UIAnchorPosition.TOP_CENTER).Width, ScreenArea.Y, GetSlice(UIAnchorPosition.TOP_RIGHT).Width, GetSlice(UIAnchorPosition.TOP_RIGHT).Height);

        Slices[(int)UIAnchorPosition.CENTER_LEFT + SLICES] =    new(ScreenArea.X, ScreenArea.Y + GetSlice(UIAnchorPosition.TOP_LEFT).Height, GetSlice(UIAnchorPosition.CENTER_LEFT).Width, ScreenArea.Height - GetSlice(UIAnchorPosition.TOP_LEFT).Height - GetSlice(UIAnchorPosition.BOTTOM_LEFT).Height);
        Slices[(int)UIAnchorPosition.CENTER_CENTER + SLICES] =  new(ScreenArea.X + GetSlice(UIAnchorPosition.CENTER_LEFT).Width, GetScreenSlice(UIAnchorPosition.CENTER_LEFT).Y, ScreenArea.Width - GetScreenSlice(UIAnchorPosition.CENTER_LEFT).Width - GetSlice(UIAnchorPosition.CENTER_RIGHT).Width, GetScreenSlice(UIAnchorPosition.CENTER_LEFT).Height);
        Slices[(int)UIAnchorPosition.CENTER_RIGHT + SLICES] =   new(ScreenArea.X + GetSlice(UIAnchorPosition.CENTER_LEFT).Width + GetScreenSlice(UIAnchorPosition.CENTER_CENTER).Width, GetScreenSlice(UIAnchorPosition.CENTER_LEFT).Y, GetSlice(UIAnchorPosition.CENTER_RIGHT).Width, GetScreenSlice(UIAnchorPosition.CENTER_LEFT).Height);

        Slices[(int)UIAnchorPosition.BOTTOM_LEFT + SLICES] =    new(ScreenArea.X, ScreenArea.Y + GetSlice(UIAnchorPosition.TOP_LEFT).Height + GetScreenSlice(UIAnchorPosition.CENTER_LEFT).Height, GetSlice(UIAnchorPosition.BOTTOM_LEFT).Width, GetSlice(UIAnchorPosition.BOTTOM_LEFT).Height);
        Slices[(int)UIAnchorPosition.BOTTOM_CENTER + SLICES] =  new(ScreenArea.X + GetSlice(UIAnchorPosition.BOTTOM_LEFT).Width, GetScreenSlice(UIAnchorPosition.BOTTOM_LEFT).Y, GetScreenSlice(UIAnchorPosition.TOP_CENTER).Width, GetSlice(UIAnchorPosition.BOTTOM_CENTER).Height);
        Slices[(int)UIAnchorPosition.BOTTOM_RIGHT + SLICES] =   new(ScreenArea.X + GetSlice(UIAnchorPosition.BOTTOM_LEFT).Width + GetScreenSlice(UIAnchorPosition.BOTTOM_CENTER).Width, GetScreenSlice(UIAnchorPosition.BOTTOM_LEFT).Y, GetSlice(UIAnchorPosition.BOTTOM_RIGHT).Width, GetSlice(UIAnchorPosition.BOTTOM_RIGHT).Height);
    }

    protected override void OnParentSizeUpdate(bool recrusive = true) {
        base.OnParentSizeUpdate(recrusive);
        RecalculateSlice();
    }

    public override void SetArea(int w, int h, bool updateChildren = true) {
        base.SetArea(w, h, updateChildren);
        RecalculateSlice();
    }

    private Rectangle GetSlice(UIAnchorPosition pos) {
        return Slices[(int)pos];
    }

    private Rectangle GetScreenSlice(UIAnchorPosition pos) {
        return Slices[(int)pos + SLICES];
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha) {

        for (int i = 0; i < SLICES; i++) {
            Sprite.Sheet.Draw(spriteBatch, Slices[SLICES + i], Slices[i], Color);
        }

    }

    public override void WriteData(SerializableDictionary dictionary) {
        base.WriteData(dictionary);
        dictionary.Put("slice", SliceRectangle);
    }

    public override void ReadData(SerializableDictionary dictionary) {
        base.ReadData(dictionary);
        SetSliceArea(dictionary.GetRectangle("slice"));
    }


#if DEBUG
    protected override void CreateDebugRenderer() {
        DebugRenderer = new NineSliceRenderer(this);
    }
#endif

}
