using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Engine.UI;

public abstract class AbstractUIComponent {

    public Rectangle LocalArea { get; protected set; }
    public Rectangle ScreenArea { get; protected set; }

    public UIAnchorPosition AnchorPosition { get; set; }

    public AbstractUIComponent Parent { get; protected set; }
    public readonly List<AbstractUIComponent> Children = [];
    public bool HasParent { get { return Parent != null; } }
    public int ChildCount { get { return Children.Count; } }

    private bool PositionRelativeToParent = true;

    public AbstractUIComponent(int x, int y, int width, int height, UIAnchorPosition anchorPosition) {
        LocalArea = new(x, y, width, height);
        AnchorPosition = anchorPosition;
        RecalculateScreenPosition();
    }

    public AbstractUIComponent(int x, int y, int width, int height) : this(x, y, width, height, UIAnchorPosition.TOP_LEFT) { }

    public AbstractUIComponent(int x, int y) : this(x, y, 0, 0, UIAnchorPosition.TOP_LEFT) { }

    public AbstractUIComponent() : this(0, 0, 0, 0, UIAnchorPosition.TOP_LEFT) { }

    public void SetParent(AbstractUIComponent parent, bool updateScreenPosition = true) {
        Parent = parent;
        PositionRelativeToParent = updateScreenPosition;
        RecalculateScreenPosition();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="deep">If children should be updated too</param>
    protected void RecalculateScreenPosition(bool deep = true) {
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
        return ScreenArea.IntersectsWith(other.ScreenArea);
    }

    public bool Intersects(Rectangle other) {
        return ScreenArea.IntersectsWith(other);
    }

}
