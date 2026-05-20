using Engine.SceneManagement;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Editors;

#if DEBUG

public abstract class EditorScene(string name) : Scene(name) {

    public void Draw() {
        Menu();
        DrawScene();
    }

    /// <summary>
    /// Used to draw ImGui
    /// </summary>
    public abstract void DrawScene();

    private void Menu() {
        ImGui.BeginMainMenuBar();

        DrawMenu();

        ImGui.EndMainMenuBar();
    }

    protected abstract void DrawMenu();

}

#endif
