using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Engine.Debugging;

public abstract class DebugMenu(string title) {

    protected readonly List<DebugMenu> Children = [];

    public readonly string Title = title;
    public ImGuiWindowFlags WindowFlags { get; protected set; } = ImGuiWindowFlags.None;
    public int X { get; protected set; }
    public int Y { get; protected set; }

    public int ChildCount { get { return Children.Count; } }

    public bool IsOpened = true;

    protected Viewport Viewport;

    public virtual void Show(Viewport viewport) {
        Resize(viewport);
        foreach (DebugMenu menu in Children) {
            menu.Show(viewport);
        }
    }

    public virtual void Resize(Viewport viewport) {
        Viewport = viewport;
        foreach (DebugMenu menu in Children) {
            menu.Show(viewport);
        }
    }

    public void Draw() {
        if (!IsOpened) return;

        PrepareWindow();

        ImGui.Begin(Title, ref IsOpened, WindowFlags);

        OnDrawMenu();

        ImGui.End();

        foreach (DebugMenu menu in Children) {
            menu.Draw();
        }
    }

    protected virtual void PrepareWindow() {
        ImGui.SetNextWindowPos(new(X, Y), ImGuiCond.Appearing);
    }

    protected abstract void OnDrawMenu();

    public DebugMenu GetChildAt(int i) {
        return Children[i];
    }

    public void AddChild(DebugMenu menu) {
        Children.Add(menu);
    }

    public void RemoveChild(DebugMenu menu) {
        Children.Remove(menu);
    }

    public void Toggle() {
        IsOpened = !IsOpened;
    }

}
