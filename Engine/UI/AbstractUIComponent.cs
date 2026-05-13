using Engine.UI.Debugging;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Engine.UI;

public abstract class AbstractUIComponent : IComparable<AbstractUIComponent> {

    public readonly string UID = Guid.NewGuid().ToString();
    private int zindex = 0;
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

    public bool IsEnabled = true;

    private UIDebugRenderer<AbstractUIComponent> DebugRenderer;

    public AbstractUIComponent(int x, int y, int width, int height, UIAnchorPosition anchorPosition) {
        LocalArea = new(x, y, width, height);
        AnchorPosition = anchorPosition;
        RecalculateScreenPosition();
    }

    public AbstractUIComponent(int x, int y, int width, int height) : this(x, y, width, height, UIAnchorPosition.TOP_LEFT) { }

    public AbstractUIComponent(int x, int y) : this(x, y, 0, 0, UIAnchorPosition.TOP_LEFT) { }

    public AbstractUIComponent() : this(0, 0, 0, 0, UIAnchorPosition.TOP_LEFT) { }

    public abstract void Draw(Microsoft.Xna.Framework.GameTime gameTime, SpriteBatch spriteBatch, float alpha);

    public void SetParent(AbstractUIComponent parent, bool updateScreenPosition = true) {
        Parent = parent;
        PositionRelativeToParent = updateScreenPosition;
        UpdateZIndex();
        RecalculateScreenPosition();
    }

    private void UpdateZIndex() {
        if (Parent != null)
            ZIndex = zindex + Parent.ZIndex;
        else
            ZIndex = zindex;

        foreach (AbstractUIComponent child in Children)
            child.UpdateZIndex();
    }

    public void SetZIndex(int index) {
        zindex = index;
        UpdateZIndex();
        UIManager.Singleton?.SortComponentDepth();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="deep">If children should be updated too</param>
    public void RecalculateScreenPosition(bool deep = true) {
        Rectangle bounds = new();
        if (HasParent) {
            bounds = Parent.LocalArea;
        } else if (BaseGame.Instance != null) {
            bounds.Width = BaseGame.Instance.GraphicsDevice.Viewport.Width;
            bounds.Height = BaseGame.Instance.GraphicsDevice.Viewport.Height;
        }

        Rectangle newScreenArea = GetAnchorRelativeRectangle(LocalArea, bounds, AnchorPosition);
        if (PositionRelativeToParent && HasParent) {
            newScreenArea.X = Parent.ScreenArea.X;
            newScreenArea.Y = Parent.ScreenArea.Y;
        }
        ScreenArea = newScreenArea;

        if (deep)
            foreach (AbstractUIComponent component in Children)
                component.RecalculateScreenPosition();
    }

    public static Rectangle GetAnchorRelativeRectangle(Rectangle rectangle, Rectangle bounds, UIAnchorPosition anchorPosition) {
        Vector2 pos = GetAnchorRelative(rectangle, bounds, anchorPosition);
        return new((int)pos.X, (int)pos.Y, rectangle.Width, rectangle.Height);
    }

    public static Vector2 GetAnchorRelative(Rectangle rectangle, Rectangle bounds, UIAnchorPosition anchorPosition) {
        return anchorPosition switch {
            UIAnchorPosition.TOP_CENTER => new((bounds.Width / 2) - rectangle.Width / 2 + rectangle.X, rectangle.Y),
            UIAnchorPosition.TOP_RIGHT => new(bounds.Width - rectangle.Width + rectangle.X, rectangle.Y),
            UIAnchorPosition.CENTER_LEFT => new(rectangle.X, (bounds.Height / 2) - rectangle.Height / 2 + rectangle.Y),
            UIAnchorPosition.CENTER_CENTER => new((bounds.Width / 2) - rectangle.Width / 2 + rectangle.X, (bounds.Height / 2) - rectangle.Height / 2 + rectangle.Y),
            UIAnchorPosition.CENTER_RIGHT => new(bounds.Width - rectangle.Width + rectangle.X, (bounds.Height / 2) - rectangle.Height / 2 + rectangle.Y),
            UIAnchorPosition.BOTTOM_LEFT => new(rectangle.X, bounds.Height - rectangle.Height + rectangle.Y),
            UIAnchorPosition.BOTTOM_CENTER => new((bounds.Width / 2) - rectangle.Width / 2 + rectangle.X, bounds.Height - rectangle.Height + rectangle.Y),
            UIAnchorPosition.BOTTOM_RIGHT => new(bounds.Width - rectangle.Width + rectangle.X, bounds.Height - rectangle.Height + rectangle.Y),
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

    protected virtual void CreateDebugRenderer() {
        DebugRenderer = new BaseRenderer(this);
    }

    public void DrawDebugMenu() {
        if (DebugRenderer == null) CreateDebugRenderer();
        DebugRenderer?.Render();
    }

}
