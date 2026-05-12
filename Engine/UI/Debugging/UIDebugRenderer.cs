namespace Engine.UI.Debugging;

public abstract class UIDebugRenderer<T>(T component) where T : AbstractUIComponent {

    public T Component = component;

    public abstract void Render();

}
