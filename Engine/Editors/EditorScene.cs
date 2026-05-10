using Engine.SceneManagement;

namespace Engine.Editors;

public abstract class EditorScene(string name) : Scene(name) {

    /// <summary>
    /// Used to draw ImGui
    /// </summary>
    public abstract void DrawScene();

}
