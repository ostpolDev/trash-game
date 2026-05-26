using Engine.Debugging;
using Engine.Serialization;
using Engine.UI.Debugging;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Engine.UI;

public abstract class AbstractUIComponent : IComparable<AbstractUIComponent>, ISerializable {

    public readonly string UID = Guid.NewGuid().ToString();
    public int RelativeZIndex { get; private set; } = 0;
    public int ZIndex { get; private set; } = 0;

    public Rectangle LocalArea { get; protected set; }
    public Rectangle ScreenArea { get; protected set; }

    public UIAnchorPosition AnchorPosition { get; protected set; }

    public AbstractUIComponent Parent { get; protected set; }
    public readonly List<AbstractUIComponent> Children = [];
    public bool HasParent { get { return Parent != null; } }
    public int ChildCount { get { return Children.Count; } }

    public string ReferenceID { get; private set; }

    private bool PositionRelativeToParent = true;

    public bool IsEnabled { get; private set; } = true;
    public bool IsParentDisabled { get; private set; } = false;

    public int Depth = 0;
    public int Index = 0;

    public bool ShouldDraw { get { return IsEnabled && !IsParentDisabled; } }

    public bool EnableScissor { get; protected set; }
    public Rectangle ScissorRectangle { get; protected set; }
    public Rectangle ScissorScreenRectangle { get; private set; }
    public UIAnchorPosition ScissorAnchor { get; private set; } = UIAnchorPosition.TOP_LEFT;

#if DEBUG
    protected UIDebugRenderer DebugRenderer;
#endif

    public readonly bool[] Stretch = new bool[2];
    public readonly int[] Padding = new int[4];

    public readonly bool[] ConstraintsEnabled = new bool[4];
    public readonly Vector2[] Constraints = new Vector2[2];

    public AbstractUIComponent(int x, int y, int width, int height, UIAnchorPosition anchorPosition) {
        LocalArea = new(x, y, width, height);
        AnchorPosition = anchorPosition;
        RecalculateScreenPosition();
    }

    public AbstractUIComponent(int x, int y, int width, int height) : this(x, y, width, height, UIAnchorPosition.TOP_LEFT) { }

    public AbstractUIComponent(int x, int y) : this(x, y, 0, 0, UIAnchorPosition.TOP_LEFT) { }

