namespace Engine.UI.Debugging;

public abstract class UIDebugRenderer(AbstractUIComponent component) {

    public AbstractUIComponent Component = component;

    public abstract void Render();

}
