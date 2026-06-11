using Engine.Editors.Windows;
using Engine.SceneManagement;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Editors;

#if DEBUG

public abstract class EditorScene(string name) : Scene(name) {

    protected ConfirmationWindow ConfirmationWindow { get; private set; }

    public void Draw() {
        ConfirmationWindow?.Draw();
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

    protected void ShowConfirmationWindow(ConfirmationWindow.ConfirmationWindowCallback callback, string message, string title = "", ConfirmationWindow.Buttons buttons = ConfirmationWindow.Buttons.OK) {
        ConfirmationWindow = new((btn) => {
            callback?.Invoke(btn);
            ConfirmationWindow = null;
        }, message, title, buttons);
    }

    protected void ShowConfirmationWindow(ConfirmationWindow.ConfirmationWindowCallback callback, string message, ConfirmationWindow.Buttons buttons = ConfirmationWindow.Buttons.OK) {
        ShowConfirmationWindow(callback, message, "", buttons);
    }

    protected void ShowConfirmationWindow(string message, string title = "", ConfirmationWindow.Buttons buttons = ConfirmationWindow.Buttons.OK) {
        ConfirmationWindow = new((_) => {
            ConfirmationWindow = null;
        }, message, title, buttons);
    }

    protected void ShowConfirmationWindow(string message, ConfirmationWindow.Buttons buttons = ConfirmationWindow.Buttons.OK) {
        ShowConfirmationWindow(message, "", buttons);
    }

    protected void ShowConfirmationWindow(string message) {
        ShowConfirmationWindow(message, "", ConfirmationWindow.Buttons.OK);
    }

}

#endif
