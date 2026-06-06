using Engine.Serialization;
using Engine.Serialization.Entries;
using Engine.UI.Debugging;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.UI.Components;

public abstract class AbstractUIComponent : IComparable<AbstractUIComponent>, ISerializable {

    public string UID { get; private set; } = Guid.NewGuid().ToString();
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

    public readonly bool[] ScissorStretch = new bool[2];
    public readonly int[] ScissorPadding = new int[4];

    public readonly bool[] ScissorConstraintsEnabled = new bool[4];
    public readonly Vector2[] ScissorConstraints = new Vector2[2];

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

    protected Rectangle GetScreenBounds() {
        Rectangle bounds = new();
        if (HasParent) {
            bounds = Parent.ScreenArea;
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

        localArea = RecalculateStretching(localArea, bounds, Stretch, Padding);
        localArea = RecalculateConstraints(localArea, ConstraintsEnabled, Constraints);

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
        Rectangle localArea = ScissorRectangle;

        localArea = RecalculateStretching(localArea, ScreenArea, ScissorStretch, ScissorPadding);
        localArea = RecalculateConstraints(localArea, ScissorConstraintsEnabled, ScissorConstraints);

        ScissorScreenRectangle = GetAnchorRelativeRectangle(localArea, ScreenArea, ScissorAnchor);
    }

    protected static Rectangle RecalculateStretching(Rectangle area, Rectangle bounds, bool[] stretching, int[] padding) {
        if (stretching[(int)Utility.Plane.VERTICAL]) {
            area.Height = bounds.Bottom - padding[0] - padding[1];
            area.Y = bounds.Top + padding[0];
        }
        if (stretching[(int)Utility.Plane.HORIZONTAL]) {
            area.Width = bounds.Right - padding[2] - padding[3];
            area.X = bounds.Left + padding[2];
        }
        return area;
    }

    protected static Rectangle RecalculateConstraints(Rectangle area, bool[] constraintsEnabled, Vector2[] constraints) {
        if (constraintsEnabled[0]) {
            area.Width = (int)Math.Max(area.Width, constraints[0].X);
        }
        if (constraintsEnabled[1]) {
            area.Width = (int)Math.Min(area.Width, constraints[0].Y);
        }
        if (constraintsEnabled[2]) {
            area.Height = (int)Math.Max(area.Height, constraints[1].X);
        }
        if (constraintsEnabled[3]) {
            area.Height = (int)Math.Min(area.Height, constraints[1].Y);
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
            _ => new(bounds.X + rectangle.X, bounds.Y + rectangle.Y),
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

    public virtual void SetPosition(int x, int y) {
        Rectangle rect = LocalArea;
        rect.X = x;
        rect.Y = y;
        LocalArea = rect;
        RecalculateScreenPosition();
    }

    public virtual void SetArea(int w, int h, bool updateChildren = true) {
        Rectangle rect = LocalArea;
        rect.Width = w;
        rect.Height = h;
        LocalArea = rect;
        RecalculateScreenPosition();
        foreach (AbstractUIComponent item in Children) {
            item.OnParentSizeUpdate();
        }
    }

    protected virtual void OnParentSizeUpdate(bool recrusive = true) {
        if (recrusive) {
            foreach (AbstractUIComponent item in Children) {
                item.OnParentSizeUpdate(recrusive);
            }
        }
    }

    public Vector2 GetScreenPosition() {
        return new(ScreenArea.X, ScreenArea.Y);
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

    public void SetID(string id) {
        UID = id;
    }

    public virtual void ReadData(SerializableDictionary dictionary) {
        LocalArea = dictionary.GetRectangle("position");
        AnchorPosition = (UIAnchorPosition)dictionary.GetByte("anchor");
        SetZIndex(dictionary.GetInt("z"));
        PositionRelativeToParent = dictionary.GetBool("relative_position");
        IsEnabled = dictionary.GetBool("enabled");

        EnableScissor = dictionary.GetBool("has_mask");
        if (EnableScissor) {
            ScissorRectangle = dictionary.GetRectangle("mask");
            ScissorAnchor = (UIAnchorPosition)dictionary.GetByte("mask_anchor");
        }

        ReferenceID = dictionary.GetString("ref", null);

        string parentUID = dictionary.GetString("parent");
        if (!string.IsNullOrEmpty(parentUID)) {
            UIManager.Singleton.FindByID(parentUID)?.AddChild(this);
        }

        // Responsive

        Stretch[0] = dictionary.GetBool("stretch_v");
        Stretch[1] = dictionary.GetBool("stretch_h");

        if (Stretch[0] || Stretch[1]) {
            ListEntry padding = dictionary.GetList("padding");
            for (int i = 0; i < Math.Min(padding.Data.Count, Padding.Length); i++) {
                Padding[i] = (int)padding.Data[i];
            }
        }

        if (dictionary.ContainsKey("constraints_enabled")) {
            ListEntry constraints_enabled = dictionary.GetList("constraints_enabled");
            for (int i = 0; i < Math.Min(constraints_enabled.Data.Count, ConstraintsEnabled.Length); i++) {
                ConstraintsEnabled[i] = (bool)constraints_enabled[i];
            }
        }

        if (dictionary.ContainsKey("constraints")) {
            ListEntry constraints = dictionary.GetList("constraints");
            for (int i = 0; i < Math.Min(constraints.Data.Count, Constraints.Length); i++) {
                Constraints[i] = (Vector2)constraints[i];
            }
        }

        if (EnableScissor) {
            ScissorStretch[0] = dictionary.GetBool("m_stretch_v");
            ScissorStretch[1] = dictionary.GetBool("m_stretch_h");

            ListEntry m_padding = dictionary.GetList("m_padding");
            for (int i = 0; i < Math.Min(m_padding.Data.Count, ScissorPadding.Length); i++) {
                ScissorPadding[i] = (int)m_padding[i];
            }

            ListEntry m_constraints_enabled = dictionary.GetList("m_constraints_enabled");
            for (int i = 0; i < Math.Min(m_constraints_enabled.Data.Count, ScissorConstraintsEnabled.Length); i++) {
                ScissorConstraintsEnabled[i] = (bool)m_constraints_enabled[i];
            }

            ListEntry m_constraints = dictionary.GetList("m_constraints");
            for (int i = 0; i < Math.Min(m_constraints.Data.Count, ScissorConstraints.Length); i++) {
                ScissorConstraints[i] = (Vector2)m_constraints[i];
            }
        }

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
        if (EnableScissor) {
            dictionary.Put("mask", ScissorRectangle);
            dictionary.Put("mask_anchor", (byte)ScissorAnchor);
        }

        // Responsive

        if (Stretch[0] || Stretch[1]) {
            dictionary.Put("stretch_v", Stretch[0]);
            dictionary.Put("stretch_h", Stretch[1]);
            dictionary.Put("padding", ListEntry.CreateFromData(DictionaryEntryType.INT, Padding));
        }

        if (ConstraintsEnabled.Contains(true)) {
            dictionary.Put("constraints_enabled", ListEntry.CreateFromData(DictionaryEntryType.BOOL, ConstraintsEnabled));
            dictionary.Put("constraints", ListEntry.CreateFromData(DictionaryEntryType.VECTOR2, Constraints));
        }

        if (EnableScissor) {
            dictionary.Put("m_stretch_v", ScissorStretch[0]);
            dictionary.Put("m_stretch_h", ScissorStretch[1]);
            dictionary.Put("m_padding", ListEntry.CreateFromData(DictionaryEntryType.INT, ScissorPadding));
            dictionary.Put("m_constraints_enabled", ListEntry.CreateFromData(DictionaryEntryType.BOOL, ScissorConstraintsEnabled));
            dictionary.Put("m_constraints", ListEntry.CreateFromData(DictionaryEntryType.VECTOR2, ScissorConstraints));
        }
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