    public AbstractUIComponent() : this(0, 0, 0, 0, UIAnchorPosition.TOP_LEFT) { }

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch, float alpha);

    public void SetParent(AbstractUIComponent parent, bool updateScreenPosition = true) {
        Parent?.Children.Remove(this);
        Parent = parent;
        PositionRelativeToParent = updateScreenPosition;
        Depth = Parent?.Depth + 1 ?? 0;
        UpdateZIndex();
        RecalculateScreenPosition();
        UIManager.Singleton?.SortComponentZ();
    }

    public void UpdateZIndex() {
        if (Parent != null)
            ZIndex = RelativeZIndex + Parent.ZIndex;
        else
            ZIndex = RelativeZIndex;

        ZIndex += Depth;
        ZIndex += Index * 100;

        foreach (AbstractUIComponent child in Children)
            child.UpdateZIndex();
    }

    /// <summary>
    /// Returns a list of all components that need to be removed from the UI Manager
    /// </summary>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public List<AbstractUIComponent> Delete(bool recursive = true) {
        List<AbstractUIComponent> components = [];
        components.Add(this);
        foreach (AbstractUIComponent component in Children) {
            component.SetParent(null);
            if (recursive) {
                components.AddRange(component.Delete(true));
            }
        }
        return components;
    }

    public void SetZIndex(int index) {
        RelativeZIndex = index;
        UpdateZIndex();
        UIManager.Singleton?.SortComponentZ();
    }

    protected Rectangle GetBounds() {
        Rectangle bounds = new();
        if (HasParent) {
            bounds = Parent.LocalArea;
        } else if (BaseGame.Instance != null) {
            bounds.Width = BaseGame.Instance.GraphicsDevice.Viewport.Width;
            bounds.Height = BaseGame.Instance.GraphicsDevice.Viewport.Height;
        }
        return bounds;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="recursive">If children should be updated too</param>
    public void RecalculateScreenPosition(bool recursive = true) {
        Rectangle bounds = GetBounds();

        Rectangle localArea = LocalArea;

        localArea = RecalculateStretching(localArea, bounds);
        localArea = RecalculateConstraints(localArea);

        Rectangle newScreenArea = GetAnchorRelativeRectangle(localArea, bounds, AnchorPosition);
        if (PositionRelativeToParent && HasParent) {
            newScreenArea.X += Parent.ScreenArea.X;
            newScreenArea.Y += Parent.ScreenArea.Y;
        }

        ScreenArea = newScreenArea;
        RecalculateScissorScreenPosition();

        if (recursive)
            foreach (AbstractUIComponent component in Children)
                component.RecalculateScreenPosition();
    }

    protected void RecalculateScissorScreenPosition() {
        ScissorScreenRectangle = GetAnchorRelativeRectangle(ScissorRectangle, ScreenArea, ScissorAnchor);
    }

    protected Rectangle RecalculateStretching(Rectangle area, Rectangle bounds) {
        if (Stretch[(int)Utility.Plane.VERTICAL]) {
            area.Height = bounds.Bottom - Padding[0] - Padding[1];
            area.Y = bounds.Top + Padding[0];
        }
        if (Stretch[(int)Utility.Plane.HORIZONTAL]) {
            area.Width = bounds.Right - Padding[2] - Padding[3];
            area.X = bounds.Left + Padding[2];
        }
        return area;
    }

    protected Rectangle RecalculateConstraints(Rectangle area) {
        if (ConstraintsEnabled[0]) {
            area.Width = (int)Math.Max(area.Width, Constraints[0].X);
        }
        if (ConstraintsEnabled[1]) {
            area.Width = (int)Math.Min(area.Width, Constraints[0].Y);
        }
        if (ConstraintsEnabled[2]) {
            area.Height = (int)Math.Max(area.Height, Constraints[1].X);
        }
        if (ConstraintsEnabled[3]) {
            area.Height = (int)Math.Min(area.Height, Constraints[1].Y);
        }
        return area;
    }

    public static Rectangle GetAnchorRelativeRectangle(Rectangle rectangle, Rectangle bounds, UIAnchorPosition anchorPosition) {
        Vector2 pos = GetAnchorRelativePosition(rectangle, bounds, anchorPosition);
        return new((int)pos.X, (int)pos.Y, rectangle.Width, rectangle.Height);
    }

    public static Vector2 GetAnchorRelativePosition(Rectangle rectangle, Rectangle bounds, UIAnchorPosition anchorPosition) {
        return anchorPosition switch {
            UIAnchorPosition.TOP_CENTER => new(bounds.X + (bounds.Width / 2) - rectangle.Width / 2 + rectangle.X, bounds.Y + rectangle.Y),
            UIAnchorPosition.TOP_RIGHT => new(bounds.X + bounds.Width - rectangle.Width + rectangle.X, bounds.Y + rectangle.Y),
            UIAnchorPosition.CENTER_LEFT => new(bounds.X + rectangle.X, bounds.Y + (bounds.Height / 2) - rectangle.Height / 2 + rectangle.Y),
            UIAnchorPosition.CENTER_CENTER => new(bounds.X + (bounds.Width / 2) - rectangle.Width / 2 + rectangle.X, bounds.Y + (bounds.Height / 2) - rectangle.Height / 2 + rectangle.Y),
            UIAnchorPosition.CENTER_RIGHT => new(bounds.X + bounds.Width - rectangle.Width + rectangle.X, bounds.Y + (bounds.Height / 2) - rectangle.Height / 2 + rectangle.Y),
            UIAnchorPosition.BOTTOM_LEFT => new(bounds.X + rectangle.X, bounds.Y + bounds.Height - rectangle.Height + rectangle.Y),
            UIAnchorPosition.BOTTOM_CENTER => new(bounds.X + (bounds.Width / 2) - rectangle.Width / 2 + rectangle.X, bounds.Y + bounds.Height - rectangle.Height + rectangle.Y),
            UIAnchorPosition.BOTTOM_RIGHT => new(bounds.X + bounds.Width - rectangle.Width + rectangle.X, bounds.Y + bounds.Height - rectangle.Height + rectangle.Y),
            _ => new(rectangle.X, rectangle.Y),
        };
    }

    public void AddChild(AbstractUIComponent component) {
        component.SetParent(this);
        Children.Add(component);
    }

    public void RemoveChild(AbstractUIComponent component) {
        component.SetParent(null);
        Children.Remove(component);
    }

    public bool Intersects(AbstractUIComponent other) {
        return ScreenArea.Intersects(other.ScreenArea);
    }

    public bool Intersects(Rectangle other) {
        return ScreenArea.Intersects(other);
    }

    public int CompareTo(AbstractUIComponent other) {
        return ZIndex - other.ZIndex;
    }

    public void SetPosition(int x, int y) {
        Rectangle rect = LocalArea;
        rect.X = x;
        rect.Y = y;
        LocalArea = rect;
        RecalculateScreenPosition();
    }

    public void SetArea(int w, int h) {
        Rectangle rect = LocalArea;
        rect.Width = w;
        rect.Height = h;
        LocalArea = rect;
        RecalculateScreenPosition();
    }

    public void SetPositionAndArea(Rectangle rect) {
        LocalArea = rect;
        RecalculateScreenPosition();
    }

    public void SetAnchorPosition(UIAnchorPosition anchor) {
        AnchorPosition = anchor;
        RecalculateScreenPosition();
    }

    public virtual void SetReferenceID(string id) {
        ReferenceID = id;
    }

    public void SetStretching(Utility.Plane plane, bool value) {
        Stretch[(int)plane] = value;
        RecalculateScreenPosition();
    }

    public void SetPadding(Direction direction, int value) {
        Padding[(int)direction] = value;
        RecalculateScreenPosition();
    }

    public virtual void LoadData(SerializableDictionary dictionary) {
        LocalArea = dictionary.GetRectangle("position");
        AnchorPosition = (UIAnchorPosition)dictionary.GetByte("anchor");
        SetZIndex(dictionary.GetInt("z"));
        PositionRelativeToParent = dictionary.GetBool("relative_position");
        IsEnabled = dictionary.GetBool("enabled");

        EnableScissor = dictionary.GetBool("has_mask");
        ScissorRectangle = dictionary.GetRectangle("mask");
        ScissorAnchor = (UIAnchorPosition)dictionary.GetByte("mask_anchor");

        ReferenceID = dictionary.GetString("ref", null);

        RecalculateScreenPosition();
    }

    public virtual void WriteData(SerializableDictionary dictionary) {
        dictionary.Put("type", GetType().Name.ToString());
        dictionary.Put("position", LocalArea);
        dictionary.Put("anchor", (byte)AnchorPosition);
        if (HasParent)
            dictionary.Put("parent", Parent.UID);
        dictionary.Put("z", RelativeZIndex);
        dictionary.Put("relative_position", PositionRelativeToParent);
        dictionary.Put("enabled", IsEnabled);
        if (ReferenceID != null)
            dictionary.Put("ref", ReferenceID);

        dictionary.Put("has_mask", EnableScissor);
        dictionary.Put("mask", ScissorRectangle);
        dictionary.Put("mask_anchor", (byte)ScissorAnchor);
    }

    public void SetEnabled(bool isEnabled = true) {
        IsEnabled = isEnabled;
        foreach (AbstractUIComponent child in Children) {
            if (child.IsEnabled)
                child.UpdateParentDisabled(!IsEnabled);
        }
    }

    protected void UpdateParentDisabled(bool isDisabled) {
        IsParentDisabled = isDisabled;
        if (IsEnabled) {
            foreach (AbstractUIComponent child in Children) {
                child.UpdateParentDisabled(IsParentDisabled);
            }
        }
    }

    public void SetAllChildrenEnabled(bool recursive) {
        foreach (AbstractUIComponent child in Children) {
            child.SetEnabled(true);
            if (recursive)
                child.SetAllChildrenEnabled(recursive);
        }
    }

    public void SetScissor(Rectangle rectangle, bool enabled = true, UIAnchorPosition anchorPosition = UIAnchorPosition.TOP_LEFT) {
        EnableScissor = enabled;
        ScissorRectangle = rectangle;
        ScissorAnchor = anchorPosition;
        RecalculateScissorScreenPosition();
    }

    public void UpdateDepth(int i = 0, bool recursive = true) {
        Depth = i;
        if (recursive) {
            foreach (AbstractUIComponent child in Children) {
                child.UpdateDepth(i + 1);
            }
        }
    }

#if DEBUG

    protected virtual void CreateDebugRenderer() {
        DebugRenderer = new BaseRenderer(this);
    }

    public void DrawDebugMenu() {
        if (DebugRenderer == null) CreateDebugRenderer();
        DebugRenderer?.Render();
    }

#endif

}
